using Godot;
using ShopGame.Extensions;
using ShopGame.Static;

namespace ShopGame.Characters.Fight.BirdEnemy;

[GlobalClass]
internal sealed partial class BirdFollow : BirdState
{
  [Export] private BirdLunge _lungeState = null!;
  
  [Export] private float _followSpeed = 50f;
  [Export] private float _stopDistance = 60f;
  [Export] private float _loseDistance = 300f;
  
  internal override void PhysicsProcess(double delta)
  {
    Vector3 girlPos = GlobalInstances.FightGirl.GlobalPosition;
    Vector3 direction = girlPos - Bird.GlobalPosition;
    float distToGirl = direction.Length();

    if (distToGirl <= _lungeState.LungeDistance && Bird is { AttackRewindTimer: 0f, DamagedNoHitTimer: 0f })
    {
      StateMachine.Transition<BirdLunge>();
    }
    else if (distToGirl <= _stopDistance)
    {
      StateMachine.Transition<BirdFlapNearby>();
    }
    else if (distToGirl >= _loseDistance)
    {
      StateMachine.Transition<BirdIdle>();
    }
    else
    {
      Bird.Velocity = Bird.Velocity.ExpLerpedVec3(
        to: direction.Normalized() * _followSpeed,
        weight: (Bird.PushbackTimer > 0f ? Bird.PushBackTurnRate : Bird.TurnRate) * (float)delta
      ); 
    }
  }
}
