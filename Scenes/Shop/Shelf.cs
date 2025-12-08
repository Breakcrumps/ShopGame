using System.Collections.Generic;
using Godot;
using ShopGame.Static;
using ShopGame.Types;
using ShopGame.Utils;

namespace ShopGame.Scenes.Shop;

[GlobalClass]
internal sealed partial class Shelf : StaticBody2D, IActionHandler
{
  internal Dictionary<int, BoxItemType> ItemsOnShelf { get; set; } = [];

  public void HandleAction(string actionName)
  {
    if (actionName == "OpenShelf")
      OpenShelf();
  }

  private void OpenShelf()
    => GlobalInstances.ShelfViewportContainer?.Activate(this);
}
