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
  [Export] internal PackedScene? ToyPrefab { get; private set; }
  [Export] internal ShelfCamera? ShelfCamera { get; private set; }
  [Export] internal ShelfPosGroup? ShelfPosGroup { get; private set; }

  internal List<Toy> Toys { get; } = [];

  internal void SpawnToysInBox()
  {
    if (ToyPrefab is null)
      return;
    
    foreach (var (itemType, itemsOfType) in Inventory.Toys)
    {
      for (int i = 0; i < itemsOfType; i++)
      {
        Toys.Add(InstantiateItem(itemType));
      }
    }
  }

  internal void DespawnToysInBox()
  {
    for (int i = 0; i < Toys.Count; i++)
    {
      Toys[i].QueueFree();
      Toys.RemoveAt(i);
    }
  }

  internal Toy InstantiateItem(ToyType itemType)
  {
    Toy itemToStock = ToyPrefab!.Instantiate<Toy>();

    itemToStock.Initialise(itemType, this);

    return itemToStock;
  }
}
