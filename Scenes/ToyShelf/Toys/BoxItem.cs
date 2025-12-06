using Godot;
using ShopGame.Static;

namespace ShopGame.UI;

[GlobalClass, Tool]
internal sealed partial class BoxItem : CharacterBody3D
{
  [Export] private Texture2D? _itemTexture;
  [Export] private Sprite3D? _spriteNode;

  private Vector3 _initPos;

  internal bool ReturnToInitPos { private get; set; }

  public override void _Ready()
  {
    _initPos = GlobalPosition;
    
    if (_itemTexture is null || !_spriteNode.IsValid())
      return;

    _spriteNode!.Texture = _itemTexture;
  }

  public override void _PhysicsProcess(double delta)
  {
    if (!ReturnToInitPos)
      return;

    if (GlobalPosition.IsEqualApprox(_initPos))
    {
      ReturnToInitPos = false;
      return;
    }

    GlobalPosition = GlobalPosition.Lerp(to: _initPos, 10f * (float)delta);
  }
}
