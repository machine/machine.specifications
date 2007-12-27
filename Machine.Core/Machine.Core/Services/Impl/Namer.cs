using System;
using System.Collections.Generic;
using System.Text;

namespace Machine.Core.Services.Impl
{
  public class Namer : INamer
  {
    public string ToCamelCase(string source)
    {
      StringBuilder sb = new StringBuilder();
      bool upper = true;
      foreach (char c in source)
      {
        if (c == '_')
        {
          upper = true;
          continue;
        }
        sb.Append(upper ? Char.ToUpper(c) : c);
        upper = false;
      }
      return sb.ToString();
    }
  }
}
