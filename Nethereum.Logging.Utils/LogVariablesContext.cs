﻿using System.Threading;

namespace Nethereum.Logging
{
    public partial class LogVariablesContext
    {
        public static readonly ConcurrentVariableContext GlobalVariablesContext = new ConcurrentVariableContext();

        public static readonly ThreadLocal<ConcurrentVariableContext> ThreadLocal =
            new ThreadLocal<ConcurrentVariableContext>(() => new ConcurrentVariableContext());

        public static readonly ThreadLocal<ConcurrentNestedVariablesContext> NestedThreadVariablesContext =
            new ThreadLocal<ConcurrentNestedVariablesContext>(() => new ConcurrentNestedVariablesContext());
    }
}
