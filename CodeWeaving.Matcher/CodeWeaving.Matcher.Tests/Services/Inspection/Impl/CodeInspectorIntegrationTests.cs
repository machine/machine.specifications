using System;

using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace CodeWeaving.Matcher.Services.Inspection.Impl
{
  [TestFixture]
  public class CodeInspectorIntegrationTests
  {
    [Test]
    public void Inspect_AllTypesInOurAssemblies_DoesntFail()
    {
      AssemblyDefinition assembly = AssemblyFactory.GetAssembly(typeof(CodeInspector).Assembly.Location);
      foreach (TypeDefinition type in assembly.MainModule.Types)
      {
        foreach (MethodDefinition method in type.Methods)
        {
          if (method.Body != null)
          {
            CodeInspector codeInspector = new CodeInspector();
            codeInspector.Inspect(method, new NullInspectionVisitor());
          }
        }
      }
    }
  }
  public class NullInspectionVisitor : IInspectionVisitor
  {
    #region IInspectionVisitor Members
    public void OnInstruction(InspectionPass pass, Instruction instruction)
    {
    }
    #endregion
  }
}