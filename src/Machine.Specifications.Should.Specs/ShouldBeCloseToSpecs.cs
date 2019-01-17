using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Should.Specs
{
  [Subject(typeof(ShouldExtensionMethods))]
  public class when_asserting_that_a_value_should_be_close_to_another_value
  {
    static Exception Exception;

    public class and_values_are_of_type__double__
    {
      static double Actual;
      static double Expected;

      Establish context = () => Expected = 0;

      public class and_default_tolerance_is_used
      {
        public class and_actual_is_within_the_tolerance
        {
          Establish context = () => Actual = 0.00000005;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected));

          It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class and_actual_is_not_within_the_tolerance
        {
          Establish context = () => Actual = 0.00000015;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected));

          It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }
      }

      public class and_custom_tolerance_is_used
      {
        static double Tolerance;

        Establish context = () => Tolerance = 0.01;

        public class and_actual_is_within_the_tolerance
        {
          Establish context = () => Actual = 0.005;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected, Tolerance));

          It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class and_actual_is_not_within_the_tolerance
        {
          Establish context = () => Actual = 0.015;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected, Tolerance));

          It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }
      }
    }

    public class and_values_are_of_type__float__
    {
      static float Actual;
      static float Expected;

      Establish context = () => Expected = 0f;

      public class and_default_tolerance_is_used
      {
        public class and_actual_is_within_the_tolerance
        {
          Establish context = () => Actual = 0.0000001f;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected));

          It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class and_actual_is_not_within_the_tolerance
        {
          Establish context = () => Actual = 0.0000002f;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected));

          It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }
      }

      public class and_custom_tolerance_is_used
      {
        static float Tolerance;

        Establish context = () => Tolerance = 0.01f;

        public class and_actual_is_within_the_tolerance
        {
          Establish context = () => Actual = 0.005f;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected, Tolerance));

          It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class and_actual_is_not_within_the_tolerance
        {
          Establish context = () => Actual = 0.015f;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected, Tolerance));

          It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }
      }
    }

    public class and_values_are_of_type__decimal__
    {
      static decimal Actual;
      static decimal Expected;

      Establish context = () => Expected = 0m;

      public class and_default_tolerance_is_used
      {
        public class and_actual_is_within_the_tolerance
        {
          Establish context = () => Actual = 0.00000005m;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected));

          It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class and_actual_is_not_within_the_tolerance
        {
          Establish context = () => Actual = 0.00000015m;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected));

          It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }
      }

      public class and_custom_tolerance_is_used
      {
        static decimal Tolerance;

        Establish context = () => Tolerance = 0.01m;

        public class and_actual_is_within_the_tolerance
        {
          Establish context = () => Actual = 0.005m;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected, Tolerance));

          It should_not_throw = () => Exception.ShouldBeNull();
        }

        public class and_actual_is_not_within_the_tolerance
        {
          Establish context = () => Actual = 0.015m;

          Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected, Tolerance));

          It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
        }
      }
    }

    public class and_values_are_of_type__TimeSpan__
    {
      static TimeSpan Actual;
      static TimeSpan Expected;
      static TimeSpan Tolerance;

      Establish context = () =>
      {
        Expected = new TimeSpan(ticks: 1000);
        Tolerance = new TimeSpan(10);
      };
      
      public class and_actual_is_within_the_tolerance
      {
        Establish context = () => Actual = new TimeSpan(1005);

        Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected, Tolerance));

        It should_not_throw = () => Exception.ShouldBeNull();
      }

      public class and_actual_is_not_within_the_tolerance
      {
        Establish context = () => Actual = new TimeSpan(1015);

        Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected, Tolerance));

        It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
      }
    }

    public class and_values_are_of_type__DateTime__
    {
      static DateTime Actual;
      static DateTime Expected;
      static TimeSpan Tolerance;

      Establish context = () =>
      {
        Expected = new DateTime(ticks:1000);
        Tolerance = new TimeSpan(10);
      };

      public class and_actual_is_within_the_tolerance
      {
        Establish context = () => Actual = new DateTime(1005);

        Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected, Tolerance));

        It should_not_throw = () => Exception.ShouldBeNull();
      }

      public class and_actual_is_not_within_the_tolerance
      {
        Establish context = () => Actual = new DateTime(1015);

        Because of = () => Exception = Catch.Exception(() => Actual.ShouldBeCloseTo(Expected, Tolerance));

        It should_throw = () => Exception.ShouldBeOfExactType<SpecificationException>();
      }
    }
  }
}
