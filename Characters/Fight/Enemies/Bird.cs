using Godot;
using ShopGame.Static;

namespace ShopGame.Characters.Fight.Enemies;

[GlobalClass]
internal sealed partial class Bird : Enemy
{
  [Export] private float _speed = 50f;
  [Export] private float _stopDistance = 50f;
  
  public override void _PhysicsProcess(double delta)
  {
    if (GlobalInstances.FightGirl.IfValid() is not FightGirl fightGirl)
      return;
    
    float weight = (
      PushbackTimer > 0f
      ? PushBackTurnRate
      : TurnRate
    );

    Vector2 direction = fightGirl.GlobalPosition - GlobalPosition;
    
    if (direction.Length() <= _stopDistance)
      direction = Vector2.Zero;

    Velocity = Velocity.Lerp(
      to: direction.Normalized() * _speed,
      weight: 1 - Mathf.Exp(-weight * (float)delta)
    );
    
    Velocity += PushbackVelocity;
    PushbackVelocity = Vector2.Zero;

    MoveAndSlide();
  }
}
