using System;
using System.Collections.Generic;

using Mono.Cecil.Cil;

using NUnit.Framework;

namespace CodeWeaving.Matcher.Services.Inspection.Impl
{
  [TestFixture]
  public class BranchDeciderTests : TestFixture<BranchDecider>
  {
    [Test]
    public void IsDone_NoBranches_IsTrue()
    {
      Assert.IsTrue(_target.IsDone);
    }

    [Test]
    public void GetBranchInstruction_ConditionalBranchFirstTime_IsBranchInstruction()
    {
      Instruction instruction = CreateConditionalBranch();
      Assert.AreEqual(instruction.Operand, _target.ChooseBranchDestination(instruction));
    }

    [Test]
    public void GetBranchInstruction_ConditionalBranchFirstTime_IsNotDone()
    {
      Instruction instruction = CreateConditionalBranch();
      _target.ChooseBranchDestination(instruction);
      Assert.IsFalse(_target.IsDone);
    }

    [Test]
    public void GetBranchInstruction_ConditionalBranchSecondTime_IsNextInstruction()
    {
      Instruction instruction = CreateConditionalBranch();
      _target.ChooseBranchDestination(instruction);
      Assert.AreEqual(instruction.Next, _target.ChooseBranchDestination(instruction));
    }

    [Test]
    public void GetBranchInstruction_ConditionalBranchSecondTime_IsDone()
    {
      Instruction instruction = CreateConditionalBranch();
      _target.ChooseBranchDestination(instruction);
      _target.ChooseBranchDestination(instruction);
      Assert.IsTrue(_target.IsDone);
    }

    [Test]
    public void GetBranchInstruction_SwitchBranchFirstTime_IsFirstLabel()
    {
      Instruction instruction = CreateSwitchBranch();
      Assert.AreEqual(((Instruction[])instruction.Operand)[0], _target.ChooseBranchDestination(instruction));
    }

    [Test]
    public void GetBranchInstruction_SwitchBranchSecondTime_IsSecondLabel()
    {
      Instruction instruction = CreateSwitchBranch();
      _target.ChooseBranchDestination(instruction);
      Assert.AreEqual(((Instruction[])instruction.Operand)[1], _target.ChooseBranchDestination(instruction));
    }

    [Test]
    public void GetBranchInstruction_SwitchBranchThirdTime_IsThirdLabel()
    {
      Instruction instruction = CreateSwitchBranch();
      _target.ChooseBranchDestination(instruction);
      _target.ChooseBranchDestination(instruction);
      Assert.AreEqual(((Instruction[])instruction.Operand)[2], _target.ChooseBranchDestination(instruction));
    }

    [Test]
    public void GetBranchInstruction_SwitchBranchFourthTime_IsNextInstruction()
    {
      Instruction instruction = CreateSwitchBranch();
      _target.ChooseBranchDestination(instruction);
      _target.ChooseBranchDestination(instruction);
      _target.ChooseBranchDestination(instruction);
      Assert.AreEqual(instruction.Next, _target.ChooseBranchDestination(instruction));
    }

    [Test]
    public void GetBranchInstruction_SwitchBranchThirdTime_IsNotDone()
    {
      Instruction instruction = CreateSwitchBranch();
      _target.ChooseBranchDestination(instruction);
      _target.ChooseBranchDestination(instruction);
      _target.ChooseBranchDestination(instruction);
      Assert.IsFalse(_target.IsDone);
    }

    [Test]
    public void GetBranchInstruction_SwitchBranchFourthTime_IsDone()
    {
      Instruction instruction = CreateSwitchBranch();
      _target.ChooseBranchDestination(instruction);
      _target.ChooseBranchDestination(instruction);
      _target.ChooseBranchDestination(instruction);
      _target.ChooseBranchDestination(instruction);
      Assert.IsTrue(_target.IsDone);
    }

    public override BranchDecider Create()
    {
      return new BranchDecider();
    }

    public Instruction CreateConditionalBranch()
    {
      Instruction branch = CilWorker.Create(OpCodes.Nop);
      Instruction instruction = CilWorker.Create(OpCodes.Brtrue, branch);
      instruction.Next = CilWorker.Create(OpCodes.Nop);
      return instruction;
    }

    public Instruction CreateSwitchBranch()
    {
      Instruction branch1 = CilWorker.Create(OpCodes.Nop);
      Instruction branch2 = CilWorker.Create(OpCodes.Nop);
      Instruction branch3 = CilWorker.Create(OpCodes.Nop);
      Instruction instruction = CilWorker.Create(OpCodes.Switch, new Instruction[] { branch1, branch2, branch3 });
      instruction.Next = CilWorker.Create(OpCodes.Nop);
      return instruction;
    }
  }
}