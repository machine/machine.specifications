using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public static class Naming
  {
    public static string ReplaceUnderscores(this string name)
    {
      return name.Replace('_', ' ');
    }

    public static string ReplaceSpaces(this string name)
    {
      return name.Replace(' ', '_');
    }
  }
}
