// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Quotation.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 40-bytes 8-byte aligned
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, Size = 40)]
    public struct Quotation
    {
        /*[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]*/
        public ulong DateTime;
        public float Price;
        public float Open;
        public float High;
        public float Low;
        public float Volume;
        public float OpenInterest;
        public float AuxData1;
        public float AuxData2;
    }
}
