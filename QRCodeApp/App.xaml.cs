using Prism.Events;
using QRCodeApp.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;

namespace QRCodeApp
{
    public partial class App : Application
    {
        private Bootstrapper bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            bootstrapper.Container.Dispose();
            base.OnExit(e);
        }
    }
}
