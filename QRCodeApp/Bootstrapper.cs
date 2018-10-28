using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using MahApps.Metro;
using QRCodeApp.Services;

namespace QRCodeApp
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterInstance(new MainWindow());
        }

        protected override void InitializeShell()
        {
            Container.Resolve<MainWindow>().Show();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
    }
}
