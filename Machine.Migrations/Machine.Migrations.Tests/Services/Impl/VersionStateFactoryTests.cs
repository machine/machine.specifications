using System;
using System.Collections.Generic;

using Machine.Core;

using NUnit.Framework;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class VersionStateFactoryTests : StandardFixture<VersionStateFactory>
  {
    private IConfiguration _configuration;
    private ISchemaStateManager _schemaStateManager;

    public override VersionStateFactory Create()
    {
      _configuration = _mocks.DynamicMock<IConfiguration>();
      _schemaStateManager = _mocks.DynamicMock<ISchemaStateManager>();
      return new VersionStateFactory(_configuration, _schemaStateManager);
    }
  }
}