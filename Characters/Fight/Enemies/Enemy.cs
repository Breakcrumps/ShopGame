using Godot;
using ShopGame.Static;
using ShopGame.Types;
using ShopGame.Utils;

namespace ShopGame.Characters.Fight.Enemies;

[GlobalClass]
internal abstract partial class Enemy : CharacterBody2D
{
  [Export] private protected HurtArea? HurtArea { get; private set; }
  [Export] private int _health = 100;
  [Export] private protected float TurnRate { get; private set; } = 10f;
  [Export] private protected float PushBackTurnRate { get; private set; } = 1f;
  [Export] private float _pushbackTime = 1f;
  [Export] private float _damagedNoHitTime = 1f;

  private protected float PushbackTimer { get; set; }
  private protected float DamagedNoHitTimer { get; set; }

  private protected Vector2 PushbackVelocity { get; set; }

  internal virtual void ProcessHit(Attack attack)
  {
    if (!HurtArea.IsValid() || !HurtArea.Collider.IsValid())
      return;
    
    _health -= attack.Strength;

    if (_health <= 0)
      QueueFree();

    Vector2 pushbackDirection = GlobalPosition - attack.Attacker.GlobalPosition;
    PushbackVelocity = pushbackDirection.Normalized() * attack.PushbackMagnitude;

    PushbackTimer = _pushbackTime;
    DamagedNoHitTimer = _damagedNoHitTime;

    HurtArea.Collider.SetDeferred("disabled", true);
  }

  private protected abstract void HandleTimers(float deltaF);
}
