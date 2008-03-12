using System;
using System.Collections.Generic;

using Castle.Windsor;

namespace Machine.Migrations.Services
{
  public interface IMigratorContainerFactory
  {
    IWindsorContainer CreateAndPopulateContainer(IConfiguration configuration);
  }
}
