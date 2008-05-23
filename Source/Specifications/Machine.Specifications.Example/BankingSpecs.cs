using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Machine.Specifications.Example
{
  public class Transferring_between_accounts
    : with_from_account_and_to_account
  {
    When the_transfer_is_made = () =>
    {
      fromAccount.Transfer(1m, toAccount);
    };

    It should_debit_the_from_account_by_the_amount_transferred = () =>
    {
      fromAccount.Balance.ShouldEqual(0m);
    };


    When a_transfer_is_made_that_is_too_large =()=>
    {
      fromAccount.Transfer(2m, toAccount);
    };

    It_should_throw a_System_Exception =x=>
    {
      x.ShouldBeOfType<Exception>();
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