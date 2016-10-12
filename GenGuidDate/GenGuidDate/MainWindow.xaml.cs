using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using GenGuidDate.Windows;

namespace GenGuidDate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            //var People = new List<object> {
            //    new { Name="a", Age=12},
            //    new { Name="b", Age=13},
            //    new { Name="c", Age=14},
            //    new { Name="d", Age=15},
            //};
            //this.guidList.ItemsSource = People;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var text = this.guidNum.Text;
            int num = 0;
            if (!string.IsNullOrEmpty(text))
            {
                num = Convert.ToInt32(text);
            }

            var list = new List<object>();
            for (int i = 0; i < num; i++)
            {
                list.Add(new { Id = Guid.NewGuid().ToString().ToLower() });
            }
            this.guidList.ItemsSource = list;
        }

        private void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            new NewWindow() { Owner = this }.ShowDialog();
        }

        private MetroWindow accentThemeTestWindow;

        private void ChangeAppStyleButtonClick(object sender, RoutedEventArgs e)
        {
            if (accentThemeTestWindow != null)
            {
                accentThemeTestWindow.Activate();
                return;
            }

            accentThemeTestWindow = new AccentStyleWindow();
            accentThemeTestWindow.Owner = this;
            accentThemeTestWindow.Closed += (o, args) => accentThemeTestWindow = null;
            accentThemeTestWindow.Left = this.Left + this.ActualWidth / 2.0;
            accentThemeTestWindow.Top = this.Top + this.ActualHeight / 2.0;
            accentThemeTestWindow.Show();
        }

        #region 应用全屏与还原
        private void FullClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            this.UseNoneWindowStyle = true;
            this.IgnoreTaskbarOnMaximize = true;
        }

        private void NormalClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.UseNoneWindowStyle = false;
            this.ShowTitleBar = true; // <-- this must be set to true
            this.IgnoreTaskbarOnMaximize = false;
        }
        #endregion
    }
}
