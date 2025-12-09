using System;
using System.Collections.Generic;
using ShopGame.Types;

namespace ShopGame.Static;

internal static class DialogueActions
{
  internal static Dictionary<string, Action> Actions { get; } = new()
  {
    ["AddMayak"] = static () => Inventory.AddToy(ToyType.Mayak)
  };
}
