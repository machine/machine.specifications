using System;
using System.Collections.Generic;

using Mono.Cecil.Cil;

namespace CodeWeaving.Matcher.Services.Inspection
{
  public interface IInspectionVisitor
  {
    void OnInstruction(InspectionPass pass, Instruction instruction);
  }
}
