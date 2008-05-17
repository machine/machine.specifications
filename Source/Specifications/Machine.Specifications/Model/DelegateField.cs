using System;
using System.Reflection;

namespace Machine.Specifications.Model
{
  public class DelegateField
  {
    public FieldInfo Field { get; private set; }
    public string Name { get; private set; }

    public DelegateField(FieldInfo field)
    {
      Field = field;
      Name = field.Name.ReplaceUnderscores().Trim();
    }

    public void InvokeOn(object instance, params object[] arguments)
    {
      Delegate action = (Delegate)Field.GetValue(instance);

      action.DynamicInvoke(arguments);
    }

    public bool CanInvokeOn(object instance)
    {
      return Field.GetValue(instance) != null;
    }
  }
}