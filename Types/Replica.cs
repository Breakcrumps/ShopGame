using System;
using System.Collections.Generic;
using System.Text;

namespace ShopGame.Types;

internal sealed class Replica
{
  public string Who { get; init; } = "";
  public string What { private get; init; } = "";
  internal string Line { get; private set; } = "";

  public List<Choice>? Choices;

  internal Dictionary<int, float> Waits = [];

  internal void ComputeLineAndWaits()
  {
    int shift = 0;
    StringBuilder whatBuilder = new();

    for (int i = 0; i < What.Length; i++)
    {
      if (What[i] != '[')
      {
        whatBuilder.Append(What[i]);
        continue;
      }

      if (What[i + 1] == 'w')
        i += ParseWait(startIndex: i, ref shift);
    }

    Line = $"{whatBuilder}";
  }

  private int ParseWait(int startIndex, ref int shift)
  {
    int endIndex = startIndex + 2;
    
    for ( ; What[endIndex] != ']'; endIndex++);
    
    if (endIndex == startIndex + 2)
    {
      Waits.Add(startIndex - shift, -1f);
      shift += 3;
      return 2;
    }

    string numStr = What[(startIndex + 3)..endIndex];

    if (!float.TryParse(numStr, out float waitTime))
      throw new ArgumentException("Couldn't handle time-specified wait!");

    Waits.Add(startIndex - shift, waitTime);
    shift += endIndex - startIndex + 1;
    return endIndex - startIndex;
  }
}
