using Godot;

namespace ShopGame.Utils;

[GlobalClass]
internal partial class ColliderArea3D : Area3D
{
  [Export] internal CollisionShape3D Collider { get; private set; } = null!;
}
