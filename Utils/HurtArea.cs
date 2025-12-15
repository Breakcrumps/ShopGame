using Godot;
using ShopGame.Characters.Fight;
using ShopGame.Types;

namespace ShopGame.Utils;

[GlobalClass]
internal sealed partial class HurtArea : Area2D
{
  [Export] private int _damage = 10;
  [Export] private float _pushbackMagnitude = 50f;
  
  public override void _Ready()
  {
    BodyEntered += node =>
    {
      if (node is not FightGirl fightGirl)
        return;

      Attack attack = new()
      {
        Strength = _damage,
        PushbackMagnitude = _pushbackMagnitude,
        Attacker = this
      };

      fightGirl.ProcessHit(attack);
    };
  }
}
