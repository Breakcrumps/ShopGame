using System;
using System.Collections.Generic;
using ShopGame.Types;

namespace ShopGame.Static;

internal static class Inventory
{
  internal static Dictionary<ToyType, int> Toys { get; } = [];

  internal static void AddToy(ToyType toyType)
  {
    if (Toys.TryGetValue(toyType, out int value))
      Toys[toyType] = value + 1;
    else
      Toys.Add(toyType, 1);
  }

  internal static void RemoveToy(ToyType toyType)
  {
    if (!Toys.TryGetValue(toyType, out int count))
      return;

    if (count <= 1)
      Toys.Remove(toyType);
    else
      Toys[toyType] = count - 1;
  }

  internal static int GetToyCount(ToyType toyType)
  {
    if (!Toys.TryGetValue(toyType, out int count))
      return 0;

    return count;
  }

  internal static string GetBoxItemName(ToyType boxItemType)
    => Enum.GetName(boxItemType)!;
}
