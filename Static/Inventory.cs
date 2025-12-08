using System;
using System.Collections.Generic;
using ShopGame.Types;

namespace ShopGame.Static;

internal static class Inventory
{
  internal static Dictionary<BoxItemType, int> BoxItemQuantities { get; } = new()
  {
    [BoxItemType.Mayak] = 3
  };

  internal static string GetBoxItemName(BoxItemType boxItemType)
    => Enum.GetName(boxItemType) ?? "Mayak";
}
