using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Communication;

namespace PME
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initialise autofac container.
        /// </summary>
        private static void InitialiseAutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CommunicationModule>();
            IContainer rootContainer = builder.Build();
           // AutofacHostFactory.Container = rootContainer;
            //// Send the container into the lower layers
            //DataAccess.Container.Instance = rootContainer;
            //Services.Container.Instance = rootContainer;

            //rootContainer.Resolve<Bootstrapper>();
        }
    }

}
