// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginControl.xaml.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for PluginControl user control.
    /// </summary>
    public partial class PluginControl : UserControl
    {
        internal struct market
        {
            public int value;
            public string title;
        }

        public PluginControl(DataSource dataSource)
        {
            this.dataSource = dataSource;
            this.InitializeComponent();
        }

        private readonly DataSource dataSource;

        private async void UpdateSymbolList_Click(object sender, RoutedEventArgs e)
        {
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri("http://www.finam.ru");
                var html = await http.GetStringAsync("/analysis/profile041CA00007/default.asp");
                var marketsJsonText = Regex.Match(html, @"Finam\.IssuerProfile\.Main\.setMarkets\(\[(.*?)\]\);", RegexOptions.Singleline).Groups[1].Value.Replace("'", "\"");
                var js = new JavaScriptSerializer();
                var marketsTemp = js.Deserialize<market[]>("[" + marketsJsonText + "]");
                var markets = new Dictionary<int, string>(marketsTemp.Length);

                for (var i = 0; i < marketsTemp.Length; i++)
                {
                    markets[marketsTemp[i].value] = marketsTemp[i].title;
                }

                var marketIDs = markets.Select(x => x.Key).ToArray();

                using (var stream = await http.GetStreamAsync("/cache/icharts/icharts.js"))
                using (var reader = new StreamReader(stream, Encoding.GetEncoding("windows-1251")))
                {
                    html = reader.ReadToEnd();
                }

                var matches = Regex.Matches(html, @"\[(.*?)\]");
                var ids = js.Deserialize<int[]>("[" + matches[0].Groups[1].Value + "]");
                var names = js.Deserialize<string[]>("[" + matches[1].Groups[1].Value + "]");
                var codes = js.Deserialize<string[]>("[" + matches[2].Groups[1].Value + "]");
                var marketids = js.Deserialize<int[]>("[" + matches[3].Groups[1].Value + "]");

                int marketID;
                var sb = new StringBuilder();
                sb.AppendLine("Ticker,FullName,MarketID");

                for (var i = 0; i < ids.Length; i++)
                {
                    marketID = Array.IndexOf<int>(marketIDs, marketids[i]);
                    sb.AppendLine(codes[i] + "," + names[i] + "," + marketID.ToString("G"));
                }

                var fileName = Path.Combine(this.dataSource.DatabasePath, "symbols.csv");
                var fileName2 = Path.Combine(this.dataSource.DatabasePath, "symbols.format");

                using (var fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                using (var sw = new StreamWriter(fs, Encoding.GetEncoding("windows-1251")))
                {
                    fs.Position = 0;
                    sw.WriteLine("Ticker,FullName,MarketID");

                    for (var i = 0; i < ids.Length; i++)
                    {
                        marketID = Array.IndexOf<int>(marketIDs, marketids[i]);
                        sw.WriteLine(codes[i] + "," + names[i] + "," + marketID.ToString("G"));
                    }

                    sw.Flush();
                    fs.SetLength(fs.Position - 2);
                }

                using (var fs = File.Open(fileName2, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                using (var sw = new StreamWriter(fs))
                {
                    fs.Position = 0;
                    sw.WriteLine("$FORMAT Ticker, FullName, MarketID\n$SKIPLINES 1\n$SEPARATOR ,\n$CONT 1\n$GROUP 255\n$AUTOADD 1\n$DEBUG 1\n$NOQUOTES 1");
                    sw.Flush();
                    fs.SetLength(fs.Position);
                }

                this.dataSource.Broker.Import(0, fileName, fileName2);

                for (var i = 0; i < marketsTemp.Length; i++)
                {
                    this.dataSource.Broker.Markets.Item(i).Name = marketsTemp[i].title;
                }

                this.dataSource.Broker.RefreshAll();
            }
        }
    }
}
