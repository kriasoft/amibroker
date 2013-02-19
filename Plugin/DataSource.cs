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
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;

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
                    using (var reader = new System.IO.StreamReader(stream, System.Text.Encoding.GetEncoding("windows-1251")))
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

        public Quotation[] GetQuotes(string ticker, Periodicity periodicity, int limit, Quotation[] lastQuotes = null)
        {
            // Check if there is already a portion of quotes waiting to be returned to the caller
            if (this.cache.ContainsKey(ticker))
            {
                var quotes = this.cache[ticker];
                
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
                    var quotesFileName = System.IO.Path.Combine(this.DatabasePath, ticker + ".csv");
                    var formatFileName = System.IO.Path.Combine(this.DatabasePath, "quotes.format");

                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
                    var lastDate = lastQuotes == null ? null : new AmiDate(lastQuotes[0].DateTime);
                    var startDate = lastQuotes == null ? DateTime.Today.AddDays(-((double)limit / 5 * 7 + 10)) : new DateTime(lastDate.Year, lastDate.Month, lastDate.Day);
                    var endDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.Date, tz);

                    Debug.WriteLine("Downloading " + ticker + " quotes starting from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString());

                    string csv = null;

                    using (var http = new HttpClient())
                    {
                        var url = await this.GetExportQuotesEndpoint();
                        Debug.WriteLine("Setting base url " + url);
                        http.BaseAddress = new Uri(url);
                        url = string.Format("/{0}.csv?market={1}&em={2}&code={0}&df={3:dd}&mf={3:mm}&yf={3:yyyy}&dt={4:dd}&mt={4:mm}&yt={4:yyyy}&p=8&f={0}&e=.csv&cn={0}&dtf=1&tmf=1&MSOR=0&mstime=on&mstimever=1&sep=1&sep2=2&datf=1", ticker, symbol.MarketID, symbol.ID, startDate, endDate);
                        Debug.WriteLine(url);
                        csv = await http.GetStringAsync(url);
                        Debug.WriteLine(csv.Substring(0, 200));
                    }

                    Debug.WriteLine("Saving quotes for " + ticker + " to " + quotesFileName);

                    using (var fs = File.Open(quotesFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    using (var sw = new StreamWriter(fs))
                    {
                        fs.Position = 0;
                        sw.Write(csv);
                        sw.Flush();
                        fs.SetLength(fs.Position);
                    }

                    Debug.WriteLine("Creating " + formatFileName);

                    using (var fs = File.Open(formatFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    using (var sw = new StreamWriter(fs))
                    {
                        fs.Position = 0;
                        sw.WriteLine("$FORMAT Ticker,Skip,Date_YMD,Skip,Open,High,Low,Close,Volume\n$SKIPLINES 0\n$SEPARATOR ,\n$DEBUG 1\n$AUTOADD 1\n$CONT 1\n$GROUP 254\n$BREAKONERR 1");
                        sw.Flush();
                        fs.SetLength(fs.Position);
                    }

                    Debug.WriteLine("Importing quotes for " + ticker + " from " + quotesFileName);

                    this.Broker.Import(0, quotesFileName, formatFileName);
                    this.Broker.RefreshAll();
                }
            });

            return new Quotation[0];
        }
    }
}
