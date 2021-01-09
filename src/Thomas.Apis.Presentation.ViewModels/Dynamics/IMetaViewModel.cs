using System;

namespace Thomas.Apis.Presentation.ViewModels.Dynamics
{
    public interface IMetaViewModel<in T>: IMetaViewModel
    {
    }

    /// <summary>
    /// A common interface for dynamic meta based view models.
    /// </summary>
    public interface IMetaViewModel:IViewModel
    {
        /// <summary>
        /// Gets the rank of the model. If the rank is higher, it will be chosen over other meta view models with a lower rank.
        /// </summary>
        int Rank { get; }

        /// <summary>
        /// Determines whether the view model supports the given context.
        /// </summary>
        /// <param name="context">The context for which the view model would be created.</param>
        /// <returns></returns>
        bool Supports(MetaViewModelContext context);

        internal MetaViewModelContext Context { get; set;  }
        /// <summary>
        /// Gets the minimum supported property type.
        /// </summary>
        Type SupportedType {get; }

        //internal Action<object?> SetValue { get; set; }
        //internal Func<object?> GetValue { get; set;}

        /// <summary>
        /// Gets or sets the value that can be bound.
        /// </summary>
        object? Value { get; set; }

        ///// <summary>
        ///// Gets the context meta for which this model has been created.
        ///// </summary>
        //public BindAttribute[] ContextMeta { get; internal set; }

        ///// <summary>
        ///// Gets or sets the type of the context for which this model has been created.
        ///// </summary>
        //internal Type ContextPropertyType { get; set; }
    }
}
