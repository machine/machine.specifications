using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FakeItEasy;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.ExtensionSyntax.Full;

using FluentAssertions;

using Machine.Specifications.Sdk;
using NUnit.Framework;

namespace Machine.Specifications.Sdk
{
    [TestFixture]
    public class SpecTypeCheckTest
    {
        ITypeInfo typeInfo;

        [SetUp]
        public void Setup()
        {
            typeInfo = A.Fake<ITypeInfo>();
        }

        [Test]
        public void ValidatesTypeIsSpecification()
        {
            A.CallTo(() => typeInfo.IsAbstract).Returns(false);
            typeInfo.Configure().CallsTo(x => x.IsAbstract).Returns(false);
            typeInfo.Configure().CallsTo(x => x.IsStruct).Returns(false);
            typeInfo.Configure().CallsTo(x => x.GenericParametersCount).Returns(0);
            typeInfo.Configure().CallsTo(x => x.HasBehaviorAttributeName).Returns(false);
            typeInfo.Configure().CallsTo(x => x.ExistsAnySpecifications()).Returns(true);
            typeInfo.Configure().CallsTo(x => x.ExistsAnyBehaviors()).Returns(true);

            var isSpec = SpecTypeCheck.IsSpecClass(typeInfo);

            isSpec.Should().BeTrue();
        }
    }
}