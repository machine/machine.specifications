using System;
using System.Collections.Generic;
using NHibernate;

namespace Machine.Migrations.NHibernate.Services
{
  public interface INHibernateSessionProvider
  {
    ISession FindCurrentSession();
  }
}
