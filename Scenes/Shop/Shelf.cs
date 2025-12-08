using System.Collections.Generic;
using Godot;
using ShopGame.Static;
using ShopGame.Types;
using ShopGame.Utils;

namespace ShopGame.Scenes.Shop;

[GlobalClass]
internal sealed partial class Shelf : StaticBody2D, IActionHandler
{
  private readonly Dictionary<int, BoxItemType> _itemsOnShelf = new()
  {
    [ShelfPos.HashRowPos(0, 0)] = BoxItemType.Mayak,
    [ShelfPos.HashRowPos(0, 1)] = BoxItemType.Mayak,
    [ShelfPos.HashRowPos(1, 0)] = BoxItemType.Mayak
  };

  public void HandleAction(string actionName)
  {
    if (actionName != "OpenShelf")
      return;

    OpenShelf();
  }

  private void OpenShelf()
    => GlobalInstances.ShelfViewportContainer?.Activate(_itemsOnShelf);
}
