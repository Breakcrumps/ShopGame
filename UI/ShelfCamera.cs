using Godot;
using ShopGame.Static;

namespace ShopGame.UI;

[GlobalClass]
internal sealed partial class ShelfCamera : Camera3D
{
  [Export] private CanvasLayer? _shelfAreas;
  
  internal Vector3 InitRotation;

  private TurnOrientation _turnOrientation;
  
  public override void _Ready()
  {
    InitRotation = GlobalRotation;

    if (!_shelfAreas.IsValid())
      return;
    
    foreach (Node node in _shelfAreas!.GetChildren())
    {
      if (node is not ShelfTurnArea shelfTurnArea)
        continue;

      shelfTurnArea.RequestingTurn += TryTurn;
    }
  }

  private void TryTurn(TurnOrientation orientation)
  {
    if (orientation == _turnOrientation)
      return;

    _turnOrientation = 1 - _turnOrientation;
  }

  internal void ResetRotation()
    => GlobalRotation = InitRotation;
}
