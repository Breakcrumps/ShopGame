using ShopGame.Types;

namespace ShopGame.Characters;

internal interface IHitProcessor
{
  void ProcessHit(Attack attack);
}
