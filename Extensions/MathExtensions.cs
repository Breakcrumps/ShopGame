using Godot;

namespace ShopGame.Extensions;

internal static class MathExtensions
{
  #region Lerp
  internal static void Lerp(this ref float from, float to, float weight)
    => from = Mathf.Lerp(from, to, weight);

  internal static float Lerped(this float from, float to, float weight)
    => Mathf.Lerp(from, to, weight);

  internal static void ExpLerp(this ref float from, float to, float weight)
    => from.Lerp(to, 1 - Mathf.Exp(-weight));

  internal static float ExpLerped(this float from, float to, float weight)
    => from.Lerped(to, 1 - Mathf.Exp(-weight));

  internal static void ExpLerpVec2(this ref Vector2 from, Vector2 to, float weight)
    => from = from.Lerp(to, 1 - Mathf.Exp(-weight));

  internal static Vector2 ExpLerpedVec2(this Vector2 from, Vector2 to, float weight)
    => from.Lerp(to, 1 - Mathf.Exp(-weight));

  internal static void ExpLerpVec3(this ref Vector3 from, Vector3 to, float weight)
    => from = from.Lerp(to, 1 - Mathf.Exp(-weight));

  internal static Vector3 ExpLerpedVec3(this Vector3 from, Vector3 to, float weight)
    => from.Lerp(to, 1 - Mathf.Exp(-weight));
  #endregion

  internal static bool IsEqualApprox(this float a, float b)
    => Mathf.IsEqualApprox(a, b);

  internal static bool IsEqualApprox(this float a, float b, float tolerance)
    => Mathf.IsEqualApprox(a, b, tolerance);

  internal static bool IsZeroApprox(this float a)
    => Mathf.IsZeroApprox(a);

  internal static float MinAxis(this Vector2 vec)
    => vec.X <= vec.Y ? vec.X : vec.Y;

  internal static Vector2I FlooredToInt(this Vector2 vec)
    => new(Mathf.FloorToInt(vec.X), Mathf.FloorToInt(vec.Y));

  internal static Vector2I RoundedToInt(this Vector2 vec)
    => new(Mathf.RoundToInt(vec.X), Mathf.RoundToInt(vec.Y));
}
