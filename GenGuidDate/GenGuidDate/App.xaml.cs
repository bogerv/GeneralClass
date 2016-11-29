using MahApps.Metro;
using MahAppsMetroThemesSample;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GenGuidDate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //// get the current app style (theme and accent) from the application
            //// you can then use the current theme and custom accent instead set a new theme
            //Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);

            //// now set the Green accent and dark theme
            //ThemeManager.ChangeAppStyle(Application.Current,
            //                            ThemeManager.GetAccent("Blue"),
            //                            ThemeManager.GetAppTheme("BaseLight")); // or appStyle.Item1

            // add custom accent and theme resource dictionaries
            ThemeManager.AddAccent("CustomAccent1", new Uri("pack://application:,,,/GenGuidDate;component/CustomAccents/CustomAccent1.xaml"));
            ThemeManager.AddAccent("CustomAccent2", new Uri("pack://application:,,,/GenGuidDate;component/CustomAccents/CustomAccent2.xaml"));
            ThemeManager.AddAppTheme("CustomTheme", new Uri("pack://application:,,,/GenGuidDate;component/CustomAccents/CustomTheme.xaml"));

            // create custom accents
            ThemeManagerHelper.CreateAppStyleBy(Colors.Red);
            ThemeManagerHelper.CreateAppStyleBy(Colors.GreenYellow);
            ThemeManagerHelper.CreateAppStyleBy(Colors.Green, true);

            base.OnStartup(e);
        }
    }
}
