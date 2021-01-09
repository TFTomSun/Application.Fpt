using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Thomas.Apis.Presentation.ViewModels
{
    /// <summary>
    /// A base class for types with property change notification support.
    /// </summary>
    public class NotifyObject : INotifyObject
    {
        public void RaisePropertyChanged(string propertyName, bool resetMember = false)
        {
            if (resetMember)
            {
                this.Values.TryRemove(propertyName, out _);
            }
            this.OnPropertyChanged(propertyName);
        }
        private ConcurrentDictionary<string, object?> Values { get; } = new ConcurrentDictionary<string, object?>();

        /// <summary>
        /// Will be raised when a property's value changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

       

        internal void MergeTo(NotifyObject other, bool overwriteExisting)
        {

            this.Values.ForEach(v =>
                other.Values.AddOrUpdate(v.Key, v.Value, (key, existing) => overwriteExisting  ? v.Value : existing ));
        }

        /// <summary>
        /// Triggers the change notification for the given property. The name of the property will be filled by the compiler, if not specfied explicitly.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets a property value from the value cache.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="getDefault">An expression that returns the default value, which will be invoked, when the property has not yet been initialized.</param>
        /// <param name="propertyName">The name of the property. Will be automatically filled by the compiler.</param>
        /// <returns></returns>
        protected internal T Get<T>(Func<T>? getDefault = null, [CallerMemberName] string? propertyName = null)
        {
            return (T)this.Values.GetOrAdd(propertyName.NullCheck(), pn => getDefault == null ? default : getDefault.Invoke());
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The value to set.</param>
        /// <param name="onChanged">Will be invoked after the value has been set and before the property change notification is raised.</param>
        /// <param name="propertyName">The name of the property.</param>
        protected internal void Set<T>(T value, Action<(T OldValue, T NewValue)>? onChanged = null, [CallerMemberName] string? propertyName = null)
        {
            var notifyRequired = true;
            var oldValue = default(T);
            this.Values.AddOrUpdate(propertyName.NullCheck(), value, (pn, v) =>
            {
                oldValue = (T)v;
#pragma warning disable CS8604 // Possible null reference argument.
                notifyRequired = !EqualityComparer<T>.Default.Equals(oldValue, value);
#pragma warning restore CS8604 // Possible null reference argument.
                return value;
            });
            if (notifyRequired)
            {
                onChanged?.Invoke((oldValue, value));
                this.OnPropertyChanged(propertyName);
            }
        }
    }
}
