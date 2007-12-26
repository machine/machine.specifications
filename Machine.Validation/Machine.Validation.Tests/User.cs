using System;
using NSTM;

namespace Machine.Validation
{
  [BufferWrites]
  public class DumbUser
  {
    private string _firstName;
    private string _lastName;

    public virtual string FirstName
    {
      get { return _firstName; }
      set { _firstName = value; }
    }

    public virtual string LastName
    {
      get { return _lastName; }
      set { _lastName = value; }
    }

    public DumbUser(string firstName, string lastName)
    {
      _firstName = firstName;
      Console.WriteLine("{0}", _firstName);
      _lastName = lastName;
    }

    public DumbUser()
    {
    }
  }
  [NstmTransactional]
  public class User : AbstractValidatable
  {
    private string _firstName;
    private string _lastName;

    public virtual string FirstName
    {
      get { return _firstName; }
      set { _firstName = value; }
    }

    public virtual string LastName
    {
      get { return _lastName; }
      set { _lastName = value; }
    }
  }
  public abstract class AbstractValidatable : IValidatable
  {
    #region Member Data
    private IValidationCallback _validationCallback;
    private bool _isValid;
    #endregion

    #region IValidatable Members
    public bool IsValid
    {
      get { return _isValid; }
      set
      {
        _isValid = value;
        if (value)
        {
          _validationCallback.RollbackChanges();
        }
        else
        {
          _validationCallback.CommitChanges();
        }
      }
    }

    public IValidationCallback ValidationCallback
    {
      get { return _validationCallback; }
      set { _validationCallback = value; }
    }

    public void CommitChanges()
    {
      _validationCallback.CommitChanges();
    }
    #endregion
  }
}