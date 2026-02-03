using Godot;
using ShopGame.Extensions;
using ShopGame.Static;
using ShopGame.Utils;

namespace ShopGame.Characters.Fight.BirdEnemy;

[GlobalClass]
internal sealed partial class BirdLunge : BirdState
{
  [Export] private EnemyHitArea _hitArea = null!;
  [Export] internal float LungeDistance { get; private set; } = 3f;
  [Export] private float _lungeSpeed = 15f;
  [Export] private float _lungeDuration = .3f;
  [Export] private float _lungeRewind = 2f;
  [Export] private float _lungeLerpSpeed = 20f;
  [Export] private int _lungeDamage = 15;

  private Vector3 _lungeDirection;
  private float _lungeTimer;

  internal override void Enter()
  {
    _hitArea.CurrentDamage = _lungeDamage;

    Vector3 girlPos = GlobalInstances.FightGirl.GlobalPosition;

    Vector3 lungeDirection = girlPos - Bird.GlobalPosition;
    _lungeDirection = lungeDirection.Normalized();

    _lungeTimer = _lungeDuration;
  }

  internal override void PhysicsProcess(double delta)
  {
    if (Bird.IsOnWall() || Bird.IsOnFloor() || Bird.IsOnCeiling())
    {
      StateMachine.Transition<BirdFollow>();
      return;
    }
    
    Bird.Velocity = Bird.Velocity.ExpLerpedVec3(
      to: _lungeDirection * _lungeSpeed,
      weight: _lungeLerpSpeed * (float)delta
    );

    _lungeTimer -= (float)delta;

    if (_lungeTimer < 0f)
      StateMachine.Transition<BirdFollow>();
  }

  internal override void Exit()
  {
    _lungeTimer = 0f;
    Bird.AttackRewindTimer = _lungeRewind;

    if (_hitArea.IsValid())
      _hitArea.CurrentDamage = _hitArea.IdleDamage;
  }
}
