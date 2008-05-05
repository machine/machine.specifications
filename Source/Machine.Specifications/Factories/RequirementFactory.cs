using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Factories
{
  public class RequirementFactory
  {
    private Dictionary<Type, Func<object, FieldInfo, Requirement>> _creationMethods;
    public RequirementFactory()
    {
      _creationMethods = new Dictionary<Type, Func<object, FieldInfo, Requirement>>();

      _creationMethods[typeof(It)] = CreateItRequirement;
      _creationMethods[typeof(It_should_throw)] = CreateItThrowsRequirement;
    }

    public Requirement CreateRequirement(object instance, FieldInfo requirementField)
    {
      return _creationMethods[requirementField.FieldType](instance, requirementField);
    }

    private static Requirement CreateItRequirement(object instance, FieldInfo requirementField)
    {
      It it = (It)requirementField.GetValue(instance);

      var requirement = new ItRequirement(requirementField, it);
      return requirement;
    }

    private static Requirement CreateItThrowsRequirement(object instance, FieldInfo requirementField)
    {
      It_should_throw it = (It_should_throw)requirementField.GetValue(instance);

      var requirement = new ItShouldThrowRequirement(requirementField, it);
      return requirement;
    }
  }
}
