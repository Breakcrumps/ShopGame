using Godot;
using ShopGame.Extensions;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Characters.Fight;

[GlobalClass]
internal sealed partial class FightGirl : CharacterBody2D
{
  [Export] private AnimationPlayer? _animPlayer;
  [Export] private int _health = 100;
  [Export] private float _speed = 200f;
  [Export] private float _g = 1100f;

  [ExportGroup("Acceleration Rates")]
  [Export] private float _accelRate = 10f;
  [Export] private float _groundDecelRate = 30f;
  [Export] private float _airDecelRate = 20f;
  [Export] private float _turnDecelRate = 35f;
  [Export] private float _pushbackTurnRate = 5f;

  [ExportGroup("Jump Params")]
  [Export] private float _jumpVelocity = 270f;
  [Export] private float _jumpCutoffFactor = .5f;
  [Export] private float _coyoteTime = .15f;
  [Export] private float _jumpBufferTime = .1f;

  [ExportGroup("Dash Params")]
  [Export] private float _dashStartSpeed = 390f;
  [Export] private float _dashDuration = .17f;
  [Export] private float _dashCutoffFactor = .8f;
  [Export] private float _dashCooldown = .4f;
  [Export] private bool _dashCancelsGravity = true;

  [ExportGroup("Attack Params")]
  [Export] internal int AttackStrength { get; private set; } = 10;
  [Export] internal float PushbackMagnitude { get; private set; } = 50f;
  [Export] private float _pushbackStagger = .5f;
  [Export] private float _pogoStagger = .15f;
  [Export] private float _selfPushbackMagnitude = 10f;
  [Export] private float _pogoMagnitude = 300f;
  [Export] internal float AttackDuration { get; private set; } = .15f;
  [Export] private float _invulnTime = 2f;

  private bool _inJump;
  private bool _doubleJump = true;
  private float _coyoteTimer;
  private float _jumpBufferTimer;

  private enum Direction { Left = -1, Right = 1 }
  private Direction _facingDirection = Direction.Right;

  private enum DashState { Ready, Dashing, Cooldown }
  private DashState _dashState;
  private float _dashTimer;
  private Direction _dashDirection;

  internal bool InAttack { get; set; }
  private Vector2 _pushbackVelocity;
  private float _pushbackTimer;
  private float _invulnTimer;

  public override void _EnterTree()
    => GlobalInstances.FightGirl = this;

  public override void _PhysicsProcess(double delta)
  {
    HandleInvuln((float)delta);
    HandleMovement((float)delta);

    _pushbackTimer = Mathf.Max(_pushbackTimer - (float)delta, 0f);
  }

  private void HandleInvuln(float delta)
  {
    if (!_animPlayer.IsValid())
      return;
    
    _invulnTimer = Mathf.Max(_invulnTimer - (float)delta, 0f);

    if (_invulnTimer == 0f && _animPlayer.CurrentAnimation == "Blink")
      _animPlayer.Stop();
  }

  private void HandleMovement(float deltaF)
  {
    if (!_pushbackVelocity.IsZeroApprox())
    {
      Velocity = _pushbackVelocity;
      _pushbackVelocity = Vector2.Zero;
      MoveAndSlide();
      return;
    }
    
    float xAxis = Input.GetAxis("Left", "Right");

    if (!Mathf.IsEqualApprox(xAxis, 0f))
      _facingDirection = xAxis < 0 ? Direction.Left : Direction.Right;

    float weight = (
      _pushbackTimer > 0f
      ? _pushbackTurnRate
      : Mathf.IsEqualApprox(xAxis, 0f)
      ? IsOnFloor() ? _groundDecelRate : _airDecelRate
      : Mathf.Sign(Velocity.X) != Mathf.Sign(xAxis)
      ? _turnDecelRate
      : _accelRate
    );
    
    Vector2 nextVelocity = new(
      Velocity.X.ExpLerped(to: xAxis * _speed, rate: weight, param: deltaF),
      IsOnFloor() ? 0f : Velocity.Y + _g * deltaF
    );

    _coyoteTimer = IsOnFloor() ? _coyoteTime : Mathf.Max(_coyoteTimer - deltaF, 0f);

    _jumpBufferTimer = (
      Input.IsActionJustPressed("Jump")
      ? _jumpBufferTime
      : Mathf.Max(_jumpBufferTimer - deltaF, 0f)
    );
    
    HandleJump(ref nextVelocity);
    HandleDash(ref nextVelocity, deltaF);

    Velocity = nextVelocity;
    
    MoveAndSlide();
  }

