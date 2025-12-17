using Godot;

namespace ShopGame.Utils;

[GlobalClass]
internal partial class ColliderArea2D : Area2D
{
  [Export] internal CollisionShape2D? Collider { get; private set; }
}
