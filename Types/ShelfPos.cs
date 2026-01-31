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

  internal static int HashRowPos(int row, int pos)
    => row >= pos ? row * row + row + pos : row + pos * pos;

  internal int GetHash() => (
    Row >= Pos
    ? Row * Row + Row + Pos
    : Row + Pos * Pos
  );
}
