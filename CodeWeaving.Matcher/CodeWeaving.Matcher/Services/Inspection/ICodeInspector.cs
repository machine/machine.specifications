using System.Collections.Generic;

using Mono.Cecil;

namespace CodeWeaving.Matcher.Services.Inspection
{
  public interface ICodeInspector
  {
    IEnumerable<InspectionPass> Inspect(MethodDefinition method, IInspectionVisitor visitor);
  }
}