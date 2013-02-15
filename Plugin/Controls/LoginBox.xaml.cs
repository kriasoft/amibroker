// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginBox.xaml.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Controls
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for LoginBox.xaml
    /// </summary>
    public partial class LoginBox : UserControl
    {
        private readonly DataSource dataSource;

        public LoginBox(DataSource dataSource)
        {
            this.dataSource = dataSource;
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Hello " + txtUserName.Text);
        }
    }
}
