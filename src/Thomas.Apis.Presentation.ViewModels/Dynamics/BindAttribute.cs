using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Thomas.Apis.Presentation.ViewModels.Dynamics
{ public static class DyanmicsExtensions
    {
        public static T Merge<T>(this IEnumerable<T> sources)
        where T:NotifyAttribute
        {
            var clone = default(T);
            var comparer = EqualityComparer<T?>.Default;
            var x = sources.ToArray();
            if (x.Length <= 1)
            {
                return x.ElementAtOrDefault(0);
            }
            foreach (var source in x)
            {
                if (comparer.Equals(clone, default))
                {
                    clone = source.Clone();
                }
                else
                {
                    source.MergeTo(clone, true);
                }
            }
            return clone;
        }

        public static void MergeTo<T>(this T source, T target, bool overwrite)
            where T : NotifyAttribute
        {
            source.Notify.MergeTo(target.Notify, overwrite);
        }

        public static T Clone<T>(this T source)
            where T : NotifyAttribute
        {
            var clone = (T)source.CreateNewSelf();
            source.MergeTo(clone, false);
            return clone;
        }
    }
    public abstract class NotifyAttribute : Attribute, INotifyPropertyChanged
    {
        internal Func<NotifyAttribute> CreateNewSelf { get; }
        public NotifyObject Notify { get; } = new NotifyObject();

        protected NotifyAttribute(Func<NotifyAttribute> createNewSelf)
        {
            CreateNewSelf = createNewSelf;
        }
        /// <summary>
        /// Gets a property value from the value cache.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="getDefault">An expression that returns the default value, which will be invoked, when the property has not yet been initialized.</param>
        /// <param name="propertyName">The name of the property. Will be automatically filled by the compiler.</param>
        /// <returns></returns>
        protected T Get<T>(Func<T>? getDefault = null, [CallerMemberName] string? propertyName = null)
        {
            return this.Notify.Get(getDefault, propertyName);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The value to set.</param>
        /// <param name="onChanged">Will be invoked after the value has been set and before the property change notification is raised.</param>
        /// <param name="propertyName">The name of the property.</param>
        protected void Set<T>(T value, Action<(T OldValue, T NewValue)>? onChanged = null, [CallerMemberName] string? propertyName = null)
        {
            this.Notify.Set(value, onChanged, propertyName);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = default!)
        {
            this.Notify.RaisePropertyChanged(propertyName);
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => this.Notify.PropertyChanged += value;
            remove => this.Notify.PropertyChanged -= value;
        }

    }
    public abstract class BindAttribute : NotifyAttribute
    {
        protected BindAttribute(Func<BindAttribute> createNewSelf):base(createNewSelf)
        {
        }

        public bool InheritToChildren
        {
            get => this.Get<bool>();
            set => this.Set(value);
        }
    }
}
