using System;
using Machine.Specifications;

namespace Example.DerivedSubject
{
    public class Account
    {
        public decimal Balance { get; set; }

        public void Transfer(decimal amount, Account toAccount)
        {
            if (amount > Balance)
                throw new Exception(String.Format("Cannot transfer ${0}. The available balance is ${1}.", amount, Balance));

            Balance -= amount;
            toAccount.Balance += amount;
        }
    }

    [MySubject(typeof(Account), "Funds transfer")]
    public class when_transferring_between_two_accounts_with_derived_subject : AccountSpecs
    {
        Because of = () =>
            from_account.Transfer(1m, to_account);

        It should_debit_the_from_account_by_the_amount_transferred = () =>
            from_account.Balance.ShouldEqual(0m);

        It should_credit_the_to_account_by_the_amount_transferred = () =>
            to_account.Balance.ShouldEqual(2m);
    }

    [Subject(typeof(Account), "Funds transfer")]
    public class when_transferring_between_two_accounts_with_normal_subject : AccountSpecs
    {
        Because of = () =>
            from_account.Transfer(1m, to_account);

        It should_debit_the_from_account_by_the_amount_transferred = () =>
            from_account.Balance.ShouldEqual(0m);

        It should_credit_the_to_account_by_the_amount_transferred = () =>
            to_account.Balance.ShouldEqual(2m);
    }

    public abstract class AccountSpecs
    {
        protected static Account from_account;
        protected static Account to_account;

        Establish context = () =>
        {
            from_account = new Account {Balance = 1m};
            to_account = new Account {Balance = 1m};
        };
    }
}
