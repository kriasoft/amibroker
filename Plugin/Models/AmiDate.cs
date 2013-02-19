// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmiDate.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System;

    internal class AmiDate : IComparable<AmiDate>
    {
        private ulong packedDate;

        public AmiDate(ulong date)
        {
            this.packedDate = date;
        }

        public AmiDate(DateTime date, bool isEOD = false, bool isFuturePad = false)
            : this(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, 0, isEOD: isEOD, isFuturePad: isFuturePad)
        {
        }

        public AmiDate(int year, int month, int day, bool isFuturePad = false)
            : this(year, month, day, 0, 0, 0, 0, 0, isEOD: true, isFuturePad: isFuturePad)
        {
        }

        public AmiDate(int year, int month, int day, int hour, int minute, int second, int millisecond, int microsecond, bool isEOD = false, bool isFuturePad = false)
        {
            if (isEOD)
            {
                // EOD markets
                hour = 31;
                minute = 63;
                second = 63;
                millisecond = 1023;
                microsecond = 1023;
            }

            this.packedDate = (ulong)year << 52 | (ulong)month << 48 | (ulong)day << 43 | (ulong)hour << 38 |
                              (ulong)minute << 32 | (ulong)second << 26 | (ulong)millisecond << 16 |
                              (ulong)microsecond << 6 | Convert.ToUInt64(isFuturePad);
        }

        public int Year
        {
            get { return (int)(this.packedDate >> 52); }
        }

        public int Month
        {
            get { return (int)(this.packedDate >> 48) & 15; }
        }

        public int Day
        {
            get { return (int)(this.packedDate >> 43) & 31; }
        }

        public int Hour
        {
            get { return (int)(this.packedDate >> 38) & 31; }
        }

        public int Minute
        {
            get { return (int)(this.packedDate >> 32) & 63; }
        }

        public int Second
        {
            get { return (int)(this.packedDate >> 26) & 63; }
        }

        public int MilliSecond
        {
            get { return (int)(this.packedDate >> 16) & 1023; }
        }

        public int MicroSecond
        {
            get { return (int)(this.packedDate >> 6) & 1023; }
        }

        public bool IsFuturePad
        {
            get { return ((int)this.packedDate & 1) == 1; }
        }

        public static explicit operator AmiDate(ulong date)
        {
            return new AmiDate(date);
        }

        public static implicit operator ulong(AmiDate date)
        {
            return date.ToUInt64();
        }

        public ulong ToUInt64()
        {
            return this.packedDate;
        }

        public override bool Equals(object obj)
        {
            var date = obj as AmiDate;
            return date != null && this.Year == date.Year && this.Month == date.Month && this.Day == date.Day && this.Hour == date.Hour && this.Minute == date.Minute && this.Second == date.Second && this.MilliSecond == date.MilliSecond && this.MicroSecond == date.MicroSecond;
        }

        public override int GetHashCode()
        {
            return this.packedDate.GetHashCode();
        }

        public int CompareTo(AmiDate other)
        {
            return new DateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second, this.MilliSecond)
                .CompareTo(new DateTime(other.Year, other.Month, other.Day, other.Hour, other.Minute, other.Second, other.MilliSecond));
        }
    }
}
