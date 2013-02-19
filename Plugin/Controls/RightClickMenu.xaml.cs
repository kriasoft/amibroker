// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RightClickMenu.xaml.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Controls
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Script.Serialization;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for RightClickMenu user control.
    /// </summary>
    public partial class RightClickMenu : UserControl
    {
        private readonly DataSource dataSource;

        public RightClickMenu(DataSource dataSource)
        {
            this.dataSource = dataSource;
            this.InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            var loginBox = new LoginBox(this.dataSource);

            var window = new Window
            {
                Title = "Connect to the Data Source",
                Content = loginBox,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            window.ShowDialog();
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void UpdateSymbolList_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Update the symbol list");

            var marekts = await this.dataSource.GetMarkets();

            Debug.WriteLine(marekts.Length + " markets found");

            var symbols = await this.dataSource.GetSymbols();

            Debug.WriteLine(symbols.Length + " symbols found");

            var fileName = Path.Combine(this.dataSource.DatabasePath, "symbols.csv");
            var fileName2 = Path.Combine(this.dataSource.DatabasePath, "symbols.format");

            using (var fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (var sw = new StreamWriter(fs, Encoding.GetEncoding("windows-1251")))
            {
                fs.Position = 0;
                sw.WriteLine("Ticker,FullName,MarketID,WebID");

                foreach (var symbol in symbols)
                {
                    sw.WriteLine(symbol.Ticker + "," + symbol.FullName + "," + symbol.MarketIndex.ToString("G") + "," + symbol.ID);
                }

                sw.Flush();
                fs.SetLength(fs.Position - 2);
            }

            using (var fs = File.Open(fileName2, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (var sw = new StreamWriter(fs))
            {
                fs.Position = 0;
                sw.WriteLine("$FORMAT Ticker,FullName,MarketID,Skip\n$SKIPLINES 1\n$SEPARATOR ,\n$CONT 1\n$GROUP 255\n$AUTOADD 1\n$DEBUG 1\n$NOQUOTES 1"); /* WebID works in AmiBroker 5.60.1 and up*/
                sw.Flush();
                fs.SetLength(fs.Position);
            }

            Debug.WriteLine("Importing symbols from " + fileName);

            int result = this.dataSource.Broker.Import(0, fileName, fileName2);

            Debug.WriteLine("Import() returned " + result);

            if (result != 0)
            {
                MessageBox.Show("Failed to import symbols from " + fileName, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            for (var i = 0; i < marekts.Length; i++)
            {
                this.dataSource.Broker.Markets.Item(i).Name = marekts[i].Name;
                Debug.Write(marekts[i].Name + ",");
            }

            this.dataSource.Broker.RefreshAll();
        }

        private async void UpdateTop50_Click(object sender, RoutedEventArgs e)
        {
            using (var http = new HttpClient())
            {
                var html = await http.GetStringAsync("http://quote.rbc.ru/exchanges/demo/micex.0/daily");
                var match = Regex.Match(html, @"var preload_stock_data = eval\('\(\[(.*?)\]\)'\);");
                
                if (!match.Success)
                {
                    MessageBox.Show("Failed to load the list of most active stocks", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                html = match.Groups[1].Value;
                var regex = new Regex(@"\\[uU]([0-9A-Fa-f]{4})");
                html = regex.Replace(html, m => ((char)int.Parse(m.Value.Substring(2), NumberStyles.HexNumber)).ToString());
                html = regex.Replace(html, m => ((char)int.Parse(m.Value.Substring(2), NumberStyles.HexNumber)).ToString());
                Debug.WriteLine("\n" + html + "\n");
                var js = new JavaScriptSerializer();
                var data = js.Deserialize<EodData>(html);
                Debug.WriteLine("Symbols found: " + data.rows.Length);
                var items = data.rows.Select(x => new Tuple<string, string, double>(x[11], x[0], double.Parse(x[7]))).OrderByDescending(x => x.Item3).Take(50);

                var stocksFileName = Path.Combine(this.dataSource.DatabasePath, "top50.csv");
                var formatFileName = Path.Combine(this.dataSource.DatabasePath, "top50.format");

                using (var fs = File.Open(stocksFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                using (var sw = new StreamWriter(fs))
                {
                    fs.Position = 0;
                    foreach (var item in items)
                    {
                        sw.WriteLine(item.Item1 + ",0");
                    }
                    sw.Flush();
                    fs.SetLength(fs.Position);
                }

                using (var fs = File.Open(formatFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                using (var sw = new StreamWriter(fs))
                {
                    fs.Position = 0;
                    sw.WriteLine("$FORMAT TICKER,WATCHLIST\n$SKIPLINES 0\n$SEPARATOR ,\n$CONT 1\n$GROUP 255\n$AUTOADD 0\n$DEBUG 1\n$NOQUOTES 1");
                    sw.Flush();
                    fs.SetLength(fs.Position);
                }

                this.dataSource.Broker.Import(0, stocksFileName, formatFileName);
                this.dataSource.Broker.RefreshAll();
            }
        }

        private class EodData
        {
            public string[][] rows;
        }
    }
}
