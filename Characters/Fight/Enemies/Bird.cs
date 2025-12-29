using System;
using Godot;
using ShopGame.Extensions;
using ShopGame.Static;
using ShopGame.Types;
using ShopGame.Utils;

namespace ShopGame.Characters.Fight.Enemies;

[GlobalClass]
internal sealed partial class Bird : Enemy
{
  private enum MovementMode { Idle, Follow, FlapNearby, Lunge }
  private MovementMode _movementMode = MovementMode.Idle;

  private const float _girlHeightOffset = 14.5f;
  private const float _birdLegsOffset = 15f;

  #region Data
  [Export] private EnemyHitArea? _hitArea;
  [Export] private AnimationPlayer? _animPlayer;
  [Export] private float _attackRewind = 2f;

  [ExportGroup("Idle Params")]
  [Export] private float _noticeDistance = 100f;
  
  [ExportGroup("Follow Params")]
  [Export] private float _followSpeed = 50f;
  [Export] private float _stopDistance = 60f;
  [Export] private float _loseDistance = 300f;

  [ExportGroup("FlapNearby Params")]
  [Export] private float _refollowDistance = 75f;

  [ExportGroup("Lunge Params")]
  [Export] private float _lungeSpeed = 300f;
  [Export] private float _lungeDuration = .5f;
  [Export] private float _lungeLerpSpeed = 60f;
  [Export] private float _lungeDistance = 70f;
  [Export] private int _lungeDamage = 15;

  [ExportGroup("Hover Params")]
  [Export] private RayCast2D? _raycast;
  [Export] private float _heightDeadzone = 13f;
  [Export] private float _flapVelocity = 20f;
  [Export] private float _flapAccelRate = 80f;
  [Export] private float _flapCooldown = .2f;
  [Export] private float _g = 150f;

  private Vector2 _lungeDirection;
  private float _lungeTimer;

  private float _attackRewindTimer;

  private float _hoverHeight;
  private float _flapCooldownTimer = -1f;
  private bool _flapping;
  #endregion
  
  public override void _PhysicsProcess(double delta)
  {
    Vector2 nextVelocity = Velocity;
    
    HandleMovement(ref nextVelocity, (float)delta);
    HandleTimers((float)delta);

    Velocity = nextVelocity;

    MoveAndSlide();
  }

  private protected override void HandleTimers(float deltaF)
  {
    if (
      DamagedNoHitTimer == 0
      && _animPlayer.IsValid()
      && _animPlayer.CurrentAnimation == "Blink"
    )
      _animPlayer.Stop();

    if (
      HitArea.IsValid() && HitArea.Collider.IsValid()
      && HitArea.Collider.Disabled && DamagedNoHitTimer == 0f
    )
      HitArea.Collider.SetDeferred("disabled", false);

    PushbackTimer = Math.Max(PushbackTimer - deltaF, 0f);
    DamagedNoHitTimer = Mathf.Max(DamagedNoHitTimer - deltaF, 0f);
    _attackRewindTimer = Mathf.Max(_attackRewindTimer - deltaF, 0f);
  }

  public override void ProcessHit(Attack attack)
  {
    base.ProcessHit(attack);
    _animPlayer?.Play("Blink");
    
    if (_movementMode == MovementMode.Lunge)
      ExitLunge();
  }

  #region Movement
  private void HandleMovement(ref Vector2 nextVelocity, float deltaF)
  {
    switch (_movementMode)
    {
      case MovementMode.Idle:
        HandleIdle(ref nextVelocity, deltaF);
        break;
      
      case MovementMode.Follow:
        HandleFollow(ref nextVelocity, deltaF);
        break;

      case MovementMode.FlapNearby:
        HandleFlapNearby(ref nextVelocity, deltaF);
        break;

      case MovementMode.Lunge:
        HandleLunge(ref nextVelocity, deltaF);
        break;
    }

    if (PushbackVelocity != Vector2.Zero)
    {
      nextVelocity = PushbackVelocity;
      PushbackVelocity = Vector2.Zero;
    }
  }
  
  private void HandleIdle(ref Vector2 nextVelocity, float deltaF)
  {
    HandleHover(ref nextVelocity, deltaF);

    if (GlobalInstances.FightGirl.IfValid() is not FightGirl fightGirl)
      return;

    Vector2 difVector = fightGirl.GlobalPosition - GlobalPosition;

    if (difVector.Length() <= _noticeDistance)
      _movementMode = MovementMode.Follow;
  }
  
