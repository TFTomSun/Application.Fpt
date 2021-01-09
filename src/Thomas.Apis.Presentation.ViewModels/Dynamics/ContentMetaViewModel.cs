using System.Linq;
using Thomas.Apis.Presentation.ViewModels.Dynamics;

namespace Thomas.Apis.Presentation.ViewModels.Layouts
{
    public abstract class ContentMetaViewModel<TMeta> : ContentMetaViewModel<object, TMeta>
        where TMeta : BindAttribute
    {
    }
    public abstract class ContentMetaViewModel<TValue, TMeta> : MetaViewModel<TValue, TMeta>
        where TMeta : BindAttribute
    {
        IMetaViewModel Me => this;


        /// <inheritdoc />
        public override bool Supports(MetaViewModelContext context)
        {
            var supported = context.PropertyMeta.OfType<TMeta>().Any();
            return supported;
        }

        /// <summary>
        /// Set the rank above average, because it should be preferred, when the give meta data is present.
        /// </summary>
        public override int Rank => 10;

        /// <summary>
        /// Creates an instances based on this models value access, but without it's type specific meta data, so that the appropriate inner view model is created.
        /// </summary>
        public IViewModel Content => this.Get(
            () => CreateInstance(new MetaViewModelContext(
                this, nameof(Value), Me.Context.PropertyType,
                    Me.Context.PropertyMeta.Where(x => !(x is TMeta)).ToArray(),
                Me.Context.GetValue, Me.Context.SetValue)));
    }
}