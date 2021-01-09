using System.Reflection;
using System.Windows.Threading;
using Thomas.Apis.Core;
using Thomas.Apis.Presentation.Wpf.Core;

public static class FactoryExtensions
{
    private class MetaData<T> : IMetaData<T> { }
    public static IMetaData<T> MetaDataOf<T>(this IGlobal global) => new MetaData<T>();

    private static FieldInfo disableProcessingCountField;

    /// <summary>
    /// Gets if dispatcher is suspended.
    /// </summary>
    /// <param name="dispatcher">Dispatcher to get information from.</param>
    /// <returns>True if dispatcher is suspended.</returns>
    public static bool IsSuspended(this Dispatcher dispatcher)
    {
        if (disableProcessingCountField == null)
        {
            var dispatcherType = dispatcher.GetType();
            disableProcessingCountField = dispatcherType.GetField(
                "_disableProcessingCount", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        var count = (int)disableProcessingCountField.GetValue(Dispatcher.CurrentDispatcher);
        var suspended = count > 0;
        return suspended;
    }

   

 
}

