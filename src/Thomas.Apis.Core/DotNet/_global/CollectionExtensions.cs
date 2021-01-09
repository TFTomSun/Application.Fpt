using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Thomas.Apis.Core;
using Thomas.Apis.Core.DotNet;
using Thomas.Apis.Core.New;


/// <summary>
/// Provides extension methods for the Observable collection
/// </summary>
// ReSharper disable once CheckNamespace
public static class CollectionExtensions
{
    [New]
    public static IList<object> WrapAsTypedList(this IList list) => new UntypedListWrapper<object>(list);
    
    [New]
    public static IList<T> WrapAsTypedList<T>(this IList list) => new UntypedListWrapper<T>(list);

    [New]
    private class UntypedListWrapper<T> : IList<T>
    {
        private IList Inner { get; }

        public UntypedListWrapper(IList inner)
        {
            Inner = inner;
        }

        public IEnumerator<T> GetEnumerator() => this.Inner.Cast<T>().GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public void Add(T item) => this.Inner.Add(item);

        public void Clear() => this.Inner.Clear();

        public bool Contains(T item) => this.Inner.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => this.Inner.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            this.Inner.Remove(item);
            return true;
        }

        public int Count => this.Inner.Count;
        public bool IsReadOnly => this.Inner.IsReadOnly;
        public int IndexOf(T item) => this.Inner.IndexOf(item);

        public void Insert(int index, T item) => this.Inner.Insert(index, item);

        public void RemoveAt(int index) => this.Inner.RemoveAt(index);

