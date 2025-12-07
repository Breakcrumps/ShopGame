using Godot;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Scenes.ToyShelf.Toys;

[GlobalClass]
internal sealed partial class BoxItem : CharacterBody3D
{
  [Export] private Sprite3D? _spriteNode;

  internal BoxItemType ItemType;

  private Vector3 _initPos;

  internal bool ReturnToInitPos { private get; set; }

  public override void _Ready()
    => _initPos = new(0f, -1f, 0f);

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

  internal void LoadTexture(BoxItemType boxItemType)
  {
    if (!_spriteNode.IsValid())
      return;
    
    ItemType = boxItemType;
    string itemName = Inventory.BoxItemName(boxItemType);
    _spriteNode!.Texture = ResourceLoader.Load<Texture2D>($"Scenes/ToyShelf/Toys/{itemName}.png");
  }
}
