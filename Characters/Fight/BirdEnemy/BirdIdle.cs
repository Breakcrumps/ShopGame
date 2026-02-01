using Godot;
using ShopGame.Static;

namespace ShopGame.Characters.Fight.BirdEnemy;

[GlobalClass]
internal sealed partial class BirdIdle : BirdState
{
  [Export] private float _noticeDistance = 2f;
  
  internal override void PhysicsProcess(double delta)
  {
    Vector3 nextVelocity = Bird.Velocity;
    
    Bird.HandleHover(ref nextVelocity, (float)delta);

    if (GlobalInstances.FightGirl.IfValid() is not FightGirl fightGirl)
      return;

    Vector3 difVector = fightGirl.GlobalPosition - Bird.GlobalPosition;

    if (difVector.Length() <= _noticeDistance)
      StateMachine.Transition<BirdFollow>();

    Bird.Velocity = nextVelocity;
  }
}
