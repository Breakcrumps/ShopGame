using System.Collections.Generic;
using Godot;
using ShopGame.Types;

namespace ShopGame.World;

[GlobalClass]
internal sealed partial class ShelfPosGroup : Node3D
{
  internal Dictionary<int, ShelfPosNode> ShelfPosDict = [];

   internal static int HashRowPos(int row, int pos)
    => row >= pos ? row * row + row + pos : row + pos * pos;

   internal static int HashRowPos(ShelfPos shelfPos) => (
    shelfPos.Row >= shelfPos.Pos
    ? shelfPos.Row * shelfPos.Row + shelfPos.Row + shelfPos.Pos
    : shelfPos.Row + shelfPos.Pos * shelfPos.Pos
   );
  
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
