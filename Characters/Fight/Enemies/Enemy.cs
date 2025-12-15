using Godot;
using ShopGame.Types;

namespace ShopGame.Characters.Fight.Enemies;

[GlobalClass]
internal partial class Enemy : CharacterBody2D
{
  [Export] private int _health = 100;
  [Export] private protected float TurnRate { get; private set; } = 10f;
  [Export] private protected float PushBackTurnRate { get; private set; } = 5f;
  [Export] private protected float PushbackTime { get; private set; } = .2f;

  private protected float PushbackTimer { get; set; }

  private protected Vector2 PushbackVelocity { get; set; }

  internal void ProcessHit(Attack attack)
  {
    _health -= attack.Strength;

    if (_health <= 0)
      QueueFree();

    Vector2 pushbackDirection = GlobalPosition - attack.Attacker.GlobalPosition;
    PushbackVelocity = pushbackDirection.Normalized() * attack.PushbackMagnitude;

    PushbackTimer = PushbackTime;
  }
}
