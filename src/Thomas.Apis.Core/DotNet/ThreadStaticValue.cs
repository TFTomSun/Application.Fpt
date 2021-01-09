using System.Collections.Generic;
using System.Threading;

namespace Thomas.Apis.Core.DotNet
{
    /// <summary>
    /// A generic class for defining values that behaves like thread static attribute.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class ThreadStaticValue<T>
    {
        private readonly Dictionary<int, T> instances = new Dictionary<int, T>();

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T Value
        {
            get
            {
                T instance;
                if (!this.instances.TryGetValue(Thread.CurrentThread.ManagedThreadId, out instance))
                {
                    return default(T);
                }

                return instance;
            }
            set
            {
                lock (this.instances)
                {
                    var threadId = Thread.CurrentThread.ManagedThreadId;
                    if (this.instances.ContainsKey(threadId))
                    {
                        this.instances[threadId] = value;
                    }
                    else
                    {
                        this.instances.Add(threadId, value);
                    }
                }
            }
        }

        /// <summary>
        /// Clears all cached thread static values.
        /// </summary>
        public void Clear()
        {
            this.instances.Clear();
        }
        /// <summary>
        /// Get all values from all threads.
        /// </summary>
        public IEnumerable<T> All
        {
            get { return this.instances.Values; }
        }
    }
}