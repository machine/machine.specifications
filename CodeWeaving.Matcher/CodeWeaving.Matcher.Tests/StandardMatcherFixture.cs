using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cecil.FlowAnalysis;
using Cecil.FlowAnalysis.ActionFlow;
using Cecil.FlowAnalysis.ControlFlow;
using Cecil.FlowAnalysis.Utilities;
using CodeWeaving.Matcher.Matchers;
using CodeWeaving.Matcher.Services.Impl;
using CodeWeaving.Matcher.Services.Inspection;
using CodeWeaving.Matcher.Services.Inspection.Impl;

using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodeWeaving.Matcher
{
  [TestFixture]
  public class StandardMatcherFixture
  {
    protected MockRepository _mocks = new MockRepository();

    [Test]
    public void Blah()
    {
      MemberHasAttributeMatcher matcher = new MemberHasAttributeMatcher("CodeWeaving.Matcher.SampleCode.SuperDuperAttribute");
      AssemblyDefinition assembly = AssemblyFactory.GetAssembly(GetType().Assembly.Location);
      MemberFinder memberFinder = new MemberFinder();
      FindMemberUsages findMemberUsages = new FindMemberUsages();
      foreach (MemberReference member in memberFinder.FindMembers(assembly, matcher))
      {
        PropertyDefinition property = member as PropertyDefinition;
        if (property != null)
        {
          Console.WriteLine("Found: {0}", property);
          foreach (MemberUsage usage in findMemberUsages.FindUsages(assembly, property.GetMethod))
          {
            Console.WriteLine("Usage: {0}", usage.MethodDefinition);
            CodeInspector codeInspector = new CodeInspector();
            codeInspector.Inspect(usage.MethodDefinition, _mocks.DynamicMock<IInspectionVisitor>());

            ControlFlowGraph controlFlowGraph = FlowGraphFactory.CreateControlFlowGraph(usage.MethodDefinition);
            // ActionFlowGraph actionFlowGraph = FlowGraphFactory.CreateActionFlowGraph(controlFlowGraph);
            StringWriter writer = new StringWriter();
            FormatControlFlowGraph(writer, controlFlowGraph);
            Console.WriteLine(writer.ToString());
          }
        }
      }
    }
		public static void FormatControlFlowGraph (TextWriter writer, ControlFlowGraph cfg)
		{
			int id = 1;
			foreach (InstructionBlock block in cfg.Blocks) {
				writer.WriteLine ("block {0}:", id);
				writer.WriteLine ("\tbody:");
				foreach (Instruction instruction in block) {
					writer.Write ("\t\t");
					Formatter.WriteInstruction (writer, instruction);
					writer.WriteLine ();
				}
				InstructionBlock [] successors = block.Successors;
				if (successors.Length > 0) {
					writer.WriteLine ("\tsuccessors:");
					foreach (InstructionBlock successor in successors) {
						writer.WriteLine ("\t\tblock {0}", GetBlockId (cfg, successor));
					}
				}

				++id;
			}
		}
		private static int GetBlockId (ControlFlowGraph cfg, InstructionBlock block)
		{
			return ((IList) cfg.Blocks).IndexOf (block) + 1;
		}
  }
}
