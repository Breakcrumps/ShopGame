using System;
using System.Collections.Generic;
using ShopGame.Types;

namespace ShopGame.Static;

internal static class Inventory
{
  internal static Dictionary<BoxItemType, int> _boxItemQuantity = new()
  {
    [BoxItemType.Mayak] = 0
  };

  internal static string BoxItemName(BoxItemType boxItemType)
    => Enum.GetName(boxItemType) ?? "Mayak";
}