  private void HandleJump(ref Vector2 nextVelocity)
  {
    if (IsOnFloor())
    {
      _inJump = false;
      _doubleJump = true;
    }

    if (_jumpBufferTimer > 0f)
    {
      if (_coyoteTimer > 0f)
      {
        nextVelocity.Y = -_jumpVelocity;
        _coyoteTimer = 0f;
        _jumpBufferTimer = 0f;
        _inJump = true;
      }
      else if (_doubleJump)
      {
        nextVelocity.Y = -_jumpVelocity;
        _doubleJump = false;
        _jumpBufferTimer = 0f;
      }
    }

    if (_inJump && Input.IsActionJustReleased("Jump"))
    {
      if (nextVelocity.Y < -_jumpVelocity * _jumpCutoffFactor)
        nextVelocity.Y = -_jumpVelocity * _jumpCutoffFactor;
      
      _inJump = false;
    }

    if (_inJump && IsOnCeiling())
    {
      _inJump = false;
    }
  }

  private void HandleDash(ref Vector2 nextVelocity, float deltaF)
  {
    switch (_dashState)
    {
      case DashState.Ready:
        TryStartDash(ref nextVelocity);
        break;
      
      case DashState.Dashing:
        ContinueDash(ref nextVelocity, deltaF);
        break;

      case DashState.Cooldown:
      {
        _dashTimer -= deltaF;

        if (_dashTimer <= 0f)
          _dashState = DashState.Ready;

        break;
      }
    }
  }

  private void TryStartDash(ref Vector2 nextVelocity)
  {
    if (!Input.IsActionJustPressed("Dash"))
      return;

    _dashDirection = _facingDirection;
    _dashState = DashState.Dashing;
    _dashTimer = _dashDuration;

    nextVelocity = new Vector2(
      (int)_dashDirection * _dashStartSpeed,
      _dashCancelsGravity ? 0f : nextVelocity.Y
    );
  }

  private void ContinueDash(ref Vector2 nextVelocity, float deltaF)
  {
    _dashTimer -= deltaF;

    if (_dashTimer <= 0f)
    {
      _dashState = DashState.Cooldown;
      _dashTimer = _dashCooldown;

      if (nextVelocity.Length() > _dashStartSpeed * _dashCutoffFactor)
        nextVelocity *= _dashCutoffFactor;
    }
    else
    {
      nextVelocity = new Vector2(
        (int)_dashDirection * _dashStartSpeed,
        _dashCancelsGravity ? 0f : nextVelocity.Y
      );
    }
  }

  internal void HandlePushback(Vector2 pushbackDirection, bool pogo)
  {
    float magnitude = pogo ? _pogoMagnitude : _selfPushbackMagnitude;
    _pushbackVelocity += pushbackDirection * magnitude;
    _pushbackTimer = pogo ? _pogoStagger : _pushbackStagger;
  }

  internal void ProcessHit(Attack attack)
  {
    if (_invulnTimer != 0f)
      return;
    
    _health -= attack.Strength;

    Vector2 pushbackDirection = GlobalPosition - attack.Attacker.GlobalPosition;
    _pushbackVelocity = pushbackDirection.Normalized() * attack.PushbackMagnitude;

    _invulnTimer = _invulnTime;

    _animPlayer?.Play("Blink");
  }
}
