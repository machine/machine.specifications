using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Machine.Specifications.Example
{
  [Concern(typeof(Account), "Funds transfer")]
  public class When_transferring_between_two_accounts
    : with_from_account_and_to_account
  {
    Because of = () =>
    {
      fromAccount.Transfer(1m, toAccount);
    };

    It should_debit_the_from_account_by_the_amount_transferred = () =>
    {
      fromAccount.Balance.ShouldEqual(0m);
    };

    It should_credit_the_to_account_by_the_amount_transferred = () =>
    {
      toAccount.Balance.ShouldEqual(2m);
    };
  }


  [Concern(typeof(Account), "Funds transfer")]
  public class When_transferring_an_amount_larger_than_the_balance_of_the_from_account
    : with_from_account_and_to_account
  {
    static Exception exception;
    Because of =()=>
    {
      exception = Catch.Exception(()=>fromAccount.Transfer(2m, toAccount));
    };

    It should_not_allow_the_transfer =()=>
    {
      exception.ShouldBeOfType<Exception>();
    };
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
}