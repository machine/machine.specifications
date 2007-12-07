using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CodeWeaving.Matcher.Services.Impl
{
  public class MemberUsage
  {
    private readonly Instruction _instruction;
    private readonly MethodDefinition _methodDefinition;

    public Instruction Instruction
    {
      get { return _instruction; }
    }

    public MethodDefinition MethodDefinition
    {
      get { return _methodDefinition; }
    }

    public MemberUsage(Instruction instruction, MethodDefinition methodDefinition)
    {
      _instruction = instruction;
      _methodDefinition = methodDefinition;
    }
  }
}