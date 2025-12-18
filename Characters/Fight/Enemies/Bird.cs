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

  #region Data
  [Export] private AnimationPlayer? _animPlayer;
  [Export] private float _attackRewind = 2f;

  [ExportGroup("Idle Params")]
  [Export] private float _noticeDistance = 100f;
  
  [ExportGroup("Follow Params")]
  [Export] private float _followSpeed = 50f;
  [Export] private float _stopDistance = 60f;
  [Export] private float _loseDistance = 150f;

  [ExportGroup("FlapNearby Params")]
  [Export] private float _refollowDistance = 75f;

  [ExportGroup("Lunge Params")]
  [Export] private float _lungeSpeed = 200f;
  [Export] private float _lungeDuration = .5f;
  [Export] private float _lungeLerpSpeed = 60f;
  [Export] private float _lungeDistance = 60f;

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

  #region TopLevelHandlers
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

      case MovementMode.FlapNearby:
        HandleFlapNearby(ref nextVelocity, deltaF);
        break;

      case MovementMode.Lunge:
      {
        nextVelocity = nextVelocity.ExpLerp(
          to: _lungeDirection * _lungeSpeed,
          rate: _lungeLerpSpeed,
          param: deltaF
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

    if (
      PushbackTimer == 0
      && _animPlayer.IsValid()
      && _animPlayer.CurrentAnimation == "Blink"
    )
      _animPlayer.Stop();

    if (HurtArea.IsValid() && HurtArea.Collider.IsValid())
    {
      HurtArea.Collider.SetDeferred("disabled", DamagedNoHitTimer != 0f);
      DamagedNoHitTimer = Mathf.Max(DamagedNoHitTimer - deltaF, 0f);
    }

    _attackRewindTimer = Mathf.Max(_attackRewindTimer - deltaF, 0f);
  }
  #endregion

  #region MovementModeHandlers
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
    float distToGirl = direction.Length();
    
    if (distToGirl <= _lungeDistance && _attackRewindTimer == 0f)
    {
      EnterLunge(ref nextVelocity, fightGirl, deltaF);
      return;
    }
    if (distToGirl <= _stopDistance)
    {
      EnterFlapNearby(ref nextVelocity, deltaF);
      return;
    }
    if (distToGirl >= _loseDistance)
    {
      _movementMode = MovementMode.Idle;
      return;
    }

    nextVelocity = nextVelocity.ExpLerp(
      to: direction.Normalized() * _followSpeed,
      rate: PushbackTimer > 0f ? PushBackTurnRate : TurnRate,
      param: deltaF
    );

    nextVelocity += PushbackVelocity;
  }

  private void HandleFlapNearby(ref Vector2 nextVelocity, float deltaF)
  {
    Flap(ref nextVelocity, deltaF);

    if (GlobalInstances.FightGirl.IfValid() is not FightGirl fightGirl)
      return;

    Vector2 difVector = fightGirl.GlobalPosition - GlobalPosition;

    float distToGirl = difVector.Length();

    if (distToGirl >= _refollowDistance)
    {
      StopFlap();
      _movementMode = MovementMode.Follow;
    }
    else if (distToGirl <= _lungeDistance && _attackRewindTimer == 0f)
      EnterLunge(ref nextVelocity, fightGirl, deltaF);
  }
  #endregion

  #region MovementModeEntrances
  private void EnterFlapNearby(ref Vector2 nextVelocity, float deltaF)
  {
    _movementMode = MovementMode.FlapNearby;
    Flap(ref nextVelocity, deltaF);
  }

  private void EnterLunge(ref Vector2 nextVelocity, FightGirl fightGirl, float deltaF)
  {
    _movementMode = MovementMode.Lunge;

    Vector2 girlPos = fightGirl.GlobalPosition;
    girlPos.Y -= 14.5f;

    Vector2 lungeDirection = girlPos - GlobalPosition;
    _lungeDirection = lungeDirection.Normalized();

    _lungeTimer = _lungeDuration;

    nextVelocity = nextVelocity.ExpLerp(
      to: _lungeDirection * _lungeSpeed,
      rate: _lungeLerpSpeed,
      param: deltaF
    );
  }
  #endregion

  #region Flap
  private void Flap(ref Vector2 nextVelocity, float deltaF)
  {
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
    else if (_raycast.IsColliding() && difVector.Length() < 15f + _heightDeadzone)
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

  public override void ProcessHit(Attack attack)
  {
    base.ProcessHit(attack);
    _animPlayer?.Play("Blink");
  }
}