        public T this[int index]
        {
            get => (T)this.Inner[index];
            set => this.Inner[index] = value;
        }
    }



    /// <summary>
    /// A type safe collection change handle container.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionChangedHandler<T>
    {
        /// <summary>
        /// Gets the inner event args.
        /// </summary>
        public NotifyCollectionChangedEventArgs EventArgs
        {
            get;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CollectionChangedHandler{T}"/>
        /// </summary>
        /// <param name="eventArgs">The inner event args.</param>
        public CollectionChangedHandler(NotifyCollectionChangedEventArgs eventArgs)
        {
            this.EventArgs = eventArgs;
        }

        /// <summary>
        /// Handles and reset operation.
        /// </summary>
        /// <param name="handleReset"></param>
        /// <returns></returns>
        public CollectionChangedHandler<T> HandleReset(Action handleReset)
        {
            if (this.EventArgs.Action == NotifyCollectionChangedAction.Reset)
            {
                handleReset();
            }
            return this;
        }

        /// <summary>
        /// Handles an add operation.
        /// </summary>
        /// <param name="handleAdd"></param>
        public CollectionChangedHandler<T> HandleAdd(Action<T[]> handleAdd)
        {
            if (this.EventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                handleAdd(this.EventArgs.NewItems.Cast<T>().ToArray());
            }
            return this;
        }

        /// <summary>
        /// Hanldes an remove operation.
        /// </summary>
        /// <param name="handleRemove"></param>
        /// <returns></returns>
        public CollectionChangedHandler<T> HandleRemove(Action<T[]> handleRemove)
        {
            if (this.EventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
                handleRemove((this.EventArgs.NewItems ?? this.EventArgs.OldItems).Cast<T>().ToArray());
            }
            return this;
        }
    }
    /// <summary>
    /// Creates a binding for the collection changed event.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="collection">The collection on which the extension is invoked.</param>
    /// <param name="onChanged">The change handler.</param>
    /// <returns>The binding, which will be released on dispose.</returns>
    public static IDisposable BindToCollectionChanged<T>(this INotifyCollectionChanged collection,
        Action<CollectionChangedHandler<T>> onChanged)
    {
        return collection.BindToCollectionChangedDynamic(args => onChanged(new CollectionChangedHandler<T>(args)));
    }

    
    /// <summary>
    /// Creates a binding for the collection changed event.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <param name="collection">The collection on which the extension is invoked.</param>
    /// <param name="onChanged">The change handler.</param>
    /// <returns>The binding, which will be released on dispose.</returns>
    public static IDisposable BindToCollectionChanged<T>(this ObservableCollection<T> collection, Action<CollectionChangedHandler<T>> onChanged)
    {
        return collection.BindToCollectionChangedDynamic(args => onChanged(new CollectionChangedHandler<T>(args)));
    }

    /// <summary>
    /// Binds to changes of the collection items and handles also changes on the collection itself, when the collection implements INotifyCollectionChanged.
    /// </summary>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    /// <param name="collection">The collection on which the extension is invoked.</param>
    /// <param name="onItemChanged">The event handler that should be invoked, when an item changes.</param>
    /// <param name="onItemsAdded">Will be invoked when items are added.</param>
    /// <param name="onItemsRemove">Will be invoked when items are removed.</param>
    /// <returns>An disposable that releases the bindings</returns>
    public static IDisposable BindToCollectionItemsChanged<T>(this ICollection<T> collection,
        Action<T, string> onItemChanged, Action<T[]> onItemsAdded = null, Action<T[]> onItemsRemove = null)
        where T : INotifyPropertyChanged
    {
        var bindings = new DisposableContainer();
        PropertyChangedEventHandler onItemPropertyChanged = (s, args) => onItemChanged((T)s, args.PropertyName);

        var notifyCollection = collection as INotifyCollectionChanged;
        if (notifyCollection != null)
        {
            bindings += notifyCollection.BindToCollectionChanged<T>(
                h => h.HandleAdd(a =>
                {
                    a.ForEach(ad => ad.PropertyChanged += onItemPropertyChanged);
                    onItemsAdded?.Invoke(a);
                }).
                    HandleRemove(r =>
                    {
                        r.ForEach(rm => rm.PropertyChanged -= onItemPropertyChanged);
                        onItemsRemove?.Invoke(r);
                    }).
                    HandleReset(() =>
                    {
                        throw Api.Create.Exception(
                            "Reseting with an active item binding is not permitted. Dispose the binding first.");
                    }));
        }
        collection.ForEach(i => i.PropertyChanged += onItemPropertyChanged);
        bindings += () => collection.ForEach(i => i.PropertyChanged -= onItemPropertyChanged);
        return bindings;
    }
    /// <summary>
    /// Creates a binding for the collection changed event.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <param name="collection">The collection on which the extension is invoked.</param>
    /// <param name="onChanged">The change handler.</param>
    /// <returns>The binding, which will be released on dispose.</returns>
    public static IDisposable BindToCollectionChangedDynamic<TCollection>(this TCollection collection, Action<NotifyCollectionChangedEventArgs> onChanged)
        where TCollection : INotifyCollectionChanged
    {
        NotifyCollectionChangedEventHandler onCollectionChanged = (s, args) => onChanged(args);
        collection.CollectionChanged += onCollectionChanged;
        return Api.Create.Disposable(() => collection.CollectionChanged -= onCollectionChanged);
    }
  

  


    /// <summary>
    /// Dispatches the specified action to the UI thread.
    /// </summary>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    /// <param name="collection">The collection on which the extension is invoked.</param>
    /// <param name="action">The action that should be dispatched to the UI thread.</param>
    //public static void Dispatch<T>(
    //    this ISelectableViewCollection<T> collection, Action<ISelectableViewCollection<T>> action)
    //    where T : INotifyPropertyChanged
    //{
    //    //action(collection);
    //    collection.DoDispatch<Object>(action);
    //}

    ////public static TResult Dispatch<T, TResult>(this ISelectableViewCollection<T> collection,
    ////    Func<ISelectableViewCollection<T>, TResult> function) where T : INotifyPropertyChanged
    ////{
    ////    //return function(collection);
    ////    return collection.DoDispatch<TResult>(function);
    ////}

    //internal static void Dispatch<T, TInnerCollection>(
    //    this ViewCollection<T, TInnerCollection> collection, Action<ViewCollection<T, TInnerCollection>> action)
    //    where T : INotifyPropertyChanged
    //    where TInnerCollection : INotifyCollectionChanged, IList<T>
    //{
    //    //action(collection);
    //    collection.DoDispatch<Object>(action);
    //}

    //internal static TResult Dispatch<T, TInnerCollection, TResult>(this ViewCollection<T, TInnerCollection> collection,
    //    Func<ViewCollection<T, TInnerCollection>, TResult> function)
    //    where T : INotifyPropertyChanged
    //    where TInnerCollection : INotifyCollectionChanged, IList<T>
    //{
    //    //return function(collection);
    //    return collection.DoDispatch<TResult>(function);
    //}

    //public static void Dispatch<T>(
    //    this ICollection<T> collection, Action<ICollection<T>> action)
    //{
    //    collection.DoDispatch<Object>(action);
    //}

    //public static TResult Dispatch<T, TResult>(this ICollection<T> collection,
    //    Func<ICollection<T>, TResult> function)
    //{
    //    return collection.DoDispatch<TResult>(function);
    //}

    ///// <summary>
    ///// Dispatches the specified action to the UI thread.
    ///// </summary>
    ///// <typeparam name="T">The type of the collection items.</typeparam>
    ///// <param name="collection">The collection on which the extension is invoked.</param>
    ///// <param name="action">The action that should be dispatched to the UI thread.</param>
    //public static void Dispatch<T>(
    //    this ObservableCollection<T> collection, Action<ObservableCollection<T>> action)
    //{
    //    collection.DoDispatch<Object>(action);
    //}

    ////public static TResult Dispatch<T, TResult>(this ObservableCollection<T> collection,
    ////    Func<ObservableCollection<T>, TResult> function)
    ////{
    ////    return collection.DoDispatch<TResult>(function);
    ////}


    //private static TResult DoDispatch<TResult>(this Object collection, Delegate method)
    //{
    //    var dispatcher = Api.Global.UiDispatcher().Get();
    //    var result = dispatcher.Invoke(method,collection);
    //    //Object result;
    //    //if (dispatcher.IsSuspended())
    //    //{
    //    //    var operation = dispatcher.BeginInvoke(method, collection);

    //    //    Api.Create.Function(() => !dispatcher.IsSuspended())
    //    //        .WaitUntilTrue((1).Minutes(), (250).Milliseconds(), true,
    //    //            "The dispatcher kept suspended for the specified timeout.");
    //    //    var status = operation.Wait((1).Minutes());
    //    //    if (status == DispatcherOperationStatus.Completed)
    //    //    {
    //    //        result = operation.Result;
    //    //    }
    //    //    else
    //    //    {
    //    //        throw Api.Create.Exception("The dispatcher operation did not complete within the specified timeout.");
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    result = dispatcher.Invoke(method, collection);
    //    //}

    //    return result == null ? default(TResult) : (TResult) result;
    //}

    //public static void Update<T>(this ObservableCollection<T> collection, IEnumerable<T> update)
    //{
    //    Application.Current.Dispatcher.Invoke(() =>
    //        {
    //            collection.Except(update).ToArray().ForEach(i => collection.Remove(i));
    //            update.Except(collection).ToArray().ForEach(collection.Add);
    //        });
    //}


  
  
    public static void Move<T>(this IList<T> collection, T element, int newIndex, bool checkForImplementation = true)
    {
        if (checkForImplementation && collection is IMoveEnumerable<T> implementation)
        {
            implementation.Move(element, newIndex);
        }
        else
        {
            collection.Remove(element);
            collection.Insert(newIndex, element);
        }
    }

   

    /// <summary>
    /// Bind enumerable to another enumerable.
    /// </summary>
    /// <typeparam name="TSource">Type of elements in source enumerable.</typeparam>
    /// <typeparam name="TTarget">Type of elements in target enumerable.</typeparam>
    /// <param name="source">Source enumerable.</param>
    /// <param name="bindMode">Bind mode for enumerable.</param>
    /// <param name="target">Target enumerable.</param>
    /// <param name="forwardTransformation">Function to forward transform binding.</param>
    /// <param name="backwardTransformation">Function to backward transform binding.</param>
    /// <param name="forwardTransformationFilter">A filter that determines whether an source item should be transformed to the target collection, when the source collection changes.</param>
    /// <param name="backwardTransformationFilter">A filter that determines whether an target item should be transformed back to the source collection, when the target collection changes.</param>
    /// <param name="performInitialSynchronization"></param>
    /// <param name="equals"></param>
    /// <returns>Disposable of binding.</returns>
    public static IDisposable BindTo<TSource, TTarget>(this IEnumerable<TSource> source, CollectionBindMode bindMode,
        IEnumerable<TTarget> target, Func<TSource, TTarget> forwardTransformation = null,
        Func<TTarget, TSource> backwardTransformation = null, Func<TSource, bool> forwardTransformationFilter = null, Func<TTarget, bool> backwardTransformationFilter = null, bool performInitialSynchronization = true,
        Func<TSource, TTarget, bool> equals = null)
    {
        var releaseBinding = new DisposableContainer();

        var isSourceChanging = false;
        var isTargetChanging = false;
        if (bindMode == CollectionBindMode.SourceToTarget || bindMode == CollectionBindMode.TwoWay)
        {
            var targetAsList = (IList<TTarget>)target;
            releaseBinding += source.BindTo(targetAsList, equals,
                forwardTransformation, 
                forwardTransformationFilter, () => isSourceChanging = true,
                () => isSourceChanging = false,
                () => isTargetChanging, performInitialSynchronization);

        }
        if (bindMode == CollectionBindMode.TargetToSource || bindMode == CollectionBindMode.TwoWay)
        {
            var backwardEquals = equals == null ? default(Func<TTarget, TSource, bool>) : (t, s) => equals(s, t);

            var sourceAsList = (IList<TSource>)source;
            releaseBinding += target.BindTo(sourceAsList, backwardEquals, backwardTransformation,
                backwardTransformationFilter,
                () => isTargetChanging = true, () => isTargetChanging = false,
                () => isSourceChanging, performInitialSynchronization);
        }
        return releaseBinding;
    }

    /// <summary>
    /// Bind enumerable to another enumerable.
    /// </summary>
    /// <typeparam name="TSource">Type of elements in source enumerable.</typeparam>
    /// <typeparam name="TTarget">Type of elements in target enumerable.</typeparam>
    /// <param name="source">Source enumerable.</param>
    /// <param name="target">Target enumerable.</param>
    /// <param name="forwardTransformation">Function to forward transform binding.</param>
    /// <param name="forwardTransformationFilter">A filter that determines whether an source item should be transformed, when the source collection changes.</param>
    /// <param name="onUpdateing">Invoke on updating of binding.</param>
    /// <param name="onUpdated">Invoke on updated of bind.</param>
    /// <param name="isSuppressed">Function to return if is suppressed.</param>
    /// <param name="performInitialSynchronization"></param>
    /// <param name="equals"></param>
    /// <param name="expectUniqueItems"></param>
    /// <returns>Disposable of binding.</returns>
    [New("Updating and updated event raising on initialization")]
    public static IDisposable BindTo<TSource, TTarget>(this IEnumerable<TSource> source,
        IList<TTarget> target, Func<TSource, TTarget, bool> equals = null, Func<TSource, TTarget> forwardTransformation = null,
        Func<TSource, bool> forwardTransformationFilter = null, Action onUpdateing = null, Action onUpdated = null,
        Func<bool> isSuppressed = null, bool performInitialSynchronization = true, bool expectUniqueItems = true)
    {
        if (typeof(TTarget) == typeof(TSource))
        {
            if (equals == null)
            {
                var comparer = EqualityComparer<TSource>.Default;
                equals = (s, t) => comparer.Equals(s, (TSource)(object)t);
            }

            if (forwardTransformation == null)
            {
                forwardTransformation = s => (TTarget)(object)s;
            }
        }
        if (forwardTransformation == null)
        {
            return Api.Create.Disposable(() => { });
        }
        if (equals == null)
        {
            throw new ArgumentException("equals must be specified if source and target type are not equal");
        }

        if (performInitialSynchronization)
        {
            try
            {
                onUpdateing?.Invoke();
                var sourcesToAdd = source.Where(s => !target.Any(t => equals(s, t)));
                target.AddRange(sourcesToAdd.Where(forwardTransformationFilter ?? (s => true))
                    .Select(forwardTransformation));
            }
            finally
            {
                onUpdated?.Invoke();
            }
        }

        if (source is INotifyCollectionChanged sourceAsNotifyChanged)
        {
            void ChangedHandler(object s, NotifyCollectionChangedEventArgs args)
            {
                try
                {
                    onUpdateing?.Invoke();
                    if (isSuppressed == null || !isSuppressed())
                    {
                        OnCollectionChanged(source, target, forwardTransformation, forwardTransformationFilter, args, @equals, expectUniqueItems);
                    }
                }
                finally
                {
                    onUpdated?.Invoke();
                }
            }

            sourceAsNotifyChanged.CollectionChanged += ChangedHandler;

            var releaseBinding = Api.Create.Disposable(() => sourceAsNotifyChanged.CollectionChanged -= ChangedHandler);
            return releaseBinding;
        }
        return Api.Create.Disposable(() => { });
    }

    private static void OnCollectionChanged<TSource, TTarget>(IEnumerable<TSource> source, IList<TTarget> targetList, Func<TSource, TTarget> transformation, Func<TSource, bool> filter, NotifyCollectionChangedEventArgs args, Func<TSource, TTarget, bool> @equals, bool expectUniqueItems)
    {
        var transformationArgs = new CollectionChangedEventArgsTransformation<TSource, TTarget>(
            source, targetList, args, transformation, filter, equals, expectUniqueItems);
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Replace:
                if (transformationArgs.HasAffectedOldItems && transformationArgs.HasAffectedNewItems)
                {
                    targetList.Replace(transformationArgs.SingleOldItem, transformationArgs.SingleNewItem);
                }
                else if (transformationArgs.HasAffectedNewItems)
                {
                    targetList.Insert(transformationArgs.OldStartingIndex, transformationArgs.SingleNewItem);
                }
                else if (transformationArgs.HasAffectedOldItems)
                {
                    targetList.Remove(transformationArgs.SingleOldItem);
                }
                break;
            case NotifyCollectionChangedAction.Move:
                if (transformationArgs.HasAffectedNewItems)
                {
                    targetList.Move(transformationArgs.SingleNewItem, transformationArgs.NewStartingIndex);
                }
                break;
            case NotifyCollectionChangedAction.Add:
                if (transformationArgs.HasAffectedNewItems)
                {
                    var newStartingIndex = transformationArgs.NewStartingIndex;

                    if (newStartingIndex < 0 || newStartingIndex >= targetList.Count)
                        targetList.AddRange(transformationArgs.NewItems);
                    else if (transformationArgs.SingleNewItem != null)
                        targetList.Insert(newStartingIndex, transformationArgs.SingleNewItem);
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (transformationArgs.HasAffectedOldItems)
                {
                    targetList.RemoveRange(transformationArgs.OldItems);
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                targetList.Clear();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private class CollectionChangedEventArgsTransformation<TSource, TTarget>
    {
        private IEnumerable<TSource> SourceSequence { get; }
        private NotifyCollectionChangedEventArgs EventArgs { get; }
        private Func<TSource, TTarget> Transformation { get; }
        private Func<TSource, bool> Filter { get; }
        public bool ExpectUniqueItems { get; }
        private Func<TSource, TTarget, bool> EqualsFunction { get; }
        private IEnumerable<TTarget> TargetSequence { get; }

        public CollectionChangedEventArgsTransformation(IEnumerable<TSource> sourceSequence,
            IEnumerable<TTarget> targetSequence,
            NotifyCollectionChangedEventArgs eventArgs,
            Func<TSource, TTarget> transformation, Func<TSource, bool> filter, Func<TSource, TTarget, bool> equals, bool expectUniqueItems)
        {
            if (transformation == null) throw new ArgumentNullException(nameof(transformation));
            this.SourceSequence = sourceSequence;
            this.TargetSequence = targetSequence;
            this.EventArgs = eventArgs;
            this.Transformation = transformation;
            this.Filter = filter;
            this.ExpectUniqueItems = expectUniqueItems;
            this.EqualsFunction = equals ?? ((s, t) => Object.Equals(s, t));
        }

        private IEnumerable<TTarget> Convert(IEnumerable<TSource> affectedSourceItems, bool expectExistant)
        {
            if (this.Filter != null)
            {
                affectedSourceItems = affectedSourceItems.Where(this.Filter);
            }
            return affectedSourceItems.Select(i => expectExistant
                ? this.ExpectUniqueItems ? this.TargetSequence.Single(v => this.EqualsFunction(i, v)) : this.TargetSequence.First(v => this.EqualsFunction(i, v))
                : this.Transformation(i));
        }

        public IEnumerable<TTarget> NewItems => this.Convert(this.NewSourceItems, false);
        public IEnumerable<TTarget> OldItems => this.Convert(this.OldSourceItems, true);

        private int ConvertIndex(int sourceIndex)
        {
            if (this.Filter == null || sourceIndex == -1)
            {
                return sourceIndex;
            }
            return this.SourceSequence.Take(sourceIndex).Where(this.Filter).Count();
        }

        public int NewStartingIndex => this.ConvertIndex(this.EventArgs.NewStartingIndex);
        public int OldStartingIndex => this.ConvertIndex(this.EventArgs.OldStartingIndex);

        public TTarget SingleNewItem => this.NewItems.SingleOrDefault();

        public TTarget SingleOldItem => this.ExpectUniqueItems ? this.OldItems.SingleOrDefault() : this.OldItems.FirstOrDefault();


        public TTarget SingleItem => (this.NewItems?.Any() ?? false) ? this.SingleNewItem : this.SingleOldItem;
        public bool HasAffectedNewItems => this.Filter == null || (this.EventArgs.NewItems?.Cast<TSource>().Where(this.Filter).Any() ?? false);
        public bool HasAffectedOldItems => this.Filter == null || (this.OldSourceItems.Where(this.Filter).Any());

        private IEnumerable<TSource> NewSourceItems => this.EventArgs.NewItems?.Cast<TSource>() ?? Enumerable.Empty<TSource>();

        private IEnumerable<TSource> OldSourceItems => this.EventArgs.OldItems?.Cast<TSource>() ?? Enumerable.Empty<TSource>();
    }

    /// <summary>
    /// Removes some items from a collection.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <param name="collection">The collection instance on which this extension is invoked.</param>
    /// <param name="items">The items to be removed.</param>
    /// <param name="comparer">The comparer to consider two items as equal.</param>
    public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items, IEqualityComparer<T> comparer = null)
    {
        if (comparer != null)
        {
            //var sourceItems = items.ToArray();
            items = collection.Where(i => items.Any(i2 => comparer.Equals(i, i2))).ToArray();
        }
        items.ForEach(i => collection.Remove(i));
    }

    /// <summary>
    /// Removes some items from a collection.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <param name="collection">The collection instance on which this extension is invoked.</param>
    /// <param name="items">The items to be removed.</param>
    /// <param name="compareFunction">A function that determines whether two items are considered as equal.</param>
    public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> items, Func<T, T, bool> compareFunction)
    {
        collection.RemoveRange(items, Api.Create.EqualityComparer(compareFunction));
    }
    /// <summary>
    /// Gets the index of the specified element in the sequence.
    /// </summary>
    /// <typeparam name="TSequence">Type of sequence.</typeparam>
    /// <typeparam name="TElement">The type of the sequence elements.</typeparam>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    /// <param name="element">The element that should be found.</param>
    /// <param name="throwException">Determines whether an exception should be thrown if the item could not be found.</param>
    /// <param name="compareFunction">[optional] A custom comparer function that is used to determine if two elements are equal.</param>
    /// <returns>The index of the searched item. If throwException is false and  the item could not be found -1 is returned.</returns>
    public static int IndexOf<TSequence, TElement>(this IEnumerable<TSequence> sequence, TElement element,
        Func<TSequence, TElement, bool> compareFunction, bool throwException = true)
    {
        var match = sequence.Select((e, i) => new { Index = i, Equal = compareFunction(e, element) }).FirstOrDefault(x => x.Equal);
        if (match == null)
        {
            if (throwException)
            {
                throw Api.Create.Exception(
                    "The element '{0}' could not be found within the sequence '{1}'",
                    element, sequence.Aggregate());
            }
            return -1;
        }
        return match.Index;
    }

    /// <summary>
    /// Gets the index of the specified element in the sequence.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    /// <param name="element">The element that should be found.</param>
    /// <param name="throwException">Determines whether an exception should be thrown if the item could not be found.</param>
    /// <param name="compareFunction">[optional] A custom comparer function that is used to determine if two elements are equal.</param>
    /// <returns>The index of the searched item. If throwException is false and  the item could not be found -1 is returned.</returns>
    public static int IndexOf<T>(this IEnumerable<T> sequence, T element, bool throwException = true,
        Func<T, T, bool> compareFunction = null)
    {
        return sequence.IndexOf(element,
            compareFunction ?? ((a, b) => EqualityComparer<T>.Default.Equals(a, b)), throwException);
    }
    /// <summary>
    /// Replace an item in a collection.
    /// </summary>
    /// <typeparam name="T">Type of item.</typeparam>
    /// <param name="list">The list to search in for replace.</param>
    /// <param name="oldItem">Item to replace.</param>
    /// <param name="newItem">Item to replace with.</param>
    /// <param name="compareFunction">Check function to find old item.</param>
    public static void Replace<T>(this IList<T> list, T oldItem, T newItem, Func<T, T, bool> compareFunction = null)
    {
        var replaceSequence = list as IReplaceEnumerable<T>;
        if (replaceSequence == null)
        {
            var index = list.IndexOf(oldItem, true, compareFunction);
            list.ReplaceAt(index, newItem);
        }
        else
        {
            replaceSequence.Replace(oldItem, newItem);
        }
    }

    /// <summary>
    /// Replaces the item at the specified index.
    /// </summary>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    /// <param name="list">The list on which the extension is invoked.</param>
    /// <param name="index">The index of the item that should be replaced.</param>
    /// <param name="item">The new item.</param>
    public static void ReplaceAt<T>(this IList<T> list, int index, T item)
    {
        var replaceSequence = list as IReplaceEnumerable<T>;
        if (replaceSequence == null)
        {
            list[index] = item;
        }
        else
        {
            replaceSequence.ReplaceAt(index, item);
        }
    }

    ///// <summary>
    ///// Replace an item in a collection.
    ///// </summary>
    ///// <typeparam name="T">Type of item.</typeparam>
    ///// <param name="collection">Collection to search in for replace.</param>
    ///// <param name="item">Item to move.</param>
    ///// <param name="index">The new index.</param>
    //public static void Move<T>(this IList<T> collection, T item, int index)
    //{
    //    var moveSequence = collection as IMoveEnumerable<T>;
    //    if (moveSequence == null)
    //    {
    //        if (!collection.Remove(item))
    //        {
    //            throw Api.Create.Exception("The item was not found in the collection");
    //        }
    //        collection.Insert(index, item);
    //    }
    //    else
    //    {
    //        moveSequence.Move(item, index);
    //    }
    //}

    /// <summary>
    /// Replace range of times in collection.
    /// </summary>
    /// <typeparam name="T">Type of value in collection.</typeparam>
    /// <param name="collection">Collection to replace in.</param>
    /// <param name="oldItems">Array of old items.</param>
    /// <param name="newItems">Array of items to replace with.</param>
    /// <param name="compareFunction">Compare function to check the range.</param>
    public static void ReplaceRange<T>(this IList<T> collection, T[] oldItems, T[] newItems, Func<T, T, bool> compareFunction = null)
    {
        if (oldItems.Length != newItems.Length)
        {
            throw Api.Create.Exception("The length of the old and new items to replace must be equal.");
        }
        foreach (var index in Enumerable.Range(0, oldItems.Length))
        {
            collection.Replace(oldItems[index], newItems[index], compareFunction);
        }
    }

    /// <summary>
    /// Adds only the given item to the collection, when the collection does not already contain the item.
    /// </summary>
    /// <typeparam name="T">The type of the list items.</typeparam>
    /// <param name="list">The caller of the extension method.</param>
    /// <param name="item">The item that should be added to the list.</param>
    /// <returns>true, if the value has been added, otherwise false.</returns>
    public static bool AddUnique<T>(this ICollection<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Simplifies to add a key and a value to a list of key value pairs.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="instance">The list that invokes the extension method.</param>
    /// <param name="key">The key to add to the list.</param>
    /// <param name="value">The value to add.</param>
    public static void Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> instance, TKey key, TValue value)
    {
        instance.Add(new KeyValuePair<TKey, TValue>(key, value));
    }


    /// <summary>
    /// Resets the collection.
    /// </summary>
    /// <typeparam name="T">The type of the collection elements.</typeparam>
    /// <param name="collection">The collection instance on which this extension is invoked.</param>
    /// <param name="items">The items to be added after the reset.</param>
    public static void Reset<T>(this ICollection<T> collection, IEnumerable<T> items = null)
    {
        if (collection.Count != 0)
        {
            collection.Clear();
        }

        var itemsArray = items?.ToArray();
        if (itemsArray != null && itemsArray.Length > 0)
        {
            collection.AddRange(itemsArray);
        }
    }
    /// <summary>
    /// Adds some elements to a collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection instance on which this extension is invoked.</param>
    /// <param name="items">The items to be added to the collection.</param>
    public static void AddRange<T>(this ICollection<T> collection, params T[] items)
    {
        IEnumerable<T> itemsSequence = items;
        collection.AddRange(itemsSequence);
    }
    /// <summary>
    /// Adds some elements to a collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection instance on which this extension is invoked.</param>
    /// <param name="items">The items to be added to the collection.</param>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        items.ForEach(collection.Add);
    }

    
}

