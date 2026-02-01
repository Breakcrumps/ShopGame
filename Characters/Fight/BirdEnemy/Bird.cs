using Godot;
using ShopGame.Characters.Fight.Enemies;
using ShopGame.Extensions;
using ShopGame.Static;
using ShopGame.Types;
using ShopGame.Utils;

namespace ShopGame.Characters.Fight.BirdEnemy;

[GlobalClass]
internal sealed partial class Bird : Enemy
{
  [Export] private AnimationPlayer _animPlayer = null!;
  [Export] private StateMachine _moveStateMachine = null!;

  [ExportGroup("Hover Params")]
  [Export] private RayCast3D _raycast = null!;
  [Export] private float _heightDeadzone = 13f;
  [Export] private float _flapVelocity = 2f;
  [Export] private float _flapAccelRate = 80f;
  [Export] private float _flapCooldown = .2f;
  [Export] private float _g = 10f;

  internal float AttackRewindTimer { get; set; }

  private float _flapCooldownTimer = 0f;
  private bool _flapping;

  public override void _Ready()
  {
    foreach (State state in _moveStateMachine.States.Values)
    {
      if (state is not BirdState birdState)
        continue;

      birdState.Bird = this;
    }
  }
  
  public override void _PhysicsProcess(double delta)
  {
    _moveStateMachine.PhysicsProcess(delta);

    if (PushbackVelocity != Vector3.Zero)
    {
      Velocity = PushbackVelocity;
      PushbackVelocity = Vector3.Zero;
    }
    
    HandleTimers((float)delta);

    MoveAndSlide();
  }

  private protected override void HandleTimers(float deltaF)
  {
    base.HandleTimers(deltaF);
    
    if (
      DamagedNoHitTimer == 0
      && _animPlayer.CurrentAnimation == "Blink"
    )
      _animPlayer.Stop();

    if (HitArea.Collider.Disabled && DamagedNoHitTimer == 0f)
      HitArea.Collider.SetDeferred("disabled", false);

    AttackRewindTimer = Mathf.Max(AttackRewindTimer - deltaF, 0f);
  }

  public override void ProcessHit(Attack attack)
  {
    base.ProcessHit(attack);
    _animPlayer.Play("Blink");
    
    if (_moveStateMachine.IsValid() && _moveStateMachine.CurrentState is BirdLunge)
      _moveStateMachine.Transition<BirdFollow>();
  }

  internal void HandleHover(ref Vector3 nextVelocity, float deltaF)
  {
    if (PushbackTimer == 0f)
      nextVelocity.X = 0f;
    
    if (!_flapping)
    {
      nextVelocity.Y -= _g * deltaF;
    }
    else
    {
      nextVelocity.Y.ExpLerp(to: _flapVelocity, weight: _flapAccelRate * deltaF);

      if (nextVelocity.Y.IsEqualApprox(_flapVelocity))
        _flapping = false;
    }
    
    if (!_raycast.IsValid())
      return;

    if (_flapCooldownTimer != 0f)
    {
      _flapCooldownTimer = Mathf.Max(_flapCooldownTimer - deltaF, 0f);
      return;
    }

    _raycast.ForceRaycastUpdate();

    Vector3 difVector = _raycast.GetCollisionPoint() - GlobalPosition;

    if (_raycast.IsColliding() && difVector.Length() < _heightDeadzone)
    {
      _flapCooldownTimer = _flapCooldown;
      nextVelocity.Y.ExpLerp(to: _flapVelocity, weight: _flapAccelRate * deltaF);
      _flapping = true;
    }
  }

  internal void ResetDataBeforeStoppingFlapping()
    => _flapCooldownTimer = 0f;
}
