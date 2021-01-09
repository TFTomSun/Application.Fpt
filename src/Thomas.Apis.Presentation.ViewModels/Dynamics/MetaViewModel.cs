using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Thomas.Apis.Presentation.ViewModels.Dynamics
{
    public abstract class MetaViewModel<T, TMeta> : MetaViewModel<T>
    where TMeta:  BindAttribute
    {
        protected MetaViewModel(T initialValue = default):base(initialValue)
        {
        }

        public virtual TMeta Settings => this.Get(()=>this.Meta.OfType<TMeta>().SingleOrDefault());
        
    }

    public abstract class MetaViewModel: ViewModel
    {
        internal static IMetaViewModel CreateInstance(MetaViewModelContext context)
        {
            var fields = FieldTypes
                .Where(ft => typeof(IMetaViewModel<>).MakeGenericType(context.PropertyType).IsAssignableFrom(ft))
                .Select(ft => (IMetaViewModel)Activator.CreateInstance(ft));
            var sortedFields = fields.GroupBy(f => f.Rank).OrderByDescending(g => g.Key).ToArray();
            var field = sortedFields.First(g => g.Any(f => f.Supports(context))).Single();
            field.Context = context;

            context.ParentModel.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == context.PropertyName)
                {
                    field.RaisePropertyChanged(nameof(IMetaViewModel.Value));

                }
            };
            return field;
        }

        private static Type[] FieldTypes => typeof(IMetaViewModel).Assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && typeof(IMetaViewModel).IsAssignableFrom(t)).ToArray();
    }
    public abstract class MetaViewModel<T> : MetaViewModel, IMetaViewModel<T>
    {
        protected MetaViewModel(T initialValue = default)
        {
            this.CachedValue = initialValue;
            this.Context = new MetaViewModelContext(default!, "<fromValue>", 
                initialValue?.GetType() ?? typeof(T), new BindAttribute[] { },
                () => this.CachedValue, v => this.CachedValue = (T)v);
        }
        private T CachedValue { get; set; }

        protected virtual void OnFirstAccess(T value)
        {

        }

        private bool firstAcces { get; set; } = true;
        public virtual T Value
        {
            get
            {
                var value =  this.SafeContext.GetValue == null ? this.CachedValue : (T) (this.SafeContext.GetValue.Invoke() ?? default!);
                if (firstAcces)
                {
                    firstAcces = false;
                    this.OnFirstAccess(value);
                }

                return value;
            }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(this.Value ?? default!, value))
                {
                    if (this.SafeContext.SetValue == null)
                    {
                        this.CachedValue = value;
                    }
                    else
                    {
                        this.SafeContext.SetValue?.Invoke(value);
                    }

                    this.OnValueChanged(value);
                    this.OnPropertyChanged();
                }
            }
        }

        public Type ContextPropertyType { get; set; }

        protected virtual void OnValueChanged(T value)
        {
        }

        public virtual int Rank { get; } = 0;

        public virtual bool Supports(MetaViewModelContext context) => true;

        private MetaViewModelContext SafeContext => Context.NullCheck();
        private MetaViewModelContext? Context { get; set; }
        MetaViewModelContext IMetaViewModel.Context
        {
            get => this.SafeContext;
            set => this.Context = value;
        }

        Type IMetaViewModel.SupportedType => typeof(T);

        protected BindAttribute[] Meta => this.Context.NullCheck().PropertyMeta;
      
        object? IMetaViewModel.Value {get=>this.Value; set => this.Value = (T)value; }


    }
}
