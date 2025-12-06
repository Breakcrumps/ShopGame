using System.Collections.Generic;
using Godot;

namespace ShopGame.World;

[GlobalClass]
internal sealed partial class ShelfPosGroup : Node3D
{
  internal Dictionary<int, ShelfPosNode> ShelfPosDict = [];

   internal static int HashRowPos(int row, int pos)
    => row >= pos ? row * row + row + pos : row + pos * pos;
  
  public override void _Ready()
  {
    foreach (Node node in GetChildren())
    {
      if (node is not ShelfPosNode shelfPos)
        continue;

      ShelfPosDict.Add(HashRowPos(shelfPos.Row, shelfPos.Pos), shelfPos);
    }
  }
}
