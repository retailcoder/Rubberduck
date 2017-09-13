﻿using System;
using System.Diagnostics;
using System.Reflection;
using Castle.Core.Internal;
using Castle.DynamicProxy;
using NLog;

namespace Rubberduck.Root
{
    /// <summary>
    /// An attribute that makes an intercepted method call log the duration of its execution.
    /// </summary>
    public class WindsorTimedCallInterceptAttribute : Attribute { }

    /// <summary>
    /// An interceptor that logs the duration of an intercepted invocation.
    /// </summary>
    public class WindsorTimedCallLoggerInterceptor : WindsorInterceptorBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private bool _running;

        protected override void BeforeInvoke(IInvocation invocation)
        {
            _running = (invocation.Method.GetCustomAttribute<WindsorTimedCallInterceptAttribute>() != null);
            if(!_running) { return; }

            _stopwatch.Reset();
            _stopwatch.Start();
        }

        protected override void AfterInvoke(IInvocation invocation)
        {
            if (!_running) { return; }

            _stopwatch.Stop();
            _logger.Trace("Intercepted invocation of '{0}.{1}' ran for {2}ms",
                invocation.TargetType.Name, invocation.Method.Name, _stopwatch.ElapsedMilliseconds);
        }
    }
}