  private void HandleFollow(ref Vector2 nextVelocity, float deltaF)
  {
    if (GlobalInstances.FightGirl.IfValid() is not FightGirl fightGirl)
    {
      _movementMode = MovementMode.Idle;
      return;
    }

    Vector2 girlPos = fightGirl.GlobalPosition;
    girlPos.Y -= _girlHeightOffset;
    Vector2 direction = girlPos - GlobalPosition;
    float distToGirl = direction.Length();
    
    if (distToGirl <= _lungeDistance && _attackRewindTimer == 0f && DamagedNoHitTimer == 0f)
    {
      EnterLunge(ref nextVelocity, fightGirl, deltaF);
    }
    else if (distToGirl <= _stopDistance)
    {
      EnterFlapNearby(ref nextVelocity, deltaF);
    }
    else if (distToGirl >= _loseDistance)
    {
      _movementMode = MovementMode.Idle;
    }
    else
    {
      nextVelocity.ExpLerpV(
        to: direction.Normalized() * _followSpeed,
        rate: PushbackTimer > 0f ? PushBackTurnRate : TurnRate,
        param: deltaF
      ); 
    }
  }

  private void HandleFlapNearby(ref Vector2 nextVelocity, float deltaF)
  {
    HandleHover(ref nextVelocity, deltaF);

    if (GlobalInstances.FightGirl.IfValid() is not FightGirl fightGirl)
      return;

    Vector2 difVector = fightGirl.GlobalPosition - GlobalPosition;
    float distToGirl = difVector.Length();

    if (distToGirl >= _refollowDistance)
    {
      StopFlap();
      _movementMode = MovementMode.Follow;
    }
    else if (distToGirl <= _lungeDistance && _attackRewindTimer == 0f && DamagedNoHitTimer == 0f)
    {
      EnterLunge(ref nextVelocity, fightGirl, deltaF);
    }
  }

  private void HandleLunge(ref Vector2 nextVelocity, float deltaF)
  {
    nextVelocity = nextVelocity.ExpLerpedV(
      to: _lungeDirection * _lungeSpeed,
      rate: _lungeLerpSpeed,
      param: deltaF
    );

    _lungeTimer -= deltaF;

    if (_lungeTimer < 0f)
      ExitLunge();
  }
  #endregion

  #region MovementModeTransitions
  private void EnterFlapNearby(ref Vector2 nextVelocity, float deltaF)
  {
    _movementMode = MovementMode.FlapNearby;
    HandleHover(ref nextVelocity, deltaF);
  }

  private void EnterLunge(ref Vector2 nextVelocity, FightGirl fightGirl, float deltaF)
  {
    _movementMode = MovementMode.Lunge;

    if (_hitArea.IsValid())
      _hitArea.CurrentDamage = _lungeDamage;

    Vector2 girlPos = fightGirl.GlobalPosition;
    girlPos.Y -= _girlHeightOffset;

    Vector2 lungeDirection = girlPos - GlobalPosition;
    _lungeDirection = lungeDirection.Normalized();

    _lungeTimer = _lungeDuration;

    nextVelocity = nextVelocity.ExpLerpedV(
      to: _lungeDirection * _lungeSpeed,
      rate: _lungeLerpSpeed,
      param: deltaF
    );
  }

  private void ExitLunge()
  {
    _lungeTimer = 0f;
    _movementMode = MovementMode.Follow;
    _attackRewindTimer = _attackRewind;

    if (_hitArea.IsValid())
      _hitArea.CurrentDamage = _hitArea.IdleDamage;
  }
  #endregion

  #region Flap
  private void HandleHover(ref Vector2 nextVelocity, float deltaF)
  {
    if (PushbackTimer == 0f)
      nextVelocity.X = 0f;
    
    if (!_flapping)
    {
      nextVelocity.Y += _g * deltaF;
    }
    else
    {
      nextVelocity.Y.ExpLerp(to: -_flapVelocity, rate: _flapAccelRate, param: deltaF);

      if (nextVelocity.Y.IsEqualApprox(-_flapVelocity))
        _flapping = false;
    }
    
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
      nextVelocity.Y.ExpLerp(to: -_flapVelocity, rate: _flapAccelRate, param: deltaF);
      _flapping = true;
    }
    else if (_raycast.IsColliding() && difVector.Length() < _birdLegsOffset + _heightDeadzone)
    {
      _hoverHeight += _heightDeadzone;
      _flapCooldownTimer = _flapCooldown;
      nextVelocity.Y.ExpLerp(to: -_flapVelocity, rate: _flapAccelRate, param: deltaF);
      _flapping = true;
    }
  }

  private void StopFlap()
    => _flapCooldownTimer = -1f;
  #endregion
}
