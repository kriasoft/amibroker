// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Plugin.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin
{
    using System;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;

    using Models;
    using RGiesecke.DllExport;

    /// <summary>
    /// Standard implementation of a typical AmiBroker plug-ins.
    /// </summary>
    public class Plugin
    {
        /// <summary>
        /// A pointer to AmiBroker main window
        /// </summary>
        static IntPtr MainWnd = IntPtr.Zero;

        /// <summary>
        /// Plugin status code
        /// </summary>
        static StatusCode Status = StatusCode.OK;

        /// <summary>
        /// Default encoding
        /// </summary>
        static Encoding encoding = Encoding.GetEncoding("windows-1251"); // TODO: Update it based on your preferences

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void GetPluginInfo(ref PluginInfo pluginInfo)
        {
            pluginInfo.Name = "AmiBroker\x00ae data Plug-in";
            pluginInfo.Vendor = "KriaSoft LLC";
            pluginInfo.Type = PluginType.Data;
            pluginInfo.Version = 10000; // v1.0.0
            pluginInfo.IDCode = PackIDCode("TEST");
            pluginInfo.Certificate = 0;
            pluginInfo.MinAmiVersion = 5600000; // v5.60
            pluginInfo.StructSize = Marshal.SizeOf((PluginInfo)pluginInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void Init()
        {
            System.Threading.Thread.Sleep(5000);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void Release()
        {
        }

        /// <summary>
        /// GetQuotesEx function is functional equivalent fo GetQuotes but
        /// handles new Quotation format with 64 bit date/time stamp and floating point volume/open int
        /// and new Aux fields
        /// it also takes pointer to context that is reserved for future use (can be null)
        /// Called by AmiBroker 5.27 and above 
        /// </summary>
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static unsafe int GetQuotesEx(string ticker, Periodicity periodicity, int lastValid, int size, Quotation* quotes, GQEContext* context)
        {
            // TODO: Add logic here. Take a look at the demo below:

            /*for (var i = 0; i < 5; i++)
            {
                quotes[i].DateTime = PackDate(DateTime.Today.AddDays(i - 5));
                quotes[i].Price = 10;
                quotes[i].Open = 15;
                quotes[i].High = 16;
                quotes[i].Low = 9;
                quotes[i].Volume = 1000;
                quotes[i].OpenInterest = 0;
                quotes[i].AuxData1 = 0;
                quotes[i].AuxData2 = 0;
            }
            return 5;*/

            // return 'lastValid + 1' if no updates are found and you want to keep all existing records
            return lastValid + 1;
        }

        public unsafe delegate void* Alloc(uint size);

        ///// <summary>
        ///// GetExtra data is optional function for retrieving non-quotation data
        ///// </summary>
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static AmiVar GetExtraData(string ticker, string name, int arraySize, Periodicity periodicity, Alloc alloc)
        {
            return new AmiVar();
        }

        /// <summary>
        /// GetSymbolLimit function is optional, used only by real-time plugins
        /// </summary>
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetSymbolLimit()
        {
            return 10000;
        }

        /// <summary>
        /// GetStatus function is optional, used mostly by few real-time plugins
        /// </summary>
        /// <param name="statusPtr">A pointer to <see cref="AmiBrokerPlugin.PluginStatus"/></param>
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void GetStatus(IntPtr statusPtr)
        {
            switch (Status)
            {
                case StatusCode.OK:
                    SetStatus(statusPtr, StatusCode.OK, Color.LightGreen, "OK", "Connected");
                    break;
                case StatusCode.Wait:
                    SetStatus(statusPtr, StatusCode.Wait, Color.LightBlue, "WAIT", "Trying to connect...");
                    break;
                case StatusCode.Error:
                    SetStatus(statusPtr, StatusCode.Error, Color.Red, "ERR", "An error occured");
                    break;
                default:
                    SetStatus(statusPtr, StatusCode.Unknown, Color.LightGray, "Ukno", "Unknown status");
                    break;
            }
        }

        #region Helper Functions

        /// <summary>
        /// Converts string code into Int32 required by AmiBroker. Used to define a unique ID for the plug-in.
        /// </summary>
        private static int PackIDCode(string id)
        {
            if (id.Length != 4)
            {
                throw new ArgumentException("Plugin ID must be 4 characters long.", "id");
            }

            return id[0] << 24 | id[1] << 16 | id[2] << 8 | id[3] << 0;
        }

        /// <summary>
        /// Sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message.
        /// </summary>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Notify AmiBroker that new streaming data arrived
        /// </summary>
        static void NotifyStreamingUpdate()
        {
            SendMessage(MainWnd, 0x0400 + 13000, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Pack AmiBroker DateTime object into UInt64
        /// </summary>
        static ulong PackDate(DateTime date, bool isFuturePad = false)
        {
            var isEOD = date.Hour == 0 && date.Minute == 0 && date.Second == 0;

            // lower 32 bits
            var ft = BitVector32.CreateSection(1);
            var rs = BitVector32.CreateSection(23, ft);
            var ms = BitVector32.CreateSection(999, rs);
            var ml = BitVector32.CreateSection(999, ms);
            var sc = BitVector32.CreateSection(59, ml);

            var bv1 = new BitVector32(0);
            bv1[ft] = isFuturePad ? 1 : 0;         // bit marking "future data"
            bv1[rs] = 0;                            // reserved set to zero
            bv1[ms] = 0;                            // microseconds 0..999
            bv1[ml] = date.Millisecond;             // milliseconds 0..999
            bv1[sc] = date.Second;                  // 0..59

            // higher 32 bits
            var mi = BitVector32.CreateSection(59);
            var hr = BitVector32.CreateSection(23, mi);
            var dy = BitVector32.CreateSection(31, hr);
            var mn = BitVector32.CreateSection(12, dy);
            var yr = BitVector32.CreateSection(4095, mn);

            var bv2 = new BitVector32(0);
            bv2[mi] = isEOD ? 63 : date.Minute;     // 0..59        63 is reserved as EOD marker
            bv2[hr] = isEOD ? 31 : date.Hour;       // 0..23        31 is reserved as EOD marker
            bv2[dy] = date.Day;                     // 1..31
            bv2[mn] = date.Month;                   // 1..12
            bv2[yr] = date.Year;                    // 0..4095

            return ((ulong)bv2.Data << 32) ^ (ulong)bv1.Data;
        }

        /// <summary>
        /// Unpack UInt64 object into AmiBroker DateTime
        /// </summary>
        static DateTime UnpackDate(ulong date)
        {
            // lower 32 bits
            var ft = BitVector32.CreateSection(1);
            var rs = BitVector32.CreateSection(23, ft);
            var ms = BitVector32.CreateSection(999, rs);
            var ml = BitVector32.CreateSection(999, ms);
            var sc = BitVector32.CreateSection(59, ml);
            var bv1 = new BitVector32((int)(date << 32 >> 32));

            // higher 32 bits
            var mi = BitVector32.CreateSection(59);
            var hr = BitVector32.CreateSection(23, mi);
            var dy = BitVector32.CreateSection(31, hr);
            var mn = BitVector32.CreateSection(12, dy);
            var yr = BitVector32.CreateSection(4095, mn);
            var bv2 = new BitVector32((int)(date >> 32));

            var hour = bv2[hr];
            var minute = bv2[mi];
            var second = bv1[sc];
            var milsec = bv1[ml];

            if (hour > 24 || minute > 59 || second > 59 || milsec > 999)
            {
                return new DateTime(bv2[yr], bv2[mn], bv2[dy]);
            }

            return new DateTime(bv2[yr], bv2[mn], bv2[dy], hour, minute, second, milsec);
        }

        /// <summary>
        /// Update status of the plugin
        /// </summary>
        /// <param name="statusPtr">A pointer to <see cref="AmiBrokerPlugin.PluginStatus"/></param>
        static void SetStatus(IntPtr statusPtr, StatusCode code, Color color, string shortMessage, string fullMessage)
        {
            Marshal.WriteInt32(new IntPtr(statusPtr.ToInt32() + 4), (int)code);
            Marshal.WriteInt32(new IntPtr(statusPtr.ToInt32() + 8), color.R);
            Marshal.WriteInt32(new IntPtr(statusPtr.ToInt32() + 9), color.G);
            Marshal.WriteInt32(new IntPtr(statusPtr.ToInt32() + 10), color.B);

            var msg = encoding.GetBytes(fullMessage);

            for (int i = 0; i < (msg.Length > 255 ? 255 : msg.Length); i++)
            {
                Marshal.WriteInt32(new IntPtr(statusPtr.ToInt32() + 12 + i), msg[i]);
            }

            Marshal.WriteInt32(new IntPtr(statusPtr.ToInt32() + 12 + msg.Length), 0x0);

            msg = encoding.GetBytes(shortMessage);

            for (int i = 0; i < (msg.Length > 31 ? 31 : msg.Length); i++)
            {
                Marshal.WriteInt32(new IntPtr(statusPtr.ToInt32() + 268 + i), msg[i]);
            }

            Marshal.WriteInt32(new IntPtr(statusPtr.ToInt32() + 268 + msg.Length), 0x0);
        }

        #endregion

        #region AmiBroker Method Delegates

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        delegate int GetStockQtyDelegate();

        static GetStockQtyDelegate GetStockQty;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        delegate int SetCategoryNameDelegate(int category, int item, string name);

        static SetCategoryNameDelegate SetCategoryName;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        delegate string GetCategoryNameDelegate(int category, int item);

        static GetCategoryNameDelegate GetCategoryName;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        delegate int SetIndustrySectorDelegate(int industry, int sector);

        static SetIndustrySectorDelegate SetIndustrySector;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        delegate int GetIndustrySectorDelegate(int industry);

        static GetIndustrySectorDelegate GetIndustrySector;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate IntPtr AddStockDelegate(string ticker); // returns a pointer to StockInfo

        static AddStockDelegate AddStock;

        #endregion
    } 
}
