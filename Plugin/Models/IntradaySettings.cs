// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntradaySettings.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System.Runtime.InteropServices;

    public enum DailyCompressionMode
    {
        Exchange,
        Local,
        SessionBased
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct IntradaySettings
    {
        public int TimeShift; /* In hours */

        public int FilterAfterHours;

        public ulong SessionStart; /* Bit encoding HHHHH.MMMMMM.0000   hours << 10 | ( minutes << 4 ) */

        public ulong SessionEnd; /* Bit encoding HHHHH.MMMMMM.0000   hours << 10 | ( minutes << 4 ) */

        public int FilterWeekends;

        public DailyCompressionMode DailyCompressionMode;

        public ulong NightSessionStart;

        public ulong NightSessionEnd;
    }
}
