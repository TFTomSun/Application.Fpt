using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Thomas.Apis.Core;
using Thomas.Apis.Core.DotNet;

/// <summary>
/// Provides extension methods for the <see cref="IEnumerable{T}"/> interface.
/// </summary>
public static class EnumerableExtensions
{
    public static IEnumerable<object> AsObjectEnumerable<T>(this IEnumerable<T> sequence)
    {
        foreach(var element in sequence)
        {
            yield return element;
        }
    }
    /// <summary>
    /// Returns distinct elements from a sequence by the result of the selector.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <typeparam name="TResult">The type of the value that will be compared.</typeparam>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    /// <param name="selector">Provides the value by which the elements should be compared.</param>
    /// <returns>The distinct sequence.</returns>
    public static IEnumerable<T> Distinct<T, TResult>(this IEnumerable<T> sequence, Func<T, TResult> selector)
    {
        var resultComparer = EqualityComparer<TResult>.Default;
        var comparer = Api.Create.EqualityComparer<T>((t1, t2) => resultComparer.Equals(selector(t1), selector(t2)), t => selector(t).GetHashCode());
        return sequence.Distinct(comparer);
    }

    public static bool CountGreaterThan<T>(this IEnumerable<T> sequence, int itemCount)
    {
        return sequence.Take(itemCount + 1).Count() == itemCount+1;
    }
   
    /// <summary>
    /// Filters for not null elements and returns them in a non-nullable sequence.
    /// </summary>
    /// <typeparam name="T">The inner type of the nullable sequence element.</typeparam>
    /// <param name="sequence">The sequence of nullable elements.</param>
    /// <returns>The filtered sequence of not-nullable elements.</returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> sequence)
        where T : struct
    {
        return sequence.Where(x => x != null).Select(x => x.Value);
    }


