using System;
using System.Collections.Generic;

namespace Machine.Migrations
{
  public interface IDatabaseMigration
  {
    void Up();
    void Down();
  }
}
