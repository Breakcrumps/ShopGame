using System.Collections.Generic;
using Godot;
using ShopGame.Scenes.ToyShelf.Toys;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Scenes.ToyShelf.In3D;

[GlobalClass]
internal sealed partial class ShelfPosGroup : Node3D
{
  [Export] internal ShelfViewport? ShelfViewport { get; private set; }
  
  internal Dictionary<int, ShelfPosNode> ShelfPosDict { get; } = [];
  
  public override void _Ready()
  {
    foreach (Node node in GetChildren())
    {
      if (node is not ShelfPosNode shelfPos)
        continue;

      ShelfPosDict.Add(ShelfPos.HashRowPos(shelfPos.Row, shelfPos.Pos), shelfPos);
    }
  }

  internal void StockItems(Dictionary<int, ToyType> boxItems)
  {
    if (!ShelfViewport.IsValid())
      return;

    foreach (var (posHash, itemType) in boxItems)
    {
      if (!ShelfPosDict.ContainsKey(posHash))
        continue;

      Toy itemToStock = ShelfViewport.InstantiateItem(itemType);
      ShelfPosDict[posHash].PutItem(itemToStock);
    }
  }

  internal Dictionary<int, ToyType> GetItems()
  {
    Dictionary<int, ToyType> boxItems = [];

    foreach (var (posHash, posNode) in ShelfPosDict)
    {
      if (posNode.HeldItem is not Toy boxItem)
        continue;
      
      boxItems.Add(posHash, boxItem.ToyType);
    }

    return boxItems;
  }

  internal void DiscardItems()
  {
    foreach (var (_, posNode) in ShelfPosDict)
    {
      posNode.DestroyItem();
    }
  }
}
