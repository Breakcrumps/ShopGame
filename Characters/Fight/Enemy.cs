using Godot;
using ShopGame.Types;
using ShopGame.Utils;

namespace ShopGame.Characters.Fight.Enemies;

[GlobalClass]
internal partial class Enemy : CharacterBody3D, IHitProcessor
{
  [Export] private protected EnemyHitArea HitArea { get; private set; } = null!;
  [Export] private int _health = 100;
  [Export] internal float TurnRate { get; private set; } = 10f;
  [Export] internal float PushBackTurnRate { get; private set; } = 1f;
  [Export] private float _pushbackTime = .1f;
  [Export] private float _damagedNoHitTime = 1f;

  internal float PushbackTimer { get; private protected set; }
  internal float DamagedNoHitTimer { get; private protected set; }

  private protected Vector3 PushbackVelocity { get; set; }

  public virtual void ProcessHit(Attack attack)
  {
    _health -= attack.Damage;

    if (_health <= 0)
    {
      QueueFree();
      return;
    }

    Vector3 pushbackDirection = GlobalPosition - attack.Attacker.GlobalPosition;
    PushbackVelocity = pushbackDirection.Normalized() * attack.PushbackMagnitude;

    PushbackTimer = _pushbackTime;
    DamagedNoHitTimer = _damagedNoHitTime;

    HitArea.Collider.SetDeferred("disabled", true);
  }

  private protected virtual void HandleTimers(float deltaF)
  {
    PushbackTimer = Mathf.Max(PushbackTimer - deltaF, 0f);
    DamagedNoHitTimer = Mathf.Max(DamagedNoHitTimer - deltaF, 0f);
  }
}
