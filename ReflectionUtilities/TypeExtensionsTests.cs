using System;
using System.Reflection;
using Xunit;

namespace ReflectionUtilities
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void Should_ReturnMethodInfo_ByExpression()
        {
            MethodInfo mi = typeof (TestType).GetInfo<TestType>(m => m.PublicOverloadedMethod());
            Assert.NotNull(mi);
            // mi.GetType() != typeof(MethodInfo) because it's RuntimeMethodInfo class which is inherited and internal
            Assert.True(mi.GetType().IsSubclassOf(typeof (MethodInfo)));
        }

        [Fact]
        public void Should_ReturnInfo_ForSpecificParameters_ByExpression()
        {
            MethodInfo mi1 = typeof (TestType).GetInfo<TestType>(m => m.PublicOverloadedMethod(int.MinValue));
            MethodInfo mi2 = typeof (TestType).GetInfo<TestType>(m => m.PublicOverloadedMethod(int.MinValue, string.Empty));
            Assert.NotNull(mi1);
            Assert.NotNull(mi2);
            // should be different metadata elements
            Assert.NotEqual(mi1.MetadataToken, mi2.MetadataToken);
        }

        [Fact]
        public void Should_ReturnInfo_ByName()
        {
            MethodInfo mi = typeof (TestType).GetInfo("PublicOverloadedMethod");
            Assert.NotNull(mi);
        }

        [Fact]
        public void Should_ReturnInfo_ByName_ForGeneric()
        {
            MethodInfo mi = typeof(TestType).GetInfo("PublicGenericMethod");
            Assert.NotNull(mi);
        }

        [Fact]
        public void Should_ReturnInfo_ByName_ForSpecificParameters()
        {
            MethodInfo mi1 = typeof (TestType).GetInfo("PublicOverloadedMethod");
            MethodInfo mi2 = typeof (TestType).GetInfo("PublicOverloadedMethod", int.MinValue, string.Empty);
            Assert.NotNull(mi1);
            Assert.NotNull(mi2);
            // should be different metadata elements
            Assert.NotEqual(mi1.MetadataToken, mi2.MetadataToken);
        }

        [Fact]
        public void Should_ReturnInfo_ByName_ForSpecificParameters_ForGeneric()
        {
            MethodInfo mi1 = typeof(TestType).GetInfo("PublicMultipleGenericMethod", int.MinValue, string.Empty);
            MethodInfo mi2 = typeof(TestType).GetInfo("PublicMultipleGenericMethod", int.MinValue, string.Empty, int.MaxValue);
            Assert.NotNull(mi1);
            Assert.NotNull(mi2);
            // should be different metadata elements
            Assert.NotEqual(mi1.MetadataToken, mi2.MetadataToken);
        }

        [Fact]
        public void Should_ReturnName_ByExpression()
        {
            string name = typeof (TestType).GetName<TestType>(m => m.PublicOverloadedMethod());
            Assert.Equal("PublicOverloadedMethod", name);
        }

        [Fact]
        public void Should_ReturnName_WithParameters_ByExpression()
        {
            string name = typeof (TestType).GetName<TestType>(m => m.PublicOverloadedMethod(int.MinValue, string.Empty));
            Assert.Equal("PublicOverloadedMethod", name);
        }

        [Fact]
        public void Can_Invoke_Method_WithOut_Parameters()
        {
            TestType testObj = new TestType();
            string name = typeof (TestType).GetName<TestType>(m => m.PublicOverloadedMethod());
            testObj.Invoke(name);
            Assert.True(testObj.Flag);
        }

        [Fact]
        public void Can_Invoke_Method_With_Parameters()
        {
            TestType testObj = new TestType();
            string name = typeof (TestType).GetName<TestType>(m => m.PublicOverloadedMethod());
            testObj.Invoke(name, int.MinValue, string.Empty);
            Assert.True(testObj.Flag);
        }

        [Fact]
        public void Can_Invoke_Method_With_Parameters_ForGeneric()
        {
            TestType testObj = new TestType();
            string name = typeof(TestType).GetName<TestType>(m => m.PublicGenericMethod(int.MinValue, string.Empty));
            testObj.Invoke(name, int.MinValue, string.Empty);
            Assert.True(testObj.Flag);
        }

        [Fact]
        public void Can_Invoke_Method_With_Result()
        {
            TestType testObj = new TestType();
            string name = typeof (TestType).GetName<TestType>(m => m.PublicBoolMethod());
            bool result = testObj.Invoke<TestType, bool>(name);
            Assert.True(testObj.Flag); // method is executed
            Assert.True(result); // result is returned
        }

        [Fact]
        public void Can_Invoke_Method_With_Result_ForGeneric()
        {
            TestType testObj = new TestType();
            string name = typeof(TestType).GetName<TestType>(m => m.PublicBoolGenericMethod(int.MinValue));
            bool result = testObj.Invoke<TestType, bool>(name, int.MinValue);
            Assert.True(testObj.Flag); // method is executed
            Assert.True(result); // result is returned
        }

        [Fact]
        public void ThrowsException_WhenMember_IsNot_Method()
        {
            Assert.Throws<ArgumentException>(() => typeof (TestType).GetName<TestType, int>(m => m.PublicProperty));
        }

        [Fact]
        public void ThrowsException_WhenMember_ParametersNotMatch()
        {
            TestType testObj = new TestType();
            string name = typeof (TestType).GetName<TestType>(m => m.PublicOverloadedMethod());
            Assert.Throws<ArgumentException>(() => testObj.Invoke(name, string.Empty)); // there is no method with such parameters
        }
        
        [Fact]
        public void ThrowsException_WhenMember_ParametersNotMatch_ForGeneric()
        {
            TestType testObj = new TestType();
            const string name = "PublicBoolGenericMethod";
            Assert.Throws<ArgumentException>(() => testObj.Invoke(name, string.Empty, int.MinValue)); // there is no method with such parameters
        }

        [Fact]
        public void ThrowsException_WhenMember_Absent()
        {
            TestType testObj = new TestType();
            const string name = "MissingMethod";
            Assert.Throws<ArgumentException>(() => testObj.Invoke(name)); // there is no such method
        }

        public class TestType
        {
            public bool Flag { get; private set; }
            public int PublicProperty { get; set; }

            public void PublicOverloadedMethod()
            {
                this.Flag = true;
            }

            public void PublicOverloadedMethod(int a)
            {
                this.Flag = true;
            }

            public void PublicOverloadedMethod(int a, string b)
            {
                this.Flag = true;
            }

            public bool PublicBoolMethod()
            {
                this.Flag = true;
                return true;
            }

            public void PublicGenericMethod<T1>()
            {
                this.Flag = true;
            }

            public void PublicGenericMethod<T1, T2>(T1 a, T2 b)
            {
                this.Flag = true;
            }

            public bool PublicBoolGenericMethod<T1>(T1 a)
            {
                this.Flag = true;
                return true;
            }

            public void PublicMultipleGenericMethod<T1, T2>(T1 a, T2 b)
            {
                this.Flag = true;
            }

            public void PublicMultipleGenericMethod<T1, T2, T3>(T1 a, T2 b, T3 c)
            {
                this.Flag = true;
            }
        }
    }
}