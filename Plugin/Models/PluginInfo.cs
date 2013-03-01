// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginInfo.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// PluginInfo structure holds general information about plugin
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PluginInfo
    {
        /// <summary>
        /// This is sizeof(struct PluginInfo). <c>int</c>
        /// </summary>
        public int StructSize;

        /// <summary>
        /// Plug-in type currently 1 - indicator is the only one supported. <c>int</c>
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public PluginType Type;

        /// <summary>
        /// Plug-in version coded to <c>int</c> as MAJOR * 10000 + MINOR * 100 + RELEASE. <c>int</c>
        /// </summary>
        public int Version;

        /// <summary>
        /// ID code used to uniquely identify the data feed (set it to zero for AFL plugins). <c>int</c>
        /// </summary>
        public int IDCode;

        /// <summary>
        /// Long name of plug-in displayed in the Plugin dialog. <c>char[64]</c>
        /// </summary>
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Name;

        /// <summary>
        /// Name of the plug-in vendor. <c>char[64]</c>
        /// </summary>
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Vendor;

        /// <summary>
        /// Certificate code - set it to zero for private plug-ins. <c>int</c>
        /// </summary>
        public int Certificate;

        /// <summary>
        /// Minimum required AmiBroker version (should be >= 380000 -> AmiBroker 3.8). <c>int</c>
        /// </summary>
        public int MinAmiVersion;
    }
}
