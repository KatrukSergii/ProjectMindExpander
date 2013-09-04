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
        public App()
        {
            InitializeIoC();
        }

        public static IContainer AutoFacContainer { get; private set; }

        /// <summary>
        /// Initialise autofac container.
        /// </summary>
        private static void InitializeIoC()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CommunicationModule>();            
            AutoFacContainer = builder.Build();
        }
    }

}
