using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Thomas.Apis.Core.New;
using Thomas.Apis.Presentation.ViewModels.Command;
using Thomas.Apis.Presentation.ViewModels.Dynamics;

namespace Thomas.Apis.Presentation.ViewModels.Layouts
{
    [New]
    public class AutoGridViewModel:MetaViewModel<IViewModel,View.Layout.Grid>
    {
        protected override async void OnFirstAccess(IViewModel value)
        {
            await this.UpdateCollectionAsync(value);
        }
        public override View.Layout.Grid? Settings => this.Value?.GetType().GetAttribute<View.Layout.Grid>();

        [Obsolete("Only for WPF dynamic access",true)]
        public AutoGridViewModel(){}

        public AutoGridViewModel(IViewModel representative):base(representative)
        {
        }
        public override bool Supports(MetaViewModelContext context) => context.PropertyType.GetAttribute<View.Layout.Grid>() != null;
        public override int Rank => -1;

        private  async Task UpdateCollectionAsync(IViewModel? model = null)
        {
            if (model != null)
            {
                //this.ViewModelProperties = new Progress().To().Enumerable().AsEnumerable<IViewModel>()
                //    .ToObservableCollection();
                var handDownMetas = model?.GetType().GetCustomAttributes<BindAttribute>()
                    .Where(x => x.InheritToChildren).ToArray();

                var fields = await this.Run(true,()=>model?.GetType().GetProperties()
                    .Select(p => (BindingInfos: p.GetCustomAttributes<BindAttribute>().ToArray(), Property: p))
                    .Where(x => x.BindingInfos?.Length > 0)
                    .Select(x =>
                    {
                        var property = x.Property;
                        var mergedData = handDownMetas.Concat(x.BindingInfos).GroupBy(g=>g.GetType()).Select(g=>g.Merge()).ToArray();
                        var field = CreateInstance(new MetaViewModelContext(
                            model, property.Name, property.PropertyType, mergedData,
                            () => property.GetValue(model),v => property.SetValue(model, v)));

                        return field.As<IViewModel>();
                    }).ToArray());
                //this.ViewModelProperties.Reset(fields);
                this.ViewModelProperties = fields.ToObservableCollection();
            }
        }

        private Task<T> Run<T>(bool async, Func<T> action)
        {
            if (async)
            {
                return Task.Run(action);
            }
            return Task.FromResult(action());
        }



        public ObservableCollection<IViewModel> ViewModelProperties
        {
            get => this.Get<ObservableCollection<IViewModel>>();
            set => this.Set(value);
        }

    }
}
