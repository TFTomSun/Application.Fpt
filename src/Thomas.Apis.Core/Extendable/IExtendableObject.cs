using System.Collections.Generic;
using System.ComponentModel;

namespace Thomas.Apis.Core.Extendable
{
    /// <summary>
    /// An interface for objects with dynamic members.
    /// </summary>
    public interface IExtendableObject
    {
        /// <summary>
        /// Gets the member cache of the dynamic object.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IDictionary<string, object> Cache { get; }
    }
}
