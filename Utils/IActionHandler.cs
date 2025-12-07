using Godot;

namespace ShopGame.Utils;

internal interface IActionHandler
{
  StringName Name { get; }

  void HandleAction(string actionName);
}
