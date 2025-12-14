using ShopGame.Characters.FightGirl;

namespace ShopGame.Types;

internal readonly struct Attack
{
  internal required int Strength { get; init; }
  internal required float PushbackMagnitude { get; init; }
  internal required FightGirl Attacker { get; init; }
}
