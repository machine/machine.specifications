using System;
using Machine.Specifications;

namespace Example.CustomDelegates
{
    [Subject(typeof(Account), "Funds transfer")]
    public class when_transferring_between_two_accounts
    {
        static Account from_account;
        static Account to_account;

        Given accounts = () =>
        {
            from_account = new Account {Balance = 1m};
            to_account = new Account {Balance = 1m};
        };

        When transfer_is_made = () =>
            from_account.Transfer(1m, to_account);

        Then should_debit_the_from_account_by_the_amount_transferred = () =>
            from_account.Balance.ShouldEqual(0m);

        Then should_credit_the_to_account_by_the_amount_transferred = () =>
            to_account.Balance.ShouldEqual(2m);
    }

    [Subject(typeof(Account), "Funds transfer")]
    public class when_transferring_an_amount_larger_than_the_balance_of_the_from_account
    {
        static Account from_account;
        static Account to_account;
        static Exception exception;

        Given accounts = () =>
        {
            from_account = new Account {Balance = 1m};
            to_account = new Account {Balance = 1m};
        };

        Because transfer_is_made = () =>
            exception = Catch.Exception(() => from_account.Transfer(2m, to_account));

        Then should_not_allow_the_transfer = () =>
            exception.ShouldBeOfExactType<Exception>();
    }
}
