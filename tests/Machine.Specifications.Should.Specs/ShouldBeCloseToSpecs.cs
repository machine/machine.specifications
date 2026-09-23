using System;

namespace Machine.Specifications.Should.Specs
{
    [Subject(typeof(ShouldExtensionMethods))]
    class when_asserting_that_a_value_should_be_close_to_another_value
    {
        static Exception exception;

        class and_values_are_of_type_double
        {
            static double actual;

            static double expected;

            Establish context = () => expected = 0;

            class and_default_tolerance_is_used
            {
                class and_actual_is_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.00000005;

                    Because of = () =>
                        exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected));

                    It should_not_throw = () =>
                        exception.ShouldBeNull();
                }

                class and_actual_is_not_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.00000015;

                    Because of = () =>
                        exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected));

                    It should_throw = () =>
                        exception.ShouldBeOfExactType<SpecificationException>();
                }
            }

            class and_custom_tolerance_is_used
            {
                static double tolerance;

                Establish context = () => tolerance = 0.01;

                class and_actual_is_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.005;

                    Because of = () =>
                        exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected, tolerance));

                    It should_not_throw = () =>
                        exception.ShouldBeNull();
                }

                class and_actual_is_not_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.015;

                    Because of = () => exception =
                        Catch.Exception(() => actual.ShouldBeCloseTo(expected, tolerance));

                    It should_throw = () =>
                        exception.ShouldBeOfExactType<SpecificationException>();
                }
            }
        }

        class and_values_are_of_type_float
        {
            static float actual;

            static float expected;

            Establish context = () => expected = 0f;

            class and_default_tolerance_is_used
            {
                class and_actual_is_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.0000001f;

                    Because of = () =>
                        exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected));

                    It should_not_throw = () =>
                        exception.ShouldBeNull();
                }

                class and_actual_is_not_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.0000002f;

                    Because of = () =>
                        exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected));

                    It should_throw = () =>
                        exception.ShouldBeOfExactType<SpecificationException>();
                }
            }

            class and_custom_tolerance_is_used
            {
                static float tolerance;

                Establish context = () =>
                    tolerance = 0.01f;

                class and_actual_is_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.005f;

                    Because of = () =>
                        exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected, tolerance));

                    It should_not_throw = () =>
                        exception.ShouldBeNull();
                }

                class and_actual_is_not_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.015f;

                    Because of = () =>
                        exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected, tolerance));

                    It should_throw = () =>
                        exception.ShouldBeOfExactType<SpecificationException>();
                }
            }
        }

        class and_values_are_of_type_decimal
        {
            static decimal actual;

            static decimal expected;

            Establish context = () =>
                expected = 0m;

            class and_default_tolerance_is_used
            {
                class and_actual_is_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.00000005m;

                    Because of = () => exception =
                        Catch.Exception(() => actual.ShouldBeCloseTo(expected));

                    It should_not_throw = () =>
                        exception.ShouldBeNull();
                }

                class and_actual_is_not_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.00000015m;

                    Because of = () =>
                        exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected));

                    It should_throw = () =>
                        exception.ShouldBeOfExactType<SpecificationException>();
                }
            }

            class and_custom_tolerance_is_used
            {
                static decimal tolerance;

                Establish context = () =>
                    tolerance = 0.01m;

                class and_actual_is_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.005m;

                    Because of = () =>
                        exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected, tolerance));

                    It should_not_throw = () =>
                        exception.ShouldBeNull();
                }

                class and_actual_is_not_within_the_tolerance
                {
                    Establish context = () =>
                        actual = 0.015m;

                    Because of = () =>
                        exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected, tolerance));

                    It should_throw = () =>
                        exception.ShouldBeOfExactType<SpecificationException>();
                }
            }
        }

        class and_values_are_of_type_timespan
        {
            static TimeSpan actual;

            static TimeSpan expected;

            static TimeSpan tolerance;

            Establish context = () =>
            {
                expected = new TimeSpan(ticks: 1000);
                tolerance = new TimeSpan(10);
            };

            class and_actual_is_within_the_tolerance
            {
                Establish context = () =>
                    actual = new TimeSpan(1005);

                Because of = () =>
                    exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected, tolerance));

                It should_not_throw = () =>
                    exception.ShouldBeNull();
            }

            class and_actual_is_not_within_the_tolerance
            {
                Establish context = () =>
                    actual = new TimeSpan(1015);

                Because of = () =>
                    exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected, tolerance));

                It should_throw = () =>
                    exception.ShouldBeOfExactType<SpecificationException>();
            }
        }

        class and_values_are_of_type_datetime
        {
            static DateTime actual;

            static DateTime expected;

            static TimeSpan tolerance;

            Establish context = () =>
            {
                expected = new DateTime(ticks: 1000);
                tolerance = new TimeSpan(10);
            };

            class and_actual_is_within_the_tolerance
            {
                Establish context = () =>
                    actual = new DateTime(1005);

                Because of = () =>
                    exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected, tolerance));

                It should_not_throw = () =>
                    exception.ShouldBeNull();
            }

            class and_actual_is_not_within_the_tolerance
            {
                Establish context = () =>
                    actual = new DateTime(1015);

                Because of = () =>
                    exception = Catch.Exception(() => actual.ShouldBeCloseTo(expected, tolerance));

                It should_throw = () =>
                    exception.ShouldBeOfExactType<SpecificationException>();
            }
        }
    }
}
