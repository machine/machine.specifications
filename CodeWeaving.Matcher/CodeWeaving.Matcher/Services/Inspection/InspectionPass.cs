using CodeWeaving.Matcher.Services.Inspection.Impl;

using Mono.Cecil;

namespace CodeWeaving.Matcher.Services.Inspection
{
  public class InspectionPass
  {
    private readonly MethodDefinition _method;
    private readonly BranchDecider _branchDecider;
    private readonly InspectedStack _stack;
    private readonly IInspectionVisitor _inspectionVisitor;

    public MethodDefinition Method
    {
      get { return _method; }
    }

    public BranchDecider BranchDecider
    {
      get { return _branchDecider; }
    }

    public InspectedStack Stack
    {
      get { return _stack; }
    }

    public IInspectionVisitor InspectionVisitor
    {
      get { return _inspectionVisitor; }
    }

    public InspectionPass(MethodDefinition method, BranchDecider branchDecider, InspectedStack stack, IInspectionVisitor inspectionVisitor)
    {
      _method = method;
      _inspectionVisitor = inspectionVisitor;
      _branchDecider = branchDecider;
      _stack = stack;
    }
  }
}