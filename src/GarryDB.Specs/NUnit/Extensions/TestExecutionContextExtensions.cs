using System;
using System.Collections.Generic;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace GarryDB.Specs.NUnit.Extensions
{
    public static class TestExecutionContextExtensions
    {
        private const string DisposablesKey = "disposables";

        public static void DisposeAfterTestRun(this TestExecutionContext testExecutionContext, IDisposable disposable)
        {
            IPropertyBag properties = testExecutionContext.CurrentTest.Properties;

            if (!properties.ContainsKey(DisposablesKey))
            {
                properties[DisposablesKey] = new List<IDisposable>();
            }

            ((IList<IDisposable>)properties[DisposablesKey]).Add(disposable);
        }

        public static void DisposeAll(this TestExecutionContext testExecutionContext)
        {
            IPropertyBag properties = testExecutionContext.CurrentTest.Properties;

            if (!properties.ContainsKey(DisposablesKey))
            {
                return;
            }

            foreach (IDisposable disposable in (IList<IDisposable>)properties[DisposablesKey])
            {
                disposable.Dispose();
            }
        }
    }
}
