using System;

using FluentAssertions;

using Machine.Specifications;

namespace Example
{
  [Subject(typeof(Account), "Funds transfer")]
  public class when_transferring_between_two_accounts
    : AccountSpecs
  {
    Establish context = () =>
    {
      fromAccount = new Account {Balance = 1m};
      toAccount = new Account {Balance = 1m};
    };

    Because of =
      () => fromAccount.Transfer(1m, toAccount);

    It should_debit_the_from_account_by_the_amount_transferred =
      () => fromAccount.Balance.Should().Be(0m);

    It should_credit_the_to_account_by_the_amount_transferred =
      () => toAccount.Balance.Should().Be(2m);
  }

  [Subject(typeof(Account), "Funds transfer")]
  [Tags("failure")]
  public class when_transferring_an_amount_larger_than_the_balance_of_the_from_account
    : AccountSpecs
  {
    static Exception exception;

    Establish context = () =>
    {
      fromAccount = new Account {Balance = 1m};
      toAccount = new Account {Balance = 1m};
    };

    Because of =
      () => exception = Catch.Exception(() => fromAccount.Transfer(2m, toAccount));

    It should_not_allow_the_transfer =
      () => exception.Should().BeOfType<Exception>();
  }

  public abstract class AccountSpecs
  {
    protected static Account fromAccount;
    protected static Account toAccount;
  }
}