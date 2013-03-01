// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecentInfo.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System.Runtime.InteropServices;

    public enum RecentInfiField
    {
        Last = 1 << 0,
        Open = 1 << 1,
        HighLow = 1 << 2,
        TradeVol = 1 << 3,
        TotalVol = 1 << 4,
        OpenInt = 1 << 5,
        PrevChange = 1 << 6,
        Bid = 1 << 7,
        Ask = 1 << 8,
        EPS = 1 << 9,
        Dividend = 1 << 10,
        Shares = 1 << 11,
        Week52 = 1 << 12,
        DateUpdated = 1 << 13,
        DateChanged = 1 << 14
    }

    public enum RecentInfoStatus
    {
        Update = 1 << 0,
        BidAsk = 1 << 1,
        Trade = 1 << 2,
        BarsReady = 1 << 3,
        Incomplete = 1 << 4,
        NewBid = 1 << 5,
        NewAsk = 1 << 6
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct RecentInfo
    {
        public int StructSize;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Exchange;

        public RecentInfoStatus Status;

        /// <summary>
        /// Describes which fields are valid. Bitmap
        /// </summary>
        public RecentInfiField ValidFields;

        public float Open;

        public float High;

        public float Low;

        public float Last;

        public int TradeVolOld;

        public int TotalVolOld;

        public float OpenInt;

        public float Change;

        public float Prev;

        public float Bid;

        public int BidSize;

        public float Ask;

        public int AskSize;

        public float EPS;

        public float Dividend;

        public float DivYield;

        /// <summary>
        /// Shares outstanding
        /// </summary>
        public int Shares;

        public float High52Week;

        public int HighDate52Week;

        public float Low52Week;

        public int LowDate52Week;

        /// <summary>
        /// Format YYYYMMDD
        /// </summary>
        public int DateChanged;

        /// <summary>
        /// Format HHMMSS
        /// </summary>
        public int TimeChanged;

        /// <summary>
        /// Format YYYYMMDD
        /// </summary>
        public int DateUpdated;

        /// <summary>
        /// Format HHMMSS
        /// </summary>
        public int TimeUpdated;

        /// <summary>
        /// NEW 5.27 field
        /// </summary>
        public float TradeVol;

        /// <summary>
        /// NEW 5.27 field
        /// </summary>
        public float TotalVol;
    }
}
