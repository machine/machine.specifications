using System;
using System.Collections.Generic;

using NSTM;

using NUnit.Framework;

namespace Machine.Validation
{
  [TestFixture]
  public class ValidationTests
  {
    [Test]
    public void Test1()
    {
      IValidationServices validation = new ValidationServices();
      User user = new User();
      user.FirstName = "Jacob";
      user.LastName = "Lewallen";
      User validatable = validation.Wrap(user);
      user.FirstName = "Andy";
      validatable.IsValid = false;
      Assert.AreEqual("Jacob", user.FirstName);
    }

    [Test]
    public void Test2()
    {
      DumbUser user = new DumbUser("Andy", "Lewallen");
      user.FirstName = "Jacob";
      Console.WriteLine("{0}", user.FirstName);
      user.LastName = "Lewallen";
    }
  }
}
