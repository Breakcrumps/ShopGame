namespace ShopGame.Types;

internal readonly struct ShelfPos
{
  internal int Row { get; }
  internal int Pos { get; }  

  internal ShelfPos(int row, int pos)
  {
    Row = row;
    Pos = pos;
  }
}