    /// <summary>
    /// Returns an empty sequence if the sequence is null.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="sequence">The sequence from which the method is invoked.</param>
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> sequence)
    {
        return sequence ?? System.Linq.Enumerable.Empty<T>();
    }

    public static bool ContainsOneOf<T>(this IEnumerable<T> sequence, IEnumerable<T> elementsToCheck)
    {
        var comparer = EqualityComparer<T>.Default;
        return sequence.Any(e => elementsToCheck.Any(c => comparer.Equals(c, e)));
    }
    public static bool ContainsOneOf<T>(this IEnumerable<T> sequence, params T[] elementsToCheck)
    {
        IEnumerable<T> help = elementsToCheck;
        return sequence.ContainsOneOf(help);
    }

    /// <summary>
    /// Creates an <see cref="IEnumerable{T}"/> with one element, the caller of the extension method.
    /// </summary>
    /// <typeparam name="T">The type of the caller.</typeparam>
    /// <param name="to">The instance that invokes the extension method.</param>
    /// <param name="count">Optional. Determines how often the element should be returned by the resulting enumerable. Default = 1</param>
    /// <param name="returnEmptyIfDefault">Returns Enumerable.empty if the caller is default</param>
    /// <returns>An <see cref="IEnumerable{T}"/> with one element.</returns>
    public static IEnumerable<T> Enumerable<T>(this IToContainer< T> to, int count = 1, bool returnEmptyIfDefault = false)
    {
        var element = to.Instance;
        if (!returnEmptyIfDefault || !Equals(element, default(T)))
        {
            for (int counter = 0; counter < count; ++counter)
            {
                yield return element;
            }
        }
    }


    public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> sequence)
    {
        return sequence?.SelectMany(x => x ??  System.Linq.Enumerable.Empty<T>()) ?? System.Linq.Enumerable.Empty<T>(); 
    }

   

    ///// <summary>
    ///// Adds the given items to the concurrent collection.
    ///// </summary>
    ///// <typeparam name="T">The type of the items.</typeparam>
    ///// <param name="concurrentQueue">The collection on which the extension is invoked.</param>
    ///// <param name="items">The items to add.</param>
    //public static void AddRange<T>(this IObservableConcurrentCollection<T> concurrentQueue, IEnumerable<T> items)
    //{
    //    items.ForEach(concurrentQueue.Add);
    //}




    /// <summary>
    /// Creates groups of the specified count per group.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    /// <param name="groupStartCondition">The condition that indicates a group start.</param>
    /// <param name="groupEndCondition">The condtion that indicates a group end.</param>
    /// <returns>The grouped element sequences.</returns>
    public static IEnumerable<IEnumerable<T>> GroupBy<T>(this IEnumerable<T> sequence,
        Func<T, bool> groupStartCondition, Func<T, bool> groupEndCondition = null)
    {
        List<IEnumerable<T>> outerSequence = new List<IEnumerable<T>>();
        using (var enumerator = sequence.GetEnumerator())
        {
            List<T> innerSequence = null;
            while (enumerator.MoveNext())
            {
                if (groupEndCondition?.Invoke(enumerator.Current) == true)
                {
                    innerSequence = null;
                }
                else if (groupStartCondition(enumerator.Current))
                {
                   
                    innerSequence = new List<T>();
                    outerSequence.Add(innerSequence);
                    innerSequence.Add(enumerator.Current);
                }
                else
                {
                    innerSequence?.Add(enumerator.Current);
                }
            }

            
        }

        return outerSequence;
    }



    /// <summary>
    /// Creates groups of the specified count per group.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    /// <param name="countPerGroup">The number of elements per group.</param>
    /// <returns>The grouped element sequences.</returns>
    public static IEnumerable<IGrouping<int, T>> GroupBy<T>(this IEnumerable<T> sequence, int countPerGroup)
    {
       return sequence.Select((e, i) => new {Element = e, Index = i}).GroupBy(at => at.Index/countPerGroup, i=>i.Element);
    }

    //public static IEnumerable<T> Differences<T>(this IEnumerable<T> sequence, IEnumerable<T> second, Func<T,T,bool> compareFunction)
    //{
    //    var comparer = compareFunction == null ? EqualityComparer<T>.Default : Api.Create.Comparer(compareFunction);
    //    sequence.Where()
    //}

  

    /// <summary>
    /// Concatenates two sequences. 
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="items">The second sequence.</param>
    /// <returns>The result sequence.</returns>
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] items)
    {
        if (items == null)
        {
            return source;
        }
        IEnumerable<T> itemSequence = items;
        return source.Concat(itemSequence);
    }
    /// <summary>
    /// Encapsulates the sequence into an observable collection.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    /// <returns>The observable collection.</returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> sequence)
    {
        return sequence as ObservableCollection<T> ?? new ObservableCollection<T>(sequence.ToEmptyWhenNull());
    }

    /// <summary>
    /// Casts the elements of an <see cref="T:System.Collections.IEnumerable"/> to the specified type.
    /// </summary>
    /// 
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains each element of the source sequence cast to the specified type.
    /// </returns>
    /// <param name="source">The <see cref="T:System.Collections.IEnumerable"/> that contains the elements to be cast to type T</param>
    /// <param name="handleNull">Returns an empty sequence when the source is null</param>
    /// <typeparam name="T">The type to cast the elements of <paramref name="source"/> to.</typeparam>
    /// <exception cref="T:System.InvalidCastException">An element in the sequence cannot be cast to type T.</exception>
    public static IEnumerable<T> Cast<T>(this IEnumerable source, bool handleNull)
    {
        return source == null ? System.Linq.Enumerable.Empty<T>() : source.Cast<T>();
    }

    /// <summary>
    /// Throws an exception when the specified sequence is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    public static IEnumerable<T> ToEmptyWhenNull<T>(this IEnumerable<T> sequence)
    {
        if (sequence == null)
        {
            return System.Linq.Enumerable.Empty<T>();
        }

        return sequence;
    }

    /// <summary>
        /// Throws an exception when the specified sequence is null or empty.
        /// </summary>
        /// <typeparam name="T">The type of the sequence elements.</typeparam>
        /// <param name="sequence">The sequence on which the extension is invoked.</param>
        public static IEnumerable<T> NotNullOrEmpty<T>(this IEnumerable<T> sequence)
    {
        if (sequence == null)
        {
            throw Api.Create.Exception("The sequence of type '{0}' was null", typeof (IEnumerable<T>));
        }
        if (!sequence.Any())
        {
            throw Api.Create.Exception("The sequence of type '{0}' was empty.", sequence.GetType());
        }
        return sequence;
    }
    internal static IList<TR> FullOuterGroupJoin<TA, TB, TK, TR>(
        this IEnumerable<TA> a,
        IEnumerable<TB> b,
        Func<TA, TK> selectKeyA,
        Func<TB, TK> selectKeyB,
        Func<IEnumerable<TA>, IEnumerable<TB>, TK, TR> projection,
        IEqualityComparer<TK> cmp = null)
    {
        cmp = cmp ?? EqualityComparer<TK>.Default;
        var alookup = a.ToLookup(selectKeyA, cmp);
        var blookup = b.ToLookup(selectKeyB, cmp);

        var keys = new HashSet<TK>(alookup.Select(p => p.Key), cmp);
        keys.UnionWith(blookup.Select(p => p.Key));

        var join = from key in keys
                   let xa = alookup[key]
                   let xb = blookup[key]
                   select projection(xa, xb, key);

        return join.ToList();
    }

    /// <summary>
    /// Performs a full outer join on the specified sequences.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left sequence.</typeparam>
    /// <typeparam name="TRight">The type of the right sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key that is used to compare the sequence elements.</typeparam>
    /// <typeparam name="TResult">The type of the joined result.</typeparam>
    /// <param name="left">The left sequence.</param>
    /// <param name="right">The right sequence.</param>
    /// <param name="leftKeySelector">The left key selector.</param>
    /// <param name="rightKeySelector">The right key selector.</param>
    /// <param name="projection">The projection that creates a result element from a left and a right sequence element.</param>
    /// <param name="leftDefault">The default value of the left value, when no matching left sequence element could be found for a right sequence element.</param>
    /// <param name="rightDefault">The default value of the right value, when no matching left sequence element could be found for a left sequence element.</param>
    /// <param name="comparer">The key comparer.</param>
    /// <returns>The full outer join sequence.</returns>
    public static IEnumerable<TResult> FullOuterJoin<TLeft, TRight, TKey, TResult>(
        this IEnumerable<TLeft> left,IEnumerable<TRight> right,Func<TLeft, TKey> leftKeySelector,Func<TRight, TKey> rightKeySelector,
        Func<TLeft, TRight, TKey, TResult> projection, TLeft leftDefault = default(TLeft), TRight rightDefault = default(TRight),IEqualityComparer<TKey> comparer = null)
    {
        comparer = comparer ?? EqualityComparer<TKey>.Default;
        var leftLookup = left.ToLookup(leftKeySelector, comparer);
        var rightLookup = right.ToLookup(rightKeySelector, comparer);

        var keys = new HashSet<TKey>(leftLookup.Select(p => p.Key), comparer);
        keys.UnionWith(rightLookup.Select(p => p.Key));

        var join = from key in keys
                   from xa in leftLookup[key].DefaultIfEmpty(leftDefault)
                   from xb in rightLookup[key].DefaultIfEmpty(rightDefault)
                   select projection(xa, xb, key);

        return join;
    }


    ///// <summary>
    ///// Getter for the next to last element.
    ///// </summary>
    ///// <typeparam name="T">The type of  the elements within the specified sequence.</typeparam>
    ///// <param name="collection">A sequence of elements of type <typeparamref name="T"/> on which the extension is invoked.</param>
    ///// <returns>The nextToLast element in the sequence </returns>
    //public static T NextToLastOrDefault<T>(this IEnumerable<T> collection)
    //{
    //    return collection.ElementAtOrDefault(collection.Count() - 2);
    //}

    /// <summary>
    /// Gets the maximum integer value of the sequence or the default value that is specified.
    /// </summary>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    /// <param name="defaultValue">The default value that will be returned if the sequence is empty.</param>
    /// <returns></returns>
    public static int MaxOrDefault(this IEnumerable<int> sequence, int defaultValue = 0)
    {
        var array = sequence.ToArray();
        if (array.Length != 0)
        {
            return array.Max();
        }
        return defaultValue;
    }

    /// <summary>
    /// Aggregates the specified sequence to a string value.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    /// <param name="separator">The separator that is used between the string values.</param>
    /// <param name="removeLastSeparator">Determines whether the last separator should be removed.</param>
    /// <param name="converter">The optional converter that converts on element of the sequence to a string. If not specified, the ToString() method will be used.</param>
    /// <returns>The aggregated string.</returns>
    public static String Aggregate<T>(this IEnumerable<T> sequence, string separator = ", ", bool removeLastSeparator = true, Func<T, String> converter = null)
    {
        var aggregatedValue = sequence.Aggregate(String.Empty, (a, v) => a + (converter == null ? v?.ToString() : converter(v)) + separator);

        if (removeLastSeparator && aggregatedValue.Length >= separator.Length)
        {
            aggregatedValue = aggregatedValue.Substring(0, aggregatedValue.Length - separator.Length);
        }
        return aggregatedValue;
    }

    /// <summary>
    /// Converts the chars sequence to a string.
    /// </summary>
    /// <param name="chars">The char sequence on which the extension is invoked.</param>
    /// <returns>The string value.</returns>
    public static String ToStringValue(this IEnumerable<char> chars)
    {
        return new String(chars.ToArray());
    }

   
  

   
    /// <summary>
	/// Returns a enumeration that contains the values of the source multiple times, depending on the specified count parameter.
	/// </summary>
	/// <typeparam name="T">The type of the enumeration elements.</typeparam>
	/// <param name="enumeration">The enumeration on which the extension method is invoked.</param>
	/// <param name="count">Determines how often the resulting enumeration contains the elements of the source enumeration.</param>
	/// <returns>The resulting enumeration.</returns>
	public static IEnumerable<T> Repeat<T> (this IEnumerable<T> enumeration, int count)
	{
		for (int i = 0; i < count; ++i)
		{
			foreach (var value in enumeration)
			{
				yield return value;
			}
		}
	}



    ///// <summary>
    ///// Creates an <see cref="IEnumerable{T}"/> with one element, the caller of the extension method.
    ///// </summary>
    ///// <param name="element">The instance that invokes the extension method.</param>
    ///// <param name="count">Optional. Determines how often the element should be returned by the resulting enumerable. Default = 1</param>
    ///// <param name="returnEmptyIfDefault">Returns Enumerable.empty if the caller is default</param>
    ///// <returns>An <see cref="IEnumerable{T}"/> with one element.</returns>
    //public static String[] ToStringArray(this object element, int count = 1, bool returnEmptyIfDefault = false)
    //{
    //    return element.ToEnumerable(count, returnEmptyIfDefault)
    //        .Select(e => e.ToString()).ToArray();
    //}

    /// <summary>
    /// Flattens a recursive data structure.
    /// </summary>
    /// <typeparam name="T">The type of the element within the enumeration.</typeparam>
    /// <param name="enumeration">The root enumeration of the recursive data elements that will be flattened into one enumeration.</param>
    /// <param name="childSelector">The lambda expression that returns the child enumeration of one element.</param>
    /// <param name="yieldSiblingsBeforeChildren">Determines whether the siblings of the sequence should be yielded before the children of the elements are yielded.</param>
    /// <param name="maxDepth">Determines the maximum depth of the recursion. Default is full recursion, if '0' is specified, only the root sequence will be returned.</param>
    /// <returns>The flattened enumeration that contains all tree elements beyond the given root enumeration.</returns>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<T> enumeration, Func<T, IEnumerable<T>> childSelector,bool yieldSiblingsBeforeChildren = false, int? maxDepth = null)
	{
		return enumeration.Flatten(yieldSiblingsBeforeChildren, maxDepth, new[] { childSelector });
	}

    /// <summary>
    /// Flattens a recursive data structure.
    /// </summary>
    /// <typeparam name="T">The type of the element within the enumeration.</typeparam>
    /// <param name="enumeration">The root enumeration of the recursive data elements that will be flattened into one enumeration.</param>
    /// <param name="childSelectors">The lambda expressions that returns the child enumerations of one element.</param>
    /// <param name="yieldSiblingsBeforeChildren">Determines whether the siblings of the sequence should be yielded before the children of the elements are yielded.</param>
    /// <param name="maxDepth">Determines the maximum depth of the recursion. If 0 is specified, only the original sequence will be returned.</param>
    /// <returns>The flattened enumeration that contains all tree elements beyond the given root enumeration.</returns>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<T> enumeration, bool yieldSiblingsBeforeChildren = false, int? maxDepth = null,  params Func<T, IEnumerable<T>>[] childSelectors)
	{

	  return  enumeration.DoFlatten(yieldSiblingsBeforeChildren, maxDepth, 0, childSelectors);
	}

  
    private static IEnumerable<T> DoFlatten<T>(this IEnumerable<T> enumeration, bool yieldSiblingsBeforeChildren = false,
        int? maxDepth = null, int currentDepth = 0, params Func<T, IEnumerable<T>>[] childSelectors)
    {
     
        var yieldChildren = maxDepth == null || currentDepth < maxDepth.Value;

        var items = yieldSiblingsBeforeChildren ? new List<T>(0) : null;

        foreach (var item in enumeration)
        {
            yield return item;

            if (yieldSiblingsBeforeChildren)
            {
                items.Add(item);
            }
            else
            {
                if (yieldChildren)
                {
                    foreach (var childSelector in childSelectors)
                    {
                        var children = childSelector?.Invoke(item);
                        if (children != null)
                        {
                            foreach (var child in children.DoFlatten(false, maxDepth, ++currentDepth, childSelectors))
                            {
                                yield return child;
                            }
                        }
                    }
                }
            }
        }

        if (yieldSiblingsBeforeChildren && yieldChildren)
        {
            foreach (var item in items)
            {
                foreach (var childSelector in childSelectors)
                {
                    var children = childSelector?.Invoke(item);
                    if (children != null)
                    {
                        foreach (var child in children.DoFlatten(true, maxDepth, ++currentDepth, childSelectors))
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        //return itemList;
    }




    /// <summary>
    /// Orders the elements of the enumerable by its frequency. e.g. 1,2,2,2,3,3 would return an enumerable with 2,2,2,3,3,1
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="enumerable">The instance that invokes the extension method.</param>
    /// <returns>The ordered enumerable.</returns>
    public static IEnumerable<T> OrderByFrequency<T>(this IEnumerable<T> enumerable)
	{
		return enumerable.GroupBy(e => e).OrderByDescending(g => g.Count()).SelectMany(g => g);
	}

   

    /// <summary>
    /// Skips a certain number of elements from the given sequence, from the beginning and from the end.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable elements</typeparam>
    /// <param name="sequence">The enumerable that invokes the extension method</param>
    /// <param name="atBeginning">Number of items to be skipped at the beginning of the sequence.</param>
    /// <param name="atEnd">Number of the items to be skipped at the end of the sequence.</param>
    /// <returns>The original sequence minus the skipped elements from the beginning and the end.</returns>
    public static IEnumerable<T> Skip<T>(this IEnumerable<T> sequence, int atBeginning, int atEnd)
    {
        var array = sequence.ToArray();
        return array.Skip(atBeginning).Take(array.Length - atBeginning - atEnd);
    }

    /// <summary>
    /// Executes the given action for each element in the enumerable when the enumerable is iterated..
    /// </summary>
    /// <typeparam name="T">The type of the elments in the enumerable.</typeparam>
    /// <param name="enumerable">The instance that invokes the extension method.</param>
    /// <param name="action">The action that should be executed for each element.</param>
    /// <returns>The yielded elements of the enumerable.</returns>
    public static IEnumerable<T> ForEachLazy<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var element in enumerable)
        {
            action(element);
            yield return element; // yield  the elements so  that the state machine is not broken, when this foreach is used. (e.g. for subsequent Find() calls)
        }
    }
    /// <summary>
    /// Executes the given action for each element in the enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="sequence">The instance that invokes the extension method.</param>
    /// <param name="action">The action that should be executed for each element.</param>
    public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
    {
        sequence.ForEach(action, false);
    }

    /// <summary>
    /// Executes the given action for each element in the enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="sequence">The instance that invokes the extension method.</param>
    /// <param name="action">The action that should be executed for each element.</param>
    /// <param name="aggregateExceptions">Determines whether exceptions that occur during the iteration should be aggregated an thrown at the end.</param>
    public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action, bool aggregateExceptions)
    {
        sequence.ForEach((e, i) => action(e), aggregateExceptions, null);
    }

    /// <summary>
    /// Executes the given action for each element in the enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="sequence">The instance that invokes the extension method.</param>
    /// <param name="action">The action that should be executed for each element.</param>
    /// <param name="aggregateExceptions">Determines whether exceptions that occur during the iteration should be aggregated an thrown at the end.</param>
    /// <param name="additionalErrorMessage">An additional error message it the for each throw an exception.</param>
    public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action, bool aggregateExceptions, String additionalErrorMessage)
    {
        sequence.ForEach((e, i) => action(e), aggregateExceptions, additionalErrorMessage);
    }


    /// <summary>
    /// Executes the given action for each element in the enumerable.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="sequence">The instance that invokes the extension method.</param>
    /// <param name="action">The action that should be executed for each element.</param>
    /// <param name="aggregateExceptions">Determines whether exceptions that occur during the iteration should be aggregated an thrown at the end.</param>
    /// <param name="additionalErrorMessage">An additional error message it the for each throw an exception.</param>
    public static void ForEach<T>(this IEnumerable<T> sequence, Action<T, int> action, bool aggregateExceptions = false, String additionalErrorMessage = null)
    {
        int counter = 0;
        if (aggregateExceptions)
        {
            var exceptions = new List<Exception>();

            foreach (var item in sequence)
            {
                try
                {
                    action(item, counter);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    //try
                    //{
                    //    throw Api.Create.Exception(ex,
                    //        "An error occurred in a aggregate exception loop for the item '{0}' at index '{1}'",
                    //        item, counter);
                    //}
                    //catch (Exception ex2)
                    //{
                    //    exceptions.Add(ex2);
                    //}
                }
                finally
                {
                    ++counter;
                }
            }
            if (exceptions.Count > 0)
            {
                throw Api.Create.AggregateException(exceptions,
                    "{0}.{1} errors occurred during the aggregation loop. The messages are:{2}",
                    additionalErrorMessage, exceptions.Count,exceptions.Select(ex=>ex.Message).Aggregate(Environment.NewLine).SurroundWith(Environment.NewLine));
            }
        }
        else
        {
            foreach (var element in sequence)
            {
                action(element, counter);
                ++counter;
            }
        }
    }

    /// <summary>
    /// Determines whether all elements of a sequence satisfy a condition.
    /// </summary>
    /// <typeparam name="T">Type of the values in the sequence.</typeparam>
    /// <param name="sequence">Sequence of elements.</param>
    /// <param name="action">Function to check if a conditon with counter.</param>
    /// <returns>Returns true if all elements satisfy a condition.</returns>
    public static bool All<T>(this IEnumerable<T> sequence, Func<T, int,bool> action)
    {
        var counter = 0;
        return sequence.All(v =>
        {
            var conditionResult = action(v, counter);
            ++counter;
            return conditionResult;
        });
    }

    /// <summary>
    /// Excludes the specified value from the enumeration.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration values.</typeparam>
    /// <param name="enumeration">The enumeration on which the extension method is invoked.</param>
    /// <param name="values">The values to exclude.</param>
    /// <returns>An enumeration without the specified value.</returns>
    public static IEnumerable<T> Except<T>(this IEnumerable<T> enumeration, params T[] values)
    {
        IEnumerable<T> valuesEnumerable = values;
        return enumeration.Except(valuesEnumerable);
    }

    /// <summary>
    /// Return a sequence of elements which include a set of values.
    /// </summary>
    /// <param name="enumeration">Sequence of elements.</param>
    /// <param name="values">Values to determinate.</param>
    /// <typeparam name="T">Type of values in sequence.</typeparam>
    /// <returns>Sequence of included values.</returns>
    public static IEnumerable<T> Include<T>(this IEnumerable<T> enumeration, params T[] values)
    {
        IEnumerable<T> valuesEnumerable = values;
        return enumeration.Where(e => valuesEnumerable.Contains(e));
    }

	/// <summary>
	/// Searches for the given sequence within the given enumerable and returns the start indices of the found positions.
	/// </summary>
	/// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
	/// <param name="enumerable">The instance that invokes the extension method.</param>
	/// <param name="sequence">The sequence of values that should be found </param>
	/// <returns>The start indices of the found positions.</returns>
	public static IEnumerable<int> FindSequence<T>(this IEnumerable<T> enumerable, IEnumerable<T> sequence)
	{
		var sequenceArray = sequence.ToArray();
		var firstSequenceElement = sequenceArray[0];
		var matches = new List<Match<T>>();

		var index = 0;
		foreach (var element in enumerable)
		{
			if (element.Equals(firstSequenceElement))
			{
				matches.Add(new Match<T>(sequenceArray, index));
			}
			for (int matchIndex = matches.Count - 1; matchIndex >= 0; --matchIndex)
			{
				var match = matches[matchIndex];
				if (!match.Update(element))
				{
					matches.RemoveAt(matchIndex);
				}
			}


			++index;
		}

		return matches.Where(m=>m.IsComplete).Select(m => m.FirstValueIndex);
	}

	private class Match<T>
	{
		private readonly IEnumerable<T> m_sequence;
		private readonly int m_matchStartIndex;
		private int m_currentIndex;
		private bool m_isComplete;

		public Match(IEnumerable<T> sequence, int firstValueIndex)
		{
			m_sequence = sequence;
			m_matchStartIndex = firstValueIndex;
			m_currentIndex = 0;

		}

		public int FirstValueIndex
		{
			get { return this.m_matchStartIndex; }
		}

		public bool IsComplete
		{
			get { return m_isComplete; }
		}

		public bool Update(T value)
		{
			if (this.IsComplete) return true;


			if (m_currentIndex == this.m_sequence.Count() - 1)
			{
				this.m_isComplete = true;
			}
			bool result = this.m_sequence.ElementAt(m_currentIndex).Equals(value);
			++m_currentIndex;
			return result;
		}

      
	}

    /// <summary>
    /// Extension of the Linq Last method to add an exception message
    /// </summary>
    /// <typeparam name="T">The type of the enumerable elements.</typeparam>
    /// <param name="sequence">The enumerable instance on which this extension is invoked.</param>
    /// <param name="exceptionMessage">The exception message in case of failure</param>
    /// <param name="parameters">The exception parameters in case of failure.</param>
    /// <returns>The element that satisfies the condition</returns>
    public static T Last<T>(this IEnumerable<T> sequence, string exceptionMessage, params object[] parameters)
    {
        return sequence.WrapException(s => s.Last(), exceptionMessage, parameters);
    }

    /// <summary>
    /// Extension of the Linq Lis method to add an exception message and using a function selector.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable elements.</typeparam>
    /// <param name="sequence">The enumerable instance on which this extension is invoked.</param>
    /// <param name="exceptionMessage">The exception message in case of failure</param>
    /// <param name="parameters">The exception parameters in case of failure.</param>
    /// <param name="selector">The selection function.</param>
    /// <returns>The element that satisfies the condition.</returns>
    public static T Last<T>(this IEnumerable<T> sequence, Func<T, bool> selector, string exceptionMessage,
                              params object[] parameters)
    {
        return sequence.WrapException(s => s.Last(selector), exceptionMessage, parameters);
    }

    /// <summary>
    /// Extension of the Linq single method to add an exception message
    /// </summary>
    /// <typeparam name="T">The type of the enumerable elements.</typeparam>
    /// <param name="sequence">The enumerable instance on which this extension is invoked.</param>
    /// <param name="exceptionMessage">The exception message in case of failure</param>
    /// <param name="parameters">The exception parameters in case of failure.</param>
    /// <returns>The element that satisfies the condition</returns>
    public static T Single<T>(this IEnumerable<T> sequence, string exceptionMessage, params object[] parameters)
    {
        return sequence.WrapException(s => s.Single(), exceptionMessage, parameters);
    }

    /// <summary>
    /// Extension of the Linq single method to add an exception message and using a function selector.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable elements.</typeparam>
    /// <param name="sequence">The enumerable instance on which this extension is invoked.</param>
    /// <param name="exceptionMessage">The exception message in case of failure</param>
    /// <param name="parameters">The exception parameters in case of failure.</param>
    /// <param name="selector">The selection function.</param>
    /// <returns>The element that satisfies the condition.</returns>
    public static T Single<T>(this IEnumerable<T> sequence, Func<T, bool> selector, string exceptionMessage,
                              params object[] parameters)
    {
        return sequence.WrapException(s=>s.Single(selector), exceptionMessage, parameters);
    }

    /// <summary>
    /// Extension of the Linq SingleOrDefault method to add an exception message
    /// </summary>
    /// <typeparam name="T">The type of the enumerable elements.</typeparam>
    /// <param name="sequence">The enumerable instance on which this extension is invoked.</param>
    /// <param name="exceptionMessage">The exception message in case of failure</param>
    /// <param name="parameters">The exception parameters in case of failure.</param>
    /// <returns>The element that satisfies the condition</returns>
    public static T SingleOrDefault<T>(this IEnumerable<T> sequence, string exceptionMessage, params object[] parameters)
    {
        return sequence.WrapException(s => s.SingleOrDefault(), exceptionMessage, parameters);
    }

    /// <summary>
    /// Extension of the Linq SingleOrDefault method to add an exception message and using a function selector.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable elements.</typeparam>
    /// <param name="sequence">The enumerable instance on which this extension is invoked.</param>
    /// <param name="exceptionMessage">The exception message in case of failure</param>
    /// <param name="parameters">The exception parameters in case of failure.</param>
    /// <param name="selector">The selection function.</param>
    /// <returns>The element that satisfies the condition.</returns>
    public static T SingleOrDefault<T>(this IEnumerable<T> sequence, Func<T, bool> selector, string exceptionMessage,
                              params object[] parameters)
    {
        return sequence.WrapException(s => s.SingleOrDefault(selector), exceptionMessage, parameters);
    }

    /// <summary>
    /// Extension of the Linq Any method to add an exception message
    /// </summary>
    /// <typeparam name="T">The type of the enumerable elements.</typeparam>
    /// <param name="sequence">The enumerable instance on which this extension is invoked.</param>
    /// <param name="exceptionMessage">The exception message in case of failure</param>
    /// <param name="parameters">The exception parameters in case of failure.</param>
    /// <returns>The element that satisfies the condition</returns>
    public static bool Any<T>(this IEnumerable<T> sequence, string exceptionMessage, params object[] parameters)
    {
        return sequence.WrapException(s => s.Any(), exceptionMessage, parameters);
    }

    /// <summary>
    /// Extension of the Linq Any method to add an exception message and using a function selector.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable elements.</typeparam>
    /// <param name="sequence">The enumerable instance on which this extension is invoked.</param>
    /// <param name="exceptionMessage">The exception message in case of failure</param>
    /// <param name="parameters">The exception parameters in case of failure.</param>
    /// <param name="selector">The selection function.</param>
    /// <returns>The element that satisfies the condition.</returns>
    public static bool Any<T>(this IEnumerable<T> sequence, Func<T, bool> selector, string exceptionMessage,
                              params object[] parameters)
    {
        return sequence.WrapException(s => s.Any(selector), exceptionMessage, parameters);
    }

    /// <summary>
    /// Extension of the Linq First method to add an exception message
    /// </summary>
    /// <typeparam name="T">The type of the enumerable elements.</typeparam>
    /// <param name="sequence">The enumerable instance on which this extension is invoked.</param>
    /// <param name="exceptionMessage">The exception message in case of failure</param>
    /// <param name="parameters">The exception parameters in case of failure.</param>
    /// <returns>The element that satisfies the condition</returns>
    public static T First<T>(this IEnumerable<T> sequence, string exceptionMessage, params object[] parameters)
    {
        return sequence.WrapException(s => s.First(), exceptionMessage, parameters);
    }

    /// <summary>
    /// Returns the first element in a sequence that satisfies a specific condition. 
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    /// <param name="selector">A function to test each element for a condition.</param>
    /// <param name="exceptionMessage">An error message pattern that will used for an exception wrapper, 
    /// when the inner LINQ extension throws an exception.</param>
    /// <param name="parameters">The parameters for the exception message.</param>
    /// <returns>The first element that satisfies the specified condition.</returns>
    public static T First<T>(this IEnumerable<T> sequence, Func<T, bool> selector, 
        string exceptionMessage, params object[] parameters)
    {
        return sequence.WrapException(s => s.First(selector), exceptionMessage, parameters);
    }
    /// <summary>
    /// Returns the element at a specified index in a sequence or a default value if the index is out of range.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> to return an element from.</param>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <param name="defaultValue">The default value that is returned, when the index is out of range.</param>
    /// <returns>The default value if the index is outside the bounds of the source sequence; otherwise, the element at the specified position in the source sequence.</returns>
    public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index, TSource defaultValue)
    {
        var result = source.ElementAtOrDefault(index);
        if (Equals(result, default(TSource)))
        {
            return defaultValue;
        }
        return result;
    }

    /// <summary>
    /// Returns the first element in a sequence that satisfies a specific condition. 
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <param name="sequence">The sequence on which the extension is invoked.</param>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <param name="exceptionMessage">An error message pattern that will used for an exception wrapper, 
    /// when the inner LINQ extension throws an exception.</param>
    /// <param name="parameters">The parameters for the exception message.</param>
    /// <returns>The first element that satisfies the specified condition.</returns>
    public static T ElementAt<T>(this IEnumerable<T> sequence, int index,
        string exceptionMessage, params object[] parameters)
    {
        return sequence.WrapException(s => s.ElementAt(index), exceptionMessage, parameters);
    }

    /// <summary>
    /// Invoke ToString on a array of message parameters and return the result as array.
    /// </summary>
    /// <param name="messageParameters">Array of message parameters for to string operation.</param>
    /// <returns>Array of to string result.</returns>
    public static String[] ToStringArray(this object[] messageParameters)
    {
        var stringArray = messageParameters.Select(a => a == null ? "<null>" : (a as Func<object>)?.DynamicInvoke()?.ToString() ?? a.ToString()).ToArray();
        return stringArray;
    }

    
    /// <summary>
    /// Wraps a .NET LINQ extension and adds additional information in case the LINQ extension throws an exception.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    /// <typeparam name="TResult">The type of the LINQ extension result.</typeparam>
    /// <param name="sequence">The sequence on which the LINQ extension is invoked.</param>
    /// <param name="selector">The LINQ extension function.</param>
    /// <param name="exceptionMessage">The additional error message pattern.</param>
    /// <param name="parameters">The parameters of the additional error message.</param>
    /// <returns>The result of the LINQ extension.</returns>
    private static TResult WrapException<T, TResult>(this IEnumerable<T> sequence, 
        Func<IEnumerable<T>, TResult> selector, string exceptionMessage, params object[] parameters)
    {
        try
        {
            return selector(sequence);
        }
        catch(Exception ex)
        {
           throw Api.Create.Exception(ex, exceptionMessage, parameters);
        }
    }
}
