using System;
using System.Diagnostics;
using Castle.DynamicProxy;

namespace Rubberduck.Root
{
    public abstract class WindsorInterceptorBase : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try
            {
                BeforeInvoke(invocation);
                invocation.Proceed();
            }
            catch (Exception exception)
            {
                OnError(invocation, exception);
            }
            finally
            {
                AfterInvoke(invocation);
            }
        }

        protected virtual void BeforeInvoke(IInvocation invocation) { }

        protected virtual void AfterInvoke(IInvocation invocation) { }

        protected virtual void OnError(IInvocation invocation, Exception exception)
        {
            Debug.Write(exception);
            throw new InterceptedException(exception);
        }
    }
}