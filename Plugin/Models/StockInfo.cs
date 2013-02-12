// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StockInfo.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct StockInfo
    {
        // Offset 0
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
        internal IntPtr ShortName;

        // Offset 48
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
        internal IntPtr AliasName;

        // Offset 96
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
        internal IntPtr WebID;

        // Offset 144
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal IntPtr FullName;

        // Offset 272
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal IntPtr Address;

        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        internal IntPtr Country; // Offset 400

        /// <summary>
        /// ISO 3 letter currency code
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        internal IntPtr Currency; // Offset 464

        /// <summary>
        /// The ID of the data plug-in, 0 - accept workspace settings
        /// </summary>
        public int DataSource; // Offset 468

        /// <summary>
        /// Local mode of operation - 0 - accept workspace settings, 1 - store locally, 2 - don't store locally
        /// </summary>
        public int DataLocalMode; // Offset 472

        // Offset 476
        public int MarketID;

        public int GroupID;

        public int IndustryID;

        public int GICS;

        /// <summary>
        /// continuous etc.
        /// </summary>
        public int Flags;

        public int MoreFlags;

        /// <summary>
        /// new futures fields - active if SI_MOREFLAGS_FUTURES is set
        /// </summary>
        public float MarginDeposit;

        public float PointValue;

        public float RoundLotSize;

        /// <summary>
        /// New futures fields - active if SI_MOREFLAGS_FUTURES is set
        /// </summary>
        public float TickSize;

        /// <summary>
        /// Number of decimal places to display
        /// </summary>
        public int Decimals;

        public unsafe fixed short LastSplitFactor[2];

        public ulong LastSplitDate;

        public ulong DividendPayDate;

        public ulong ExDividendDate;

        public float SharesFloat;

        public float SharesOut;

        public float DividendPerShare;

        public float BookValuePerShare;

        /// <summary>
        /// PE Growth ratio
        /// </summary>
        public float PEGRatio;

        public float ProfitMargin;

        public float OperatingMargin;

        public float OneYearTargetPrice;

        public float ReturnOnAssets;

        public float ReturnOnEquity;

        /// <summary>
        ///  Year over year
        /// </summary>
        public float QtrlyRevenueGrowth;

        public float GrossProfitPerShare;

        /// <summary>
        /// TTN Sales Revenue
        /// </summary>
        public float SalesPerShare;

        public float EBITDAPerShare;

        public float QtrlyEarningsGrowth;

        public float InsiderHoldPercent;

        public float InstitutionHoldPercent;

        public float SharesShort;

        public float SharesShortPrevMonth;

        /// <summary>
        /// From Forward P/E
        /// </summary>
        public float ForwardEPS;

        /// <summary>
        /// TTM EPS
        /// </summary>
        public float EPS;

        public float EPSEstCurrentYear;

        public float EPSEstNextYear;

        public float EPSEstNextQuarter;

        public float ForwardDividendPerShare;

        public float Beta;

        public float OperatingCashFlow;

        public float LeveredFreeCashFlow;

        public unsafe fixed float ReservedInternal[28];

        public unsafe fixed float UserData[100];
    }
}
