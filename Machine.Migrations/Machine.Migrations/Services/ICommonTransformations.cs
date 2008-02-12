using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface ICommonTransformations
  {
    void AddColumn(string table, string column, Type type, short size, bool allowNull, object currentValue);
    IDisposable EnableIdentityInsertion(string table);
    void SetIdentityInsertion(string table, bool enabled);
  }
}
