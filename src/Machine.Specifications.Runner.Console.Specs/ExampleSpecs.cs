namespace Machine.Specifications.ConsoleRunner.Specs
{
    public class ExampleSpecs : CompiledSpecs
    {
        public const string Code = @"
using System;
using Machine.Specifications;

namespace Example
{
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

    public class TestAssemblyContext : IAssemblyContext
    {
        public static bool OnAssemblyStartRun;
        public static bool OnAssemblyCompleteRun;

        public void OnAssemblyStart()
        {
            OnAssemblyStartRun = true;
        }

        public void OnAssemblyComplete()
        {
            OnAssemblyCompleteRun = true;
        }
    }

    [Subject(typeof(Account), ""Funds transfer"")]
    public class when_transferring_between_two_accounts : AccountSpecs
    {
        Establish context = () =>
        {
            fromAccount = new Account { Balance = 1m };
            toAccount = new Account { Balance = 1m };
        };

        Because of = () =>
            fromAccount.Transfer(1m, toAccount);

        It should_debit_the_from_account_by_the_amount_transferred = () =>
            fromAccount.Balance.ShouldEqual(0m);

        It should_credit_the_to_account_by_the_amount_transferred = () =>
            toAccount.Balance.ShouldEqual(2m);
    }

    [Subject(typeof(Account), ""Funds transfer"")]
    [Tags(""failure"")]
    public class when_transferring_an_amount_larger_than_the_balance_of_the_from_account : AccountSpecs
    {
        static Exception exception;

        Establish context = () =>
        {
            fromAccount = new Account { Balance = 1m };
            toAccount = new Account { Balance = 1m };
        };

        Because of = () =>
            exception = Catch.Exception(() => fromAccount.Transfer(2m, toAccount));

        It should_not_allow_the_transfer = () =>
            exception.ShouldBeOfExactType<Exception>();
    }

    public abstract class AccountSpecs
    {
        protected static Account fromAccount;
        protected static Account toAccount;
    }

    [Subject(""Recent Account Activity Summary page"")]
    public class when_a_customer_first_views_the_account_summary_page
    {
        It should_display_all_account_transactions_for_the_past_thirty_days;
        It should_display_debit_amounts_in_red_text;
        It should_display_deposit_amounts_in_black_text;
    }
}";

        protected static string path;

        Establish context = () =>
            path = compiler.Compile(Code);
    }
}
