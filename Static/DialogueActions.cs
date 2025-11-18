using System;
using System.Collections.Generic;
using Godot;

namespace ShopGame.Static;

internal static class DialogueActions
{
  internal static readonly Dictionary<string, Action> Actions = new() {
    ["OpenShelf"] = OpenShelf
  };

  private static void OpenShelf()
    => GD.Print("Pretend like a shelf menu opened, please...");
}
