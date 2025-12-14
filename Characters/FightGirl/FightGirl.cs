using Godot;

namespace ShopGame.Characters.FightGirl;

[GlobalClass]
internal sealed partial class FightGirl : CharacterBody2D
{
  [Export] private float _speed = 200f;
  [Export] private float _g = 1100f;

  [ExportGroup("Acceleration Rates")]
  [Export] private float _accelRate = 10f;
  [Export] private float _groundDecelRate = 30f;
  [Export] private float _airDecelRate = 20f;
  [Export] private float _turnDecelRate = 35f;

  [ExportGroup("Jump Params")]
  [Export] private float _jumpVelocity = 270f;
  [Export] private float _jumpCutoffFactor = .5f;

  [ExportGroup("Dash Params")]
  [Export] private float _dashStartSpeed = 390f;
  [Export] private float _dashDuration = .17f;
  [Export] private float _dashCutoffFactor = .8f;
  [Export] private float _dashCooldown = .4f;
  [Export] private bool _dashCancelsGravity = true;

  [ExportGroup("Attack Params")]
  [Export] internal int AttackStrength { get; private set; } = 10;
  [Export] internal float PushbackMagnitude { get; private set; } = 20f;

  private bool _inJump;
  private bool _doubleJump = true;

  private enum Direction { Left = -1, Right = 1 }
  private Direction _facingDirection = Direction.Right;

  private enum DashState { Ready, Dashing, Cooldown }
  private DashState _dashState;
  private float _dashTimer;
  private Direction _dashDirection;

  public override void _PhysicsProcess(double delta)
  {
    float xAxis = Input.GetAxis("Left", "Right");

    if (!Mathf.IsEqualApprox(xAxis, 0f))
      _facingDirection = xAxis < 0 ? Direction.Left : Direction.Right;

    float weight = (
      Mathf.IsEqualApprox(xAxis, 0f)
      ? IsOnFloor() ? _groundDecelRate : _airDecelRate
      : Mathf.Sign(Velocity.X) != Mathf.Sign(xAxis)
      ? _turnDecelRate
      : _accelRate
    );

    float xVelocity = Mathf.Lerp(
      from: Velocity.X,
      to: xAxis * _speed,
      weight: 1f - Mathf.Exp(-weight * (float)delta)
    );
    
    Vector2 nextVelocity = new(
      xVelocity,
      IsOnFloor() ? 0f : Velocity.Y + _g * (float)delta
    );

    HandleJump(ref nextVelocity);
    HandleDash(ref nextVelocity, (float)delta);

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

    if (Input.IsActionJustPressed("Jump"))
    {
      if (IsOnFloor())
      {
        nextVelocity.Y = -_jumpVelocity;
        _inJump = true;
      }
      else if (_doubleJump)
      {
        nextVelocity.Y = -_jumpVelocity;
        _doubleJump = false;
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
}
