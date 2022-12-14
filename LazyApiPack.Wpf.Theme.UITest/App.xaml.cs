using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LazyApiPack.Wpf.Theme.UITest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
           
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemeManager.Start(this,
               SkeumorphDarkTheme.CreateSkeumorphNightTheme(null, null, null,
                   new[] { "SkeuTest1", "SkeuTest2" },
                   new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.UITest;component/Themes/CustomSkeumorphTheme.xaml", UriKind.Absolute)),
               SkeumorphBrightTheme.CreateSkeumorphBrightBlueTheme(null, null, null,
                   new[] { "SkeuTest3", "SkeuTest4" },
                   new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.UITest;component/Themes/CustomSkeumorphTheme.xaml", UriKind.Absolute)),
               SkeumorphBrightTheme.CreateSkeumorphBrightGreenTheme(null, null, null,
                   new[] { "SkeuTest5", "SkeuTest6" },
                   new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.UITest;component/Themes/CustomSkeumorphTheme.xaml", UriKind.Absolute))
               );

            ThemeManager.Instance.SetTheme("Skeumorph Dark Night");
        }
    }
}
