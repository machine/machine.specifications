using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class EmptyOverrides : IOverrideLookup
  {
    #region IOverrideLookup Members
    public object LookupOverride(ServiceEntry serviceEntry)
    {
      return null;
    }
    #endregion
  }
}