// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginStatus.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System.Runtime.InteropServices;

    public enum StatusCode
    {
        OK,
        Wait,
        Error,
        Unknown
    }


    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PluginStatus
    {
        public int StructSize;

        public int StatusCode;

        public ColorRef StatusColor;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string LongMessage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string ShortMessage;
    }
}
