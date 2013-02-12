// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginNotification.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System;
    using System.Runtime.InteropServices;

    public enum PluginNotificationReason
    {
        DatabaseLoaded = 1,
        DatabaseUnloaded = 2,
        SettingsChange = 4,
        StatusRightClick = 8
    }

    /// <summary>
    /// PluginNotification structure is filled up by AmiBroker and passed to plugin as an argument of Notify() call.
    /// PluginNotificationReason property describes the "reason" of notification, that could be the fact that database
    /// is loaded, unloaded, settings are changed or user has clicked with right mouse button over plugin status area
    /// of AmiBroker status bar. This last value (StatusRightClick) is used to display context menu that can offer some
    /// choices to the user like "Connect", "Disconnect", etc.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PluginNotification
    {
        public int StructSize;                  // int

        public PluginNotificationReason Reason; // int

        internal IntPtr DatabasePath;             // LPCTSTR

        internal IntPtr MainWnd;                  // HWND

        internal IntPtr StockInfoFormat4;         // struct StockInfoFormat4*

        public unsafe Workspace* Workspace;     // struct _Workspace*

        public unsafe StockInfo* StockInfo;     // struct StockInfo*
    }
}
