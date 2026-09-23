using System;
using System.Reflection;
using Machine.Specifications.Fakes.Proxy;

namespace Machine.Specifications.Fakes.Specs
{
    public interface IMyInterface
    {
        bool ReadProp { get; }

        bool WriteProp { set; }

        bool ReadWriteProp { get; set; }

        event EventHandler Event;

        bool Method(int arg1, ref int arg2, out int arg3);
    }

    public class MyClass
    {
        public MyClass(int arg1, int arg2)
        {
        }

        public virtual bool ReadProp { get; }

        public virtual bool WriteProp
        {
            set
            {
            }
        }

        public virtual bool ReadWriteProp { get; set; }

        public virtual event EventHandler Event;

        public virtual bool Method(int arg1, ref int arg2, out int arg3)
        {
            arg3 = default;

            return default;
        }
    }

    public delegate bool MyDelegate(int arg1, ref int arg2, out int arg3);

    public class MyInterfaceFake : IMyInterface
    {
        private static readonly MethodInfo MethodHandle = typeof(IMyInterface).GetMethod("Method");
        private static readonly MethodInfo ReadPropGetHandle = typeof(IMyInterface).GetMethod("get_ReadProp");
        private static readonly MethodInfo WritePropSetHandle = typeof(IMyInterface).GetMethod("set_WriteProp");
        private static readonly MethodInfo ReadWritePropGetHandle = typeof(IMyInterface).GetMethod("get_ReadWriteProp");
        private static readonly MethodInfo ReadWritePropSetHandle = typeof(IMyInterface).GetMethod("set_ReadWriteProp");
        private static readonly MethodInfo EventAddHandle = typeof(IMyInterface).GetMethod("add_Event");
        private static readonly MethodInfo EventRemoveHandle = typeof(IMyInterface).GetMethod("remove_Event");

        private readonly IInterceptor interceptor;

        public MyInterfaceFake(IInterceptor interceptor)
        {
            this.interceptor = interceptor;
        }

        public bool ReadProp
        {
            get
            {
                var arguments = new object[0];
                var invocation = new Invocation(ReadPropGetHandle, arguments);

                interceptor.Intercept(invocation);

                return (bool) invocation.ReturnValue;
            }
        }

        public bool WriteProp
        {
            set
            {
                var arguments = new object[] {value};
                var invocation = new Invocation(WritePropSetHandle, arguments);

                interceptor.Intercept(invocation);
            } 
        }

        public bool ReadWriteProp
        {
            get
            {
                var arguments = new object[0];
                var invocation = new Invocation(ReadWritePropGetHandle, arguments);

                interceptor.Intercept(invocation);

                return (bool) invocation.ReturnValue;
            }
            set
            {
                var arguments = new object[] {value};
                var invocation = new Invocation(ReadWritePropSetHandle, arguments);

                interceptor.Intercept(invocation);
            }
        }

        public event EventHandler Event
        {
            add
            {
                var arguments = new object[] {value};
                var invocation = new Invocation(EventAddHandle, arguments);

                interceptor.Intercept(invocation);
            }
            remove
            {
                var arguments = new object[] {value};
                var invocation = new Invocation(EventRemoveHandle, arguments);

                interceptor.Intercept(invocation);
            }
        }

        public bool Method(int arg1, ref int arg2, out int arg3)
        {
            arg3 = default;

            var arguments = new object[] {arg1, arg2, arg3};
            var invocation = new Invocation(MethodHandle, arguments);

            interceptor.Intercept(invocation);

            arg2 = (int) arguments[1];
            arg3 = (int) arguments[2];

            return (bool) invocation.ReturnValue;
        }
    }

    public class MyClassFake : MyClass
    {
        private static readonly MethodInfo MethodHandle = typeof(MyClass).GetMethod("Method");
        private static readonly MethodInfo ReadPropGetHandle = typeof(MyClass).GetMethod("get_ReadProp");
        private static readonly MethodInfo WritePropSetHandle = typeof(MyClass).GetMethod("set_WriteProp");
        private static readonly MethodInfo ReadWritePropGetHandle = typeof(MyClass).GetMethod("get_ReadWriteProp");
        private static readonly MethodInfo ReadWritePropSetHandle = typeof(MyClass).GetMethod("set_ReadWriteProp");
        private static readonly MethodInfo EventAddHandle = typeof(MyClass).GetMethod("add_Event");
        private static readonly MethodInfo EventRemoveHandle = typeof(MyClass).GetMethod("remove_Event");

        private readonly IInterceptor interceptor;

        public MyClassFake(IInterceptor interceptor, int arg1, int arg2)
            : base(arg1, arg2)
        {
            this.interceptor = interceptor;
        }

        public override bool ReadProp
        {
            get
            {
                var arguments = new object[0];
                var invocation = new Invocation(ReadPropGetHandle, arguments);

                interceptor.Intercept(invocation);

                return (bool) invocation.ReturnValue;
            }
        }

        public override bool WriteProp
        {
            set
            {
                var arguments = new object[] {value};
                var invocation = new Invocation(WritePropSetHandle, arguments);

                interceptor.Intercept(invocation);
            } 
        }

        public override bool ReadWriteProp
        {
            get
            {
                var arguments = new object[0];
                var invocation = new Invocation(ReadWritePropGetHandle, arguments);

                interceptor.Intercept(invocation);

                return (bool) invocation.ReturnValue;
            }
            set
            {
                var arguments = new object[] {value};
                var invocation = new Invocation(ReadWritePropSetHandle, arguments);

                interceptor.Intercept(invocation);
            }
        }

        public override event EventHandler Event
        {
            add
            {
                var arguments = new object[] {value};
                var invocation = new Invocation(EventAddHandle, arguments);

                interceptor.Intercept(invocation);
            }
            remove
            {
                var arguments = new object[] {value};
                var invocation = new Invocation(EventRemoveHandle, arguments);

                interceptor.Intercept(invocation);
            }
        }

        public override bool Method(int arg1, ref int arg2, out int arg3)
        {
            arg3 = default;

            var arguments = new object[] {arg1, arg2, arg3};
            var invocation = new Invocation(MethodHandle, arguments);

            interceptor.Intercept(invocation);

            arg2 = (int) arguments[1];
            arg3 = (int) arguments[2];

            return (bool) invocation.ReturnValue;
        }
    }

    public class MyDelegateFake
    {
        private static readonly MethodInfo MethodHandle = typeof(MyDelegate).GetMethod("Invoke");

        private readonly IInterceptor interceptor;

        public MyDelegateFake(IInterceptor interceptor)
        {
            this.interceptor = interceptor;
        }

        public bool Invoke(int arg1, ref int arg2, out int arg3)
        {
            arg3 = default;

            var arguments = new object[] {arg1, arg2, arg3};
            var invocation = new Invocation(MethodHandle, arguments);

            interceptor.Intercept(invocation);

            arg2 = (int) arguments[1];
            arg3 = (int) arguments[2];

            return (bool) invocation.ReturnValue;
        }

        public static MyDelegate Create(IInterceptor interceptor)
        {
            var target = new MyDelegateFake(interceptor);

            return (MyDelegate) Delegate.CreateDelegate(typeof(MyDelegate), target, "Invoke");
        }
    }
}
