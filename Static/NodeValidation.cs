using Godot;

namespace ShopGame.Static;

internal static class NodeValidation
{
  internal static bool IsValid(this GodotObject? godotObject)
    => GodotObject.IsInstanceValid(godotObject) && !godotObject.IsQueuedForDeletion();

  internal static T? IfValid<T>(this T? obj) where T : GodotObject
    => obj.IsValid() ? obj : null;
}
