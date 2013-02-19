// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSource.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using System.Windows;

    using Models;

    public class DataSource
    {
        public class Market
        {
            public string ID { get; set; }

            public string Name { get; set; }
        }

        public class Symbol
        {
            public string ID { get; set; }

            public string Ticker { get; set; }

            public string FullName { get; set; }

            public string MarketID { get; set; }

            public int MarketIndex { get; set; }

        }

        private readonly string baseUrl = "http://www.finam.ru";

        private string exportUrl = null;

        private string exportEndpoint = null;

        private Dictionary<string, Tuple<DateTime, Quotation[]>> cache = new Dictionary<string, Tuple<DateTime, Quotation[]>>();

        private Market[] markets;

        private Symbol[] symbols;

        public DataSource(string databasePath, IntPtr mainWnd)
        {
            this.DatabasePath = databasePath;
            this.MainWnd = mainWnd;
            this.Broker = Activator.CreateInstance(Type.GetTypeFromProgID("Broker.Application", true));

            if (this.Broker.ActiveDocument == null)
            {
                var processes = Process.GetProcesses().Where(x => x.ProcessName.Contains("AmiBroker") && x.MainWindowHandle != this.MainWnd);
                
                foreach (var proc in processes)
                {
                    MessageBox.Show("Please close AmiBroker application with Process ID: " + proc.Id, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    proc.WaitForExit();
                }

                this.Broker = Activator.CreateInstance(Type.GetTypeFromProgID("Broker.Application", true));
            }
        }

        public string DatabasePath { get; set; }

        /// <summary>
        /// Gets the pointer to AmiBroker's main window.
        /// </summary>
        public IntPtr MainWnd { get; private set; }

        /// <summary>
        /// Gets AmiBroker's OLE automation object.
        /// </summary>
        public dynamic Broker { get; private set; }

        public async Task<string> GetExportQuotesUrl()
        {
            if (string.IsNullOrWhiteSpace(this.exportUrl))
            {
                using (var http = new HttpClient())
                {
                    var html = await http.GetStringAsync(baseUrl);
                    var match = Regex.Match(html, "href=\"(.*?)\">Экспорт данных<");

                    if (!match.Success)
                    {
                        throw new InvalidOperationException("Failed to obtain the export quotes URL from the www.finam.ru website.");
                    }

                    this.exportUrl = match.Groups[1].Value;
                    Debug.WriteLine("Export quotes URL found: " + this.exportUrl);
                }
            }

            return this.exportUrl;
        }

        public async Task<string> GetExportQuotesEndpoint()
        {
            if (string.IsNullOrWhiteSpace(this.exportEndpoint))
            {
                using (var http = new HttpClient())
                {
                    http.BaseAddress = new Uri(baseUrl);
                    var url = await this.GetExportQuotesUrl();
                    var html = await http.GetStringAsync(url);
                    var match = Regex.Match(html, "id=\"chartform\" action=\"http://(.*?)/");

                    if (!match.Success)
                    {
                        throw new InvalidOperationException("Failed to obtain the quotes endpoint URL from the http://www.finam.ru" + this.exportUrl);
                    }

                    this.exportEndpoint = "http://" + match.Groups[1].Value + "/";
                    Debug.WriteLine("Quotes endpoint found: " + this.exportEndpoint);
                }
            }

            return this.exportEndpoint;
        }

        public async Task<Market[]> GetMarkets()
        {
            if (this.markets == null)
            {
                using (var http = new HttpClient())
                {
                    http.BaseAddress = new Uri(baseUrl);
                    var url = await this.GetExportQuotesUrl();
                    var html = await http.GetStringAsync(url);
                    var match = Regex.Match(html, @"Finam\.IssuerProfile\.Main\.setMarkets\(\[(.*?)\]\);", RegexOptions.Singleline);
                    this.markets = match.Groups[1].Value.TrimEnd('}').Split(new string[] { "}, " }, StringSplitOptions.None).Select(x =>
                    {
                        var split = x.Split(',');
                        return new Market { ID = split[0].Substring(9), Name = split[1].Substring(9, split[1].Length - 10) };
                    }).ToArray();
                }
            }

            return this.markets;
        }

        public async Task<Symbol[]> GetSymbols()
        {
            if (this.symbols == null)
            {
                var markets = await this.GetMarkets();

                using (var http = new HttpClient())
                {
                    string html;
                    var js = new JavaScriptSerializer();
                    http.BaseAddress = new Uri(baseUrl);

                    using (var stream = await http.GetStreamAsync("/cache/icharts/icharts.js"))
                    using (var reader = new StreamReader(stream, System.Text.Encoding.GetEncoding("windows-1251")))
                    {
                        html = reader.ReadToEnd();
                    }

                    var matches = Regex.Matches(html, @"\[(.*?)\]");
                    var ids = js.Deserialize<string[]>("[" + matches[0].Groups[1].Value + "]");
                    var names = js.Deserialize<string[]>("[" + matches[1].Groups[1].Value + "]");
                    var codes = js.Deserialize<string[]>("[" + matches[2].Groups[1].Value + "]");
                    var marketids = js.Deserialize<string[]>("[" + matches[3].Groups[1].Value + "]");

                    this.symbols = ids.Select((id, i) => new Symbol { ID = id, MarketID = marketids[i], Ticker = codes[i], FullName = names[i], MarketIndex = markets.ToList().FindIndex(x => x.ID == marketids[i]) }).ToArray();
                }
            }

            return this.symbols;
        }

        public Quotation[] GetQuotes(string ticker, Periodicity periodicity, int limit, Quotation[] existingQuotes)
        {
            // Check if there is already a portion of quotes waiting to be returned to the caller
            if (this.cache.ContainsKey(ticker))
            {
                var quotes = this.cache[ticker];

                Debug.WriteLine("Found " + quotes.Item2.Length + " in the cache");
                
                // Check if quotes were fetched less than 5 minutes ago
                if (quotes.Item1 > DateTime.Now.AddMinutes(-5))
                {
                    var result = quotes.Item2;
                    this.cache.Remove(ticker);
                    return result;
                }
            }

            // If not, fetch new quotes asynchroniously
            Task.Run(async () =>
            {
                var symbol = (await this.GetSymbols()).FirstOrDefault(x => x.Ticker == ticker);

                if (symbol == null)
                {
                    Debug.WriteLine("Symbol " + ticker + " was not found.");
                    return;
                }

                if (periodicity == Periodicity.EndOfDay)
                {
                    var importFileName = Path.Combine(this.DatabasePath, "ASCII\\" + ticker + ".txt");
                    var formatFileName = Path.Combine(this.DatabasePath, "ASCII\\" + "finam.format");

                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
                    var lastDate = existingQuotes.Any() ? new AmiDate(existingQuotes[0].DateTime) : null;
                    var startDate = lastDate == null ? DateTime.Today.AddDays(-((double)limit / 5 * 7 + 10)) : new DateTime(lastDate.Year, lastDate.Month, lastDate.Day);
                    var endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.Date, tz);

                    Debug.WriteLine("Downloading " + ticker + " quotes starting from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString());

                    string csv = null;
                    var url = await this.GetExportQuotesEndpoint();
                    var newQuotes = new List<Quotation>();

                    using (var http = new HttpClient())
                    {
                        Debug.WriteLine("Setting base url " + url);
                        http.BaseAddress = new Uri(url);
                        url = string.Format("/{0}.csv?market={1}&em={2}&code={0}&df={3:g}&mf={4:g}&yf={5:g}&dt={6:g}&mt={7:g}&yt={8:g}&p=8&f={0}&e=.csv&cn={0}&dtf=1&tmf=1&MSOR=0&mstime=on&mstimever=1&sep=1&sep2=1&datf=5", ticker, symbol.MarketID, symbol.ID, startDate.Day, startDate.Month - 1, startDate.Year, endDate.Day, endDate.Month - 1, endDate.Year);
                        Debug.WriteLine(url);
                        using (var stream = await http.GetStreamAsync(url))
                        using (var reader = new StreamReader(stream))
                        {
                            while (reader.Peek() > 0)
                            {
                                var line = await reader.ReadLineAsync();
                                //Debug.WriteLine(line);
                                var data = line.Split(',');
                                var yeah = int.Parse(data[0].Substring(0, 4), CultureInfo.InvariantCulture);
                                var month = int.Parse(data[0].Substring(4, 2), CultureInfo.InvariantCulture);
                                var day = int.Parse(data[0].Substring(6, 2), CultureInfo.InvariantCulture);
                                newQuotes.Add(new Quotation
                                {
                                    DateTime = new AmiDate(yeah, month, day),
                                    Open = float.Parse(data[2], CultureInfo.InvariantCulture),
                                    High = float.Parse(data[3], CultureInfo.InvariantCulture),
                                    Low = float.Parse(data[4], CultureInfo.InvariantCulture),
                                    Price = float.Parse(data[5], CultureInfo.InvariantCulture),
                                    Volume = float.Parse(data[6], CultureInfo.InvariantCulture),
                                });
                            }
                        }
                    }

                    Debug.WriteLine("Downloaded " + newQuotes.Count + " new quotes for " + ticker);
                    
                    if (newQuotes.Any())
                    {
                        var fromDate = new AmiDate(newQuotes[0].DateTime);
                        var toDate = new AmiDate(newQuotes[newQuotes.Count - 1].DateTime);
                        Debug.WriteLine("From {0:dddd}-{1:dd}-{2:dd} till {3:dddd}-{4:dd}-{5:dd}", fromDate.Year, fromDate.Month, fromDate.Day, toDate.Year, toDate.Month, toDate.Year);
                    }
                    else
                    {
                        Debug.WriteLine("No quotes were downloaded from finam.ru for " + ticker);
                        return;
                    }

                    if (newQuotes.Count == 1 && existingQuotes.Any())
                    {
                        var lastQuote = existingQuotes[0];
                        var lastQuoteDate = new AmiDate(lastQuote.DateTime);
                        var newQuote = newQuotes[0];
                        var newQuoteDate = new AmiDate(newQuote.DateTime);

                        if (lastQuoteDate.Equals(newQuoteDate) && lastQuote.Open == newQuote.Open && lastQuote.High == newQuote.High && lastQuote.Low == newQuote.Low && lastQuote.Price == newQuote.Price)
                        {
                            Debug.WriteLine("No new quotes found for " + ticker);
                            return;
                        }
                    }

                    //newQuotes.Reverse();
                    //newQuotes.AddRange(existingQuotes);
                    var count = newQuotes.Count - 1;

                    for (var i = 0; i < count; i++)
                    {
                        if (new AmiDate(newQuotes[i].DateTime).Equals(new AmiDate(newQuotes[i + 1].DateTime)))
                        {
                            newQuotes.RemoveAt(i + 1);
                            i--;
                            count--;
                        }
                    }

                    Debug.WriteLine("Adding " + newQuotes.Count + " quotes for " + ticker + " to the cache");
                    this.cache.Add(ticker, new Tuple<DateTime,Quotation[]>(DateTime.Now, newQuotes.Take(limit).ToArray()));
                    NativeMethods.SendMessage(this.MainWnd, 0x0400 + 13000, IntPtr.Zero, IntPtr.Zero);
                }
            });

            return new Quotation[0];
        }
    }
}
