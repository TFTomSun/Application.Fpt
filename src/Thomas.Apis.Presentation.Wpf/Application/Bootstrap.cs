using Autofac;
using System.Linq;
using System.Reflection;
using System.Windows;
using Thomas.Apis.Presentation.Wpf.Views;

namespace Thomas.Apis.Presentation.Wpf.Application
{
    class Bootstrap
    {
        public IContainer Build<TMainViewModel>(UIElement element, params Assembly[] serviceAssemblies)
        {
            var builder = new ContainerBuilder();

            // Register individual components
            //builder.RegisterInstance(new TaskRepository())
            //       .As<ITaskRepository>();
            //builder.RegisterType<TaskController>();
            //builder.Register(c => new LogManager(DateTime.Now))
            //       .As<ILogger>();
            builder.RegisterInstance(new DefaultViewFacade(element))
                .AsImplementedInterfaces();
            // Scan an assembly for components
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
