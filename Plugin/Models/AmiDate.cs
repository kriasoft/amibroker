// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmiDate.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System;

    internal class AmiDate
    {
        private ulong date;

        public AmiDate(ulong date)
        {
            this.date = date;
        }

        public AmiDate(DateTime date, bool isEOD = false, bool isFuturePad = false)
            : this(
                date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, 0,
                isEOD: isEOD, isFuturePad: isFuturePad
            )
        {
        }

        public AmiDate(int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond,
                       bool isEOD = false, bool isFuturePad = false)
        {
            if (isEOD)
            {
                // EOD markets
                hour = 31;
                minute = 63;
            }

            this.date = (ulong)year << 52 | (ulong)month << 48 | (ulong)day << 43 | (ulong)hour << 38 |
                        (ulong)minute << 32 | (ulong)second << 26 | (ulong)millisecond << 16 | (ulong)microsecond << 6 |
                        Convert.ToUInt64(isFuturePad);
        }

        public ulong ToUInt64()
        {
            return this.date;
        }

        public static explicit operator AmiDate(ulong date)
        {
            return new AmiDate(date);
        }

        public static implicit operator ulong(AmiDate date)
        {
            return date.ToUInt64();
        }
    }
}
