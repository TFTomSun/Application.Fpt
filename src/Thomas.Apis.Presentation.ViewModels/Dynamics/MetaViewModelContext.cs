using System;

namespace Thomas.Apis.Presentation.ViewModels.Dynamics
{
    public class MetaViewModelContext
    {
        public Action<object?> SetValue { get;  }
        public Func<object?> GetValue { get;  }
        public IViewModel ParentModel { get; }
        public string PropertyName { get; }
        public Type PropertyType { get; }
        public BindAttribute[] PropertyMeta { get; }

        public MetaViewModelContext(IViewModel parentModel, string propertyName, Type propertyType, BindAttribute[] propertyMeta, Func<object?> get, Action<object?> set)
        {
            ParentModel = parentModel;
            PropertyName = propertyName;
            PropertyType = propertyType;
            PropertyMeta = propertyMeta;
            GetValue = get;
            SetValue = set;
        }
    }
}