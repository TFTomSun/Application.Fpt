using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Thomas.Apis.Core.Extendable;

namespace Thomas.Apis.Core
{
    /// <summary>
    /// An common entry point for APIs and Services
    /// </summary>
    public static class Api
    {
        /// <summary>
        /// Gets the extensions point for factory methods.
        /// </summary>
        public static IFactory Create { get; } = new FactoryImpl();

        /// <summary>
        /// Gets the extension point for global methods without any state or with process wide state.
        /// </summary>
        public static IGlobal Global { get; } = new GlobalImpl();

        private class FactoryImpl : IFactory
        {
        }

        private class GlobalImpl : IGlobal
        {
        }
    }

}
