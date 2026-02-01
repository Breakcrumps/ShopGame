using Godot;
using ShopGame.Utils;

namespace ShopGame.Characters.Fight.BirdEnemy;

[GlobalClass]
internal abstract partial class BirdState : State
{
  internal Bird Bird { private protected get; set; } = null!;
}
