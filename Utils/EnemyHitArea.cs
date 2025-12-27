using Godot;
using ShopGame.Characters.Fight;
using ShopGame.Types;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class EnemyHitArea : ColliderArea2D
{
  [Export] internal int IdleDamage { get; private set; } = 5;
  [Export] private float _pushbackMagnitude = 50f;

  internal int CurrentDamage { private get; set; }

  public override void _Ready()
  {
    CurrentDamage = IdleDamage;
    
    BodyEntered += node =>
    {
      if (node is not FightGirl fightGirl)
        return;

      Attack attack = new()
      {
        Strength = CurrentDamage,
        PushbackMagnitude = _pushbackMagnitude,
        Attacker = this
      };

      fightGirl.ProcessHit(attack);
    };
  }
}
