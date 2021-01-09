using System;

namespace Thomas.Apis.Core.New
{
    /// <summary>
    /// An attribute to mark new stuff that needs to be incorporated into the framework
    /// </summary>
    public class NewAttribute : Attribute
    {
        private string Message { get; }

        public NewAttribute(string message = null)
        {
            Message = message;
        }
    }
}