using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Mocks
{
  public class Foo
  {

  }

  public interface IFoo
  {
    bool Query();
    int QueryInt(int i);
    string QueryString(string s);
    void CommandInt(int i);
    void CommandString(string i);
    void Command();
  }
}
