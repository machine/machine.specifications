using System;
using Machine.Specifications.Reporting.Generation;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Specs.Generation
{
    [Subject(typeof(SpecificationTreeListener))]
    public class when_getting_a_tree_from_a_spec_run
    {
        private const string code = @"
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
                throw new Exception(string.Format(""Cannot transfer {0}. The available balance is {1}."", amount, Balance));

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

        static ISpecificationRunner runner;
        static AssemblyPath specAssemblyPath;
        static SpecificationTreeListener listener;
        static CompileContext compiler;

        Establish context = () =>
          {
              listener = new SpecificationTreeListener();
              runner = new AppDomainRunner(listener, RunOptions.Default);
              compiler = new CompileContext(code);
              specAssemblyPath = compiler.Compile();
          };

        Because of =
          () => runner.RunAssemblies(new [] { specAssemblyPath });

        It should_set_the_total_specifications =
          () => listener.Run.TotalSpecifications.ShouldEqual(6);

        It should_set_the_report_generation_date =
          () => DateTime.Now.AddSeconds(-5).ShouldBeLessThanOrEqualTo(listener.Run.Meta.GeneratedAt);

        It should_default_to_no_timestamp =
          () => listener.Run.Meta.ShouldGenerateTimeInfo.ShouldBeFalse();

        It should_default_to_no_link_to_the_summary =
          () => listener.Run.Meta.ShouldGenerateIndexLink.ShouldBeFalse();

        Cleanup cleanup = () =>
            compiler.Dispose();
    }
}
