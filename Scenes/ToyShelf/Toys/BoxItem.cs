using Godot;
using ShopGame.Scenes.ToyShelf.In3D;
using ShopGame.Static;
using ShopGame.Types;

namespace ShopGame.Scenes.ToyShelf.Toys;

[GlobalClass]
internal sealed partial class BoxItem : CharacterBody3D
{
  [Export] private Sprite3D? _spriteNode;

  internal BoxItemType ItemType { get; private set; }
  internal ShelfPosNode? AssociatedPosNode { private get; set; }

  internal Vector3 InitPos { get; set; } = Vector3.Down;

  internal bool ReturnToInitPos { private get; set; }

  private ShelfViewport? _shelfViewport;

  public override void _PhysicsProcess(double delta)
  {
    if (!ReturnToInitPos)
      return;

    if (GlobalPosition.IsEqualApprox(InitPos))
    {
      ReturnToInitPos = false;
      return;
    }

    GlobalPosition = GlobalPosition.Lerp(to: InitPos, 10f * (float)delta);
  }

  internal void Initialise(BoxItemType boxItemType, ShelfViewport shelfViewport)
  {
    if (!_spriteNode.IsValid())
      return;
      
    shelfViewport.AddChild(this);
    GlobalPosition = InitPos;
    
    ItemType = boxItemType;
    string itemName = Inventory.GetBoxItemName(boxItemType);
    _spriteNode!.Texture = ResourceLoader.Load<Texture2D>($"Scenes/ToyShelf/Toys/{itemName}.png");

    _shelfViewport = shelfViewport;
  }

  internal void FreeIfOnShelf()
  {
    if (!_shelfViewport.IsValid())
      return;
    
    if (AssociatedPosNode is null)
      return;

    AssociatedPosNode.HeldItem = null;
    AssociatedPosNode = null;

    _shelfViewport!.BoxItems.Add(this);
    Inventory.BoxItemQuantities[ItemType]++;

    GD.Print(Inventory.BoxItemQuantities[ItemType]);
  }
}
