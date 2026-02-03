using Godot;
using ShopGame.Static;

namespace ShopGame.Characters.Fight.BirdEnemy;

[GlobalClass]
internal sealed partial class BirdFlapNearby : BirdState
{
  [Export] private BirdLunge _lungeState = null!;

  [Export] private float _refollowDistance = 3f;
  
  internal override void PhysicsProcess(double delta)
  {
    Vector3 nextVelocity = Bird.Velocity;
    
    Bird.HandleHover(ref nextVelocity, (float)delta);

    Vector3 difVector = GlobalInstances.FightGirl.GlobalPosition - Bird.GlobalPosition;
    float distToGirl = difVector.Length();

    if (distToGirl >= _refollowDistance)
    {
      Bird.ResetDataBeforeStoppingFlapping();
      StateMachine.Transition<BirdFollow>();
    }
    else if (distToGirl <= _lungeState.LungeDistance && Bird is { AttackRewindTimer: 0f, DamagedNoHitTimer: 0f })
    {
      StateMachine.Transition<BirdLunge>();
    }

    Bird.Velocity = nextVelocity;
  }
}
