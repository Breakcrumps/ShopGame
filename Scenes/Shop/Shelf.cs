using System.Collections.Generic;
using Godot;
using ShopGame.Static;
using ShopGame.Types;
using ShopGame.Utils;

namespace ShopGame.Scenes.Shop;

[GlobalClass]
internal sealed partial class Shelf : StaticBody2D, IActionHandler
{
  internal Dictionary<int, ToyType> ItemsOnShelf { get; set; } = [];

  public void HandleAction(string actionName)
  {
    if (actionName == "OpenShelf")
      OpenShelf();
    
    void OpenShelf()
      => GlobalInstances.ShelfViewportContainer?.Activate(this);
  }
}
