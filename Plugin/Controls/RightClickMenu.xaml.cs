// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginControl.xaml.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Controls
{
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

    /// <summary>
    /// Interaction logic for RightClickMenu user control.
    /// </summary>
    public partial class RightClickMenu : UserControl
    {
        public RightClickMenu(DataSource dataSource)
        {
            this.dataSource = dataSource;
            this.InitializeComponent();
        }

        private readonly DataSource dataSource;

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "This is a demo plug-in built with AmiBroker .NET SDK. For more info visit: http://github.com/kriasoft/amibroker",
                "AmiBroker® Demo Plug-in",
                MessageBoxButton.OK);
        }
    }
}
