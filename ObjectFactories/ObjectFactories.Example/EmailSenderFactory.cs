using System;
using ObjectFactories.Services;

namespace ObjectFactories.Example
{
  public class EmailSenderFactory : IObjectFactory<EmailSender>
  {
    #region IObjectFactory<EmailSender> Members
    public EmailSender Create()
    {
      Console.WriteLine("Creating EmailSender...");
      return new EmailSender();
    }
    #endregion
  }
}