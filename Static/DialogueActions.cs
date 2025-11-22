using System;
using System.Collections.Generic;

namespace ShopGame.Static;

internal static class DialogueActions
{
  internal static readonly Dictionary<string, Action> Actions = new() {
    ["OpenShelf"] = static () => GlobalInstances.ShelfViewportContainer?.Activate()
  };
}
