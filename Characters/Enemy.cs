using Godot;
using ShopGame.Types;

namespace ShopGame.Characters;

[GlobalClass]
internal sealed partial class Enemy : CharacterBody2D
{
  [Export] private int _health = 100;
  [Export] private float _pushbackStopRate = 10f;

  private bool _inPushback;

  internal void ProcessHit(Attack attack)
  {
    _health -= attack.Strength;

    Vector2 pushbackDirection = GlobalPosition - attack.Attacker.GlobalPosition;
    Velocity = pushbackDirection.Normalized() * attack.PushbackMagnitude;
    MoveAndSlide();

    _inPushback = true;
  }

  public override void _PhysicsProcess(double delta)
  {
    if (!_inPushback)
      return;

    Velocity = Velocity.Lerp(
      to: Vector2.Zero,
      weight: 1 - Mathf.Exp(-_pushbackStopRate * (float)delta)
    );

    if (Velocity.IsZeroApprox())
    {
      _inPushback = false;
      Velocity = Vector2.Zero;
    }
  }
}
