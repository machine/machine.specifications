using System;

using FluentAssertions;

using Machine.Specifications;

namespace Example.CustomDelegates
{
  [Subject(typeof(Account), "Funds transfer")]
  public class when_transferring_between_two_accounts
  {
    static Account fromAccount;
    static Account toAccount;

    Given accounts = () =>
    {
      fromAccount = new Account {Balance = 1m};
      toAccount = new Account {Balance = 1m};
    };

    When transfer_is_made =
      () => fromAccount.Transfer(1m, toAccount);

    Then should_debit_the_from_account_by_the_amount_transferred =
      () => fromAccount.Balance.Should().Be(0m);

    Then should_credit_the_to_account_by_the_amount_transferred =
      () => toAccount.Balance.Should().Be(2m);
  }

  [Subject(typeof(Account), "Funds transfer")]
  public class when_transferring_an_amount_larger_than_the_balance_of_the_from_account
  {
    static Account fromAccount;
    static Account toAccount;
    static Exception exception;

    Given accounts = () =>
    {
      fromAccount = new Account {Balance = 1m};
      toAccount = new Account {Balance = 1m};
    };

    Because transfer_is_made =
      () => exception = Catch.Exception(() => fromAccount.Transfer(2m, toAccount));

    Then should_not_allow_the_transfer =
      () => exception.Should().BeOfType<Exception>();
  }
}