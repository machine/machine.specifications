using System.Collections.Generic;

using Mono.Cecil.Cil;

namespace CodeWeaving.Matcher.Services.Inspection.Impl
{
  public class BranchDecider
  {
    #region Member Data
    private readonly Dictionary<Instruction, BranchInstanceInformation> _branches = new Dictionary<Instruction, BranchInstanceInformation>();
    #endregion

    #region Members
    public Instruction ChooseBranchDestination(Instruction instruction)
    {
      if (!_branches.ContainsKey(instruction))
      {
        _branches[instruction] = new BranchInstanceInformation(instruction);
      }
      return _branches[instruction].GetBranchInstruction();
    }

    public bool IsDone
    {
      get
      {
        foreach (BranchInstanceInformation value in _branches.Values)
        {
          if (!value.IsDone)
          {
            return false;
          }
        }
        return true;
      }
    }
    #endregion
  }

  public class BranchInstanceInformation
  {
    #region Member Data
    private readonly List<Instruction> _branchOptions = new List<Instruction>();
    private int _index;
    #endregion

    #region Properties
    public bool IsDone
    {
      get { return _index >= _branchOptions.Count - 1; }
    }
    #endregion

    #region BranchInstanceInformation()
    public BranchInstanceInformation(Instruction instruction)
    {
      if (instruction.Operand is Instruction[])
      {
        _branchOptions.AddRange((Instruction[])instruction.Operand);
      }
      else
      {
        _branchOptions.Add((Instruction)instruction.Operand);
      }
      _branchOptions.Add(instruction.Next);
      _index = -1;
    }
    #endregion

    #region Methods
    public Instruction GetBranchInstruction()
    {
      _index++;
      return _branchOptions[_index % _branchOptions.Count];
    }
    #endregion
  }
}