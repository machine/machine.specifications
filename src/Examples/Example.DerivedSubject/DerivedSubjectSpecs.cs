using System;

using FluentAssertions;

using Machine.Specifications;

namespace Example.DerivedSubject
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

  [MySubject(typeof(Account), "Funds transfer")]
  public class when_transferring_between_two_accounts_with_derived_subject
      : AccountSpecs
  {
      Because of = () =>
        fromAccount.Transfer(1m, toAccount);

      It should_debit_the_from_account_by_the_amount_transferred = () =>
        fromAccount.Balance.Should().Be(0m);

      It should_credit_the_to_account_by_the_amount_transferred = () =>
        toAccount.Balance.Should().Be(2m);
  }

  [Subject(typeof(Account), "Funds transfer")]
  public class when_transferring_between_two_accounts_with_normal_subject
      : AccountSpecs
  {
      Because of = () =>
        fromAccount.Transfer(1m, toAccount);

      It should_debit_the_from_account_by_the_amount_transferred = () =>
        fromAccount.Balance.Should().Be(0m);

      It should_credit_the_to_account_by_the_amount_transferred = () =>
        toAccount.Balance.Should().Be(2m);
  }

  public abstract class AccountSpecs
    {
        protected static Account fromAccount;
        protected static Account toAccount;

        Establish context = () =>
        {
            fromAccount = new Account { Balance = 1m };
            toAccount = new Account { Balance = 1m };
        };
    }
}