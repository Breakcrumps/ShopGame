using System;
using Godot;
using ShopGame.Extensions;
using ShopGame.Static;
using ShopGame.Utils;

namespace ShopGame.Characters.Fight.Enemies;

[GlobalClass]
internal sealed partial class Bird : Enemy
{
  private enum MovementMode { Idle, Follow, FlapNearby, Windup, Lunge }
  private MovementMode _movementMode = MovementMode.Idle;

  [Export] private float _attackRewind = 2f;

  [ExportGroup("Idle Params")]
  [Export] private float _noticeDistance = 40f;
  
  [ExportGroup("Follow Params")]
  [Export] private float _followSpeed = 50f;
  [Export] private float _stopDistance = 40f;
  
  [ExportGroup("Windup Params")]
  [Export] private float _windupDuration = .2f;
  [Export] private float _windupAccelDuration = .1f;
  [Export] private Vector2 _windupStartVelocity = new(200f, 0f);
  [Export] private Vector2 _windupEndVelocity = new(0f, -200f);
  [Export] private float _windupStartLerpSpeed = 7f;
  [Export] private float _windupEndLerpSpeed = 5f;

  [ExportGroup("Lunge Params")]
  [Export] private float _lungeSpeed = 200f;
  [Export] private float _lungeDuration = .5f;
  [Export] private float _lungeLerpSpeed = 60f;

  [ExportGroup("Hover Params")]
  [Export] private RayCast2D? _raycast;
  [Export] private float _heightDeadzone = 13f;
  [Export] private float _flapVelocity = 100f;
  [Export] private float _flapAccelRate = 10f;
  [Export] private float _flapCooldown = .5f;
  [Export] private float _g = 200f;

  private float _windupTimer;

  private Vector2 _lungeDirection;
  private float _lungeTimer;

  private float _attackRewindTimer;

  private float _hoverHeight;
  private float _flapCooldownTimer = -1f;
  
  public override void _PhysicsProcess(double delta)
  {
    Vector2 nextVelocity = Velocity;
    
    HandleMovement(ref nextVelocity, (float)delta);
    HandleTimers((float)delta);

    Velocity = nextVelocity;

    MoveAndSlide();
  }

  private void HandleMovement(ref Vector2 nextVelocity, float deltaF)
  {
    switch (_movementMode)
    {
      case MovementMode.Idle:
      {
        Flap(ref nextVelocity, deltaF);

        if (GlobalInstances.FightGirl.IfValid() is not FightGirl fightGirl)
          break;

        Vector2 difVector = fightGirl.GlobalPosition - GlobalPosition;

        if (difVector.Length() <= _noticeDistance)
          _movementMode = MovementMode.Follow;
        
        break;
      }
      
      case MovementMode.Follow:
        HandleFollow(ref nextVelocity, deltaF);
        break;

      case MovementMode.Windup:
        HandleWindup(ref nextVelocity, deltaF);
        break;

      case MovementMode.Lunge:
      {
        nextVelocity = nextVelocity.Lerp(
          to: _lungeDirection * _lungeSpeed,
          weight: 1 - Mathf.Exp(-_lungeLerpSpeed * deltaF)
        );

        _lungeTimer -= deltaF;

        if (_lungeTimer < 0f)
        {
          _lungeTimer = 0f;
          _movementMode = MovementMode.Follow;
          _attackRewindTimer = _attackRewind;
        }
        
        break;
      }
    }
  }

  private protected override void HandleTimers(float deltaF)
  {
    PushbackVelocity = Vector2.Zero;
    PushbackTimer = Math.Max(PushbackTimer - deltaF, 0f);

    if (HurtArea.IsValid() && HurtArea.Collider.IsValid())
    {
      HurtArea.Collider.SetDeferred("disabled", DamagedNoHitTimer != 0f);
      DamagedNoHitTimer = Mathf.Max(DamagedNoHitTimer - deltaF, 0f);
    }
  }

