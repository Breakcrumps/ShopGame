using System.Collections.Generic;

namespace ShopGame.Types;

internal sealed record Replica(string Who = "", string What = "", List<string>? Choices = null);
