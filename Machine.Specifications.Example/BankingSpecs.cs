using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Example
{
  public abstract class with_from_account_and_to_account : Context
  {
    protected static Account fromAccount;
    protected static Account toAccount;

    public override void SetupContext()
    {
      fromAccount = new Account { Balance = 1m };
      toAccount = new Account { Balance = 1m };
    }
  }

  [Specification]
  public class when_transfering_between_two_accounts_with_no_context
  {
    private int foo;
    When the_transfer_is_made =()=>
    {
      fromAccount = new Account { Balance = 1m };
      toAccount = new Account { Balance = 1m };
      fromAccount.Transfer(1m, toAccount);
    };
     
    It should_debit_the_from_account_by_the_amount_transferred =()=>
    {
      fromAccount.Balance.ShouldEqual(0m);
    };

    It should_credit_the_to_account_by_the_amount_transferred =()=>
    {
      toAccount.Balance.ShouldEqual(2m);
    };

    protected static Account fromAccount;
    protected static Account toAccount;
  }

  [Specification]
  public class when_transfering_between_two_accounts_has_context
    : IHasContext
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

    protected static Account fromAccount;
    protected static Account toAccount;
    public void SetupContext()
    {
      fromAccount = new Account { Balance = 1m };
      toAccount = new Account { Balance = 1m };
    }
  }

  [Specification]
  public class when_transfering_between_two_accounts 
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

  [Specification]
  public class when_transfering_an_amount_greater_than_the_balance_of_the_from_account
    : with_from_account_and_to_account
  {
    When the_transfer_is_made =()=>
    {
      fromAccount.Transfer(2m, toAccount);
    };

    It should_not_allow_the_transfer =()=>
    {
      exception.ShouldNotBeNull();
    };

    It should_raise_System_Exception =()=>
    {
      exception.ShouldBeOfType(typeof(Exception));
    };
  }
}
