using System.Collections.Generic;
using System.Text.Json.Serialization;
using ShopGame.Types;

namespace ShopGame.UI.Textbox;

[JsonSourceGenerationOptions(
  IncludeFields = true,
  WriteIndented = true,
  PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified
)]
[JsonSerializable(typeof(Dictionary<string, List<Replica>>))]
internal sealed partial class DialogueContext : JsonSerializerContext;
