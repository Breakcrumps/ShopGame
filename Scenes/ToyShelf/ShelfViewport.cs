using System.Collections.Generic;
using Godot;
using ShopGame.Scenes.ToyShelf.In2D;
using ShopGame.Scenes.ToyShelf.In3D;
using ShopGame.Scenes.ToyShelf.Toys;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Scenes.ToyShelf;

[GlobalClass]
internal sealed partial class ShelfViewport : SubViewport
{
  [Export] internal PackedScene? BoxItemPrefab { get; private set; }
  [Export] internal ShelfCamera? ShelfCamera { get; private set; }
  [Export] internal ShelfPosGroup? ShelfPosGroup { get; private set; }

  internal List<BoxItem> BoxItems { get; } = [];

  internal void SpawnToysInBox()
  {
    if (BoxItemPrefab is null)
      return;
    
    foreach (var (itemType, itemsOfType) in Inventory.BoxItemQuantities)
    {
      for (int i = 0; i < itemsOfType; i++)
      {
        BoxItems.Add(InstantiateItem(itemType));
      }
    }
  }

  internal void DespawnToysInBox()
  {
    for (int i = 0; i < BoxItems.Count; i++)
    {
      BoxItems[i].QueueFree();
      BoxItems.RemoveAt(i);
    }
  }

  internal BoxItem InstantiateItem(BoxItemType itemType)
  {
    BoxItem itemToStock = BoxItemPrefab!.Instantiate<BoxItem>();

    itemToStock.Initialise(itemType, this);

    return itemToStock;
  }
}
