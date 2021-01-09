using Autofac;
using System.Linq;
using System.Reflection;
using System.Windows;
using Thomas.Apis.Presentation.Wpf.Views;

namespace Thomas.Apis.Presentation.Wpf.Application
{
    class WpfAppBootstrap
    {
        public IContainer Build<TMainViewModel>(UIElement element, params Assembly[] serviceAssemblies)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(new DefaultViewFacade(element))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(serviceAssemblies)
               .Where(t => t.Name.EndsWith("Service"))
               .AsImplementedInterfaces()
               .AsSelf();

            builder.RegisterAssemblyTypes(typeof(TMainViewModel).Assembly)
                   .Where(t => t.Name.EndsWith("ViewModel"))
                   .AsSelf();

            var container = builder.Build();
            return container;
        }
    }

  

}
