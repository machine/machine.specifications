using System;

namespace Example.CustomDelegates
{
  public class Account
  {
    private decimal _balance;

    public decimal Balance
    {
      get { return _balance; }
      set { _balance = value; }
    }

    public void Transfer(decimal amount, Account toAccount)
    {
      if (amount > _balance)
      {
        throw new Exception(String.Format("Cannot transfer ${0}. The available balance is ${1}.", amount, _balance));
      }

      _balance -= amount;
      toAccount.Balance += amount;
    }
  }
}