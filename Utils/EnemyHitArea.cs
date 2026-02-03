using Godot;
using ShopGame.Characters.Fight.Girl;
using ShopGame.Types;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class EnemyHitArea : ColliderArea3D
{
  [Export] internal int IdleDamage { get; private set; } = 5;
  [Export] private float _pushbackMagnitude = 5f;

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
        Damage = CurrentDamage,
        PushbackMagnitude = _pushbackMagnitude,
        Attacker = this
      };

      fightGirl.ProcessHit(attack);
    };
  }
}
