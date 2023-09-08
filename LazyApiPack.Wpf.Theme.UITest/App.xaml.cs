using LazyApiPack.Theme;
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

            var a = SkeumorphDarkTheme.CreateSkeumorphNightTheme(null, null, null,
                   new[] { "SkeuTest1", "SkeuTest2" },
                   new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.UITest;component/Themes/CustomSkeumorphTheme.xaml", UriKind.Absolute));
            var b = SkeumorphBrightTheme.CreateSkeumorphBrightBlueTheme(null, null, null,
                  new[] { "SkeuTest3", "SkeuTest4" },
                  new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.UITest;component/Themes/CustomSkeumorphTheme.xaml", UriKind.Absolute));
            var c = SkeumorphBrightTheme.CreateSkeumorphBrightGreenTheme(null, null, null,
                      new[] { "SkeuTest5", "SkeuTest6" },
                      new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.UITest;component/Themes/CustomSkeumorphTheme.xaml", UriKind.Absolute));
            ThemeService = new ThemeService(this, new[] { a, b, c });
            ThemeService.CurrentTheme = a;
        }

        public static IThemeService ThemeService { get; private set; }
    }
}
