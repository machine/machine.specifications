using Castle.MonoRail.Framework;

namespace Machine.MonoRail.Extensions.Controllers
{
  [Layout("default"), Rescue("generalerror")]
  public class HomeController : SmartDispatcherController
  {
    public void Index()
    {
    }
  }
}