  private void HandleFollow(ref Vector2 nextVelocity, float deltaF)
  {
    if (GlobalInstances.FightGirl.IfValid() is not FightGirl fightGirl)
    {
      _movementMode = MovementMode.Idle;
      return;
    }

    Vector2 girlPos = fightGirl.GlobalPosition;
    girlPos.Y -= 14.5f;
    Vector2 direction = girlPos - GlobalPosition;
    
    if (direction.Length() <= _stopDistance && _attackRewindTimer == 0f)
    {
      _movementMode = MovementMode.Windup;
      _windupTimer = _windupDuration + _windupAccelDuration;
      return;
    }
    
    _attackRewindTimer = Mathf.Max(_attackRewindTimer - deltaF, 0f);

    if (direction.Length() <= _stopDistance)
    {
      nextVelocity = Vector2.Zero;
      Flap(ref nextVelocity, deltaF);
      return;
    }

    StopFlap();

    float weight = (
      PushbackTimer > 0f
      ? PushBackTurnRate
      : TurnRate
    );

    nextVelocity = nextVelocity.Lerp(
      to: direction.Normalized() * _followSpeed,
      weight: 1 - Mathf.Exp(-weight * deltaF)
    );
    
    nextVelocity += PushbackVelocity;
  }

  private void Flap(ref Vector2 nextVelocity, float deltaF)
  {
    nextVelocity.Y += _g * deltaF;
    
    if (_flapCooldownTimer == -1f)
    {
      _hoverHeight = GlobalPosition.Y;
      _flapCooldownTimer = 0f;
    }
    
    if (!_raycast.IsValid())
      return;

    if (_flapCooldownTimer != 0f)
    {
      _flapCooldownTimer = Mathf.Max(_flapCooldownTimer - deltaF, 0f);
      return;
    }

    _raycast.ForceRaycastUpdate();

    Vector2 difVector = _raycast.GetCollisionPoint() - GlobalPosition;

    if (GlobalPosition.Y >= _hoverHeight)
    {
      _flapCooldownTimer = _flapCooldown;
      nextVelocity.Y.Lerp(-_flapVelocity, 1 - Mathf.Exp(-_flapAccelRate * deltaF));
    }
    else if (_raycast.IsColliding() && difVector.Length() < _heightDeadzone)
    {
      _hoverHeight += _heightDeadzone;
      _flapCooldownTimer = _flapCooldown;
      nextVelocity.Y.Lerp(-_flapVelocity, 1 - Mathf.Exp(-_flapAccelRate * deltaF));
    }
  }

  private void StopFlap()
    => _flapCooldownTimer = -1f;

  private void HandleWindup(ref Vector2 nextVelocity, float deltaF)
  {
    if (GlobalInstances.FightGirl.IfValid() is not FightGirl fightGirl)
      return;

    if (_windupTimer > _windupDuration)
    {
      Vector2 difVector = fightGirl.GlobalPosition - GlobalPosition;
      
      nextVelocity = nextVelocity.Lerp(
        to: difVector.X > 0f ? -_windupStartVelocity : _windupStartVelocity,
        weight: 1 - Mathf.Exp(-_windupStartLerpSpeed * deltaF)
      );
    }
    else
    {
      nextVelocity = nextVelocity.Lerp(
        to: _windupEndVelocity,
        weight: 1 - Mathf.Exp(-_windupEndLerpSpeed * deltaF)
      );
    }

    _windupTimer -= deltaF;

    if (_windupTimer <= 0f)
    {
      _windupTimer = 0f;
      _movementMode = MovementMode.Lunge;

      Vector2 girlPos = fightGirl.GlobalPosition;
      girlPos.Y -= 14.5f;

      Vector2 lungeDirection = girlPos - GlobalPosition;
      _lungeDirection = lungeDirection.Normalized();

      _lungeTimer = _lungeDuration;
    }
  }
}
