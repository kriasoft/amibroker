// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Workspace.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System;
    using System.Runtime.InteropServices;

    public enum DataSourceType
    {
        /// <summary>
        /// Use data source type from preferences
        /// </summary>
        Default,

        /// <summary>
        /// Local data source, plugin ID
        /// </summary>
        LocalSource
    }

    public enum DataSourceMode
    {
        /// <summary>
        /// Use data storage mode from preferences
        /// </summary>
        Default,

        /// <summary>
        /// Store data locally
        /// </summary>
        LocalStorage,

        /// <summary>
        /// No local data storage
        /// </summary>
        NoLocalStorage
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Workspace
    {
        public DataSourceType DataSourceType;

        public DataSourceMode DataSourceMode;

        public int NumBars;

        public int TimeBase;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public unsafe fixed int ReservedB[8];

        public int AllowMixedEODIntra;

        public int RequestDataOnSave;

        public int PadNonTradingDays;

        public int ReservedC;

        public IntradaySettings IntradaySettings;

        public int ReservedD;
    }
}
