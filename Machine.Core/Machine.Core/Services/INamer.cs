using System;
using System.Collections.Generic;

namespace Machine.Core.Services
{
  public interface INamer
  {
    string ToCamelCase(string source);
    string MakeRandomName();
  }
}
