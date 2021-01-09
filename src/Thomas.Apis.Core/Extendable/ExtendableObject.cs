using System;
using System.Collections.Generic;

namespace Thomas.Apis.Core.Extendable
{

    /// <summary>
    /// The default implementation of the <see cref="IExtendableObject"/> interface.
    /// </summary>
    [Serializable]
    public class ExtendableObject : IExtendableObject
    {
        private readonly IDictionary<string, object> m_cache = new Dictionary<string, object>();

        #region IExtendableObject Members

        IDictionary<string, object> IExtendableObject.Cache
        {
            get { return m_cache; }
        }

        #endregion
    }
}
