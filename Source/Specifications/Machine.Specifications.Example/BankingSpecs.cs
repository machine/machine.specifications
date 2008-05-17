using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Machine.Specifications.Example
{
  public static class Will
  {
    public static T WillReturn<T>(this T obj, T ret)
    {
      return obj;
    }

    public static void Throw(object toThrow)
    {
      
    }
  }

  public static class Should
  {
    public static void ShouldHaveBeenCalled(this object obj)
    {
      
    }

    public static void ShouldNotHaveBeenCalled<T>(this object obj)
    {
      
    }

    public static void HaveBeenCalled()
    {
      
    }

    public static void NotHaveBeenCalled()
    {
      
    }
  }

  public static class With
  {
    public static T Any<T>()
    {
      return default(T);
    }

    public static T Matching<T>(Func<T, bool> match)
    {
      return default(T);
    }

    public static T Any<T>(Func<T, bool> match)
    {
      return default(T);
    }
  }
  public class Outside
  {
    static Account fromAccount;
    static Account toAccount;

    public class Transferring_between_from_account_and_to_account
    {
      Context before_each = () =>
      {
        fromAccount = new Account { Balance = 1m };
        toAccount = new Account { Balance = 1m };

        //fromAccount.Transfer(With.Matching<decimal>(x => x > 0m), With.Any<Account>());Will.Throw(new Exception());
      };

      When the_transfer_is_made = () =>
      {
        fromAccount.Transfer(1m, toAccount);
      };

      It should_debit_the_from_account_by_the_amount_transferred = () =>
      {
        fromAccount.Balance.ShouldEqual(0m);
      };

      It credit_the_to_account_by_the_amount_transferred = () =>
      {
        toAccount.Balance.ShouldEqual(2m);
      };
    }
  }

  public abstract class with_from_account_and_to_account
  {
    protected static Account fromAccount;
    protected static Account toAccount;

    Context before_each =()=>
    {
      fromAccount = new Account {Balance = 1m};
      toAccount = new Account {Balance = 1m};
    };
  }

  public class Transferring_between_two_accounts 
    : with_from_account_and_to_account
  {
    When the_transfer_is_made =()=>
    {
      fromAccount.Transfer(1m, toAccount);
    };
     
    It should_debit_the_from_account_by_the_ammount_transferred =()=>
    {
      fromAccount.Balance.ShouldEqual(0m);
    };

    It should_credit_the_to_account_by_the_amount_transferred =()=>
    {
      toAccount.Balance.ShouldEqual(2m);
    };
  }

  public class Transferring_an_amount_greater_than_the_balance_of_the_from_account
    : with_from_account_and_to_account
  {
    When the_transfer_is_made =()=>
    {
      fromAccount.Transfer(2m, toAccount);
    };

    It_should_throw a_System_Exception =x=>
    {
      x.ShouldBeOfType<Exception>();
    };
  }
}