using System.Collections.Generic;
using ShopGame.Types;

namespace ShopGame.Scenes.ToyShelf.Toys;

internal static class ToyParamDict
{
  internal static Dictionary<ToyType, ToyParams> ToyToParams { get; } = new()
  {
    [ToyType.Mayak] = new() { Cost = 1 }
  };
}
