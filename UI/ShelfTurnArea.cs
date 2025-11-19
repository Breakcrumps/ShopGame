using System;
using Godot;

namespace ShopGame.UI;

internal enum TurnOrientation { Up, Down }

[GlobalClass]
internal sealed partial class ShelfTurnArea : Control
{
  [Export] private TurnOrientation _turnOrientation;

  internal event Action<TurnOrientation>? RequestingTurn;

  public override void _Ready()
    => MouseEntered += () => RequestingTurn?.Invoke(_turnOrientation);
}
