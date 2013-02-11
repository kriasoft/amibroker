// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Workspace.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public class Workspace
    {
        /// <summary>
        /// 0 - use preferences, 1 - local, ID of plugin
        /// </summary>
        public int DataSource;

        /// <summary>
        /// 0 - use preferences, 1 - store locally, 2 - don't
        /// </summary>
        public int DataLocalMode;

        public int NumBars;

        public int TimeBase;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public int[] reservedB;

        public int AllowMixedEODIntra;

        public int RequestDataOnSave;

        public int PadNonTradingDays;

        public int ReservedC;

        public IntPtr IntradaySettings;

        public int ReservedD;
    }
}
