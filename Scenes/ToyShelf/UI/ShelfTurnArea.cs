using System;
using Godot;

namespace ShopGame.Scenes.ToyShelf.UI;

[GlobalClass]
internal sealed partial class ShelfTurnArea : Control
{
  [Export] internal TurnOrientation FlickDirection { get; private set; }

  internal event Action<TurnOrientation>? RequestingTurn;

  public override void _Ready()
    => MouseEntered += () => RequestingTurn?.Invoke(FlickDirection);
}
