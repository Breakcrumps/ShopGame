using Godot;
using ShopGame.Characters;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class EnemyHitArea : ColliderArea3D
{
  [Export] internal int IdleDamage { get; private set; } = 5;
  [Export] private float _pushbackMagnitude = 5f;

  internal int CurrentDamage { private get; set; }

  internal bool EnemyHitThisFrame { private get; set; }
  private IHitProcessor? _targetHitThisFrame;

  public override void _Ready()
  {
    CurrentDamage = IdleDamage;
    
    BodyEntered += node =>
    {
      if (node is IHitProcessor hitProcessor)
        _targetHitThisFrame = hitProcessor;
    };
  }

  public override void _PhysicsProcess(double delta)
  {
    if (((GodotObject?)_targetHitThisFrame).IsValid() && !EnemyHitThisFrame)
      Hit(_targetHitThisFrame);

    EnemyHitThisFrame = false;
    _targetHitThisFrame = null;
  }

  private void Hit(IHitProcessor hitProcessor)
  {
    Attack attack = new()
    {
      Damage = CurrentDamage,
      PushbackMagnitude = _pushbackMagnitude,
      Attacker = this
    };

    hitProcessor.ProcessHit(attack);
  }
}
