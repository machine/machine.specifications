namespace Machine.Specifications.Runner.Utility
{
    public class custom_delegate_specs : running_specs
    {
        const string Code = @"
using System;
using Machine.Specifications;

namespace Example.CustomDelegates
{
    [SetupDelegate]
    public delegate void Given();

    [ActDelegate]
    public delegate void When();

    [AssertDelegate]
    public delegate void Then();

    [Subject(typeof(Account), ""Funds transfer"")]
    public class when_transferring_between_two_accounts
    {
        static Account fromAccount;
        static Account toAccount;

        Given accounts = () =>
        {
            fromAccount = new Account {Balance = 1m};
            toAccount = new Account {Balance = 1m};
        };

        When transfer_is_made = () =>
            fromAccount.Transfer(1m, toAccount);

        Then should_debit_the_from_account_by_the_amount_transferred = () =>
            fromAccount.Balance.ShouldEqual(0m);

        Then should_credit_the_to_account_by_the_amount_transferred = () =>
            toAccount.Balance.ShouldEqual(2m);
    }

    [Subject(typeof(Account), ""Funds transfer"")]
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

        Because transfer_is_made = () =>
            exception = Catch.Exception(() => fromAccount.Transfer(2m, toAccount));

        Then should_not_allow_the_transfer = () =>
            exception.ShouldBeOfExactType<Exception>();
    }

    public class Account
    {
        public decimal Balance { get; set; }

        public void Transfer(decimal amount, Account toAccount)
        {
            if (amount > Balance)
                throw new Exception(String.Format(""Cannot transfer ${0}. The available balance is ${1}."", amount, Balance));

            Balance -= amount;
            toAccount.Balance += amount;
        }
    }
}";

        static CompileContext compiler;

        static AssemblyPath path;

        Establish context = () =>
        {
            compiler = new CompileContext();

            path = compiler.Compile(Code);
        };

        Because of = () =>
            runner.RunAssembly(path);

        Cleanup after = () =>
            compiler.Dispose();
    }

    public class when_running_specs_with_custom_delegates : custom_delegate_specs
    {
        It should_run_them_all = () =>
            listener.SpecCount.ShouldEqual(3);
    }
}
