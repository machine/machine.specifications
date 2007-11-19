using System;
using System.Collections.Generic;

namespace ObjectFactories.Example
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      EmailSender emailer1 = new EmailSender();
      emailer1.TellEveryone("Hello, World!");
      emailer1.TellEveryone("Bye, World");

      EmailSender emailer2 = new EmailSender();
      emailer2.TellEveryone("Hello, World!");
    }
  }
}
