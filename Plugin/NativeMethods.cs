// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        /// <summary>
        /// Sends the specified message to a window or windows. It calls the window procedure for the specified window and does not return until the window procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">AmiBroker's main window handler.</param>
        /// <param name="Msg">The message code.</param>
        /// <param name="wParam">An optional parameter 1 to be passed to AmiBroker.</param>
        /// <param name="lParam">An optional parameter 2 to be passed to AmiBroker.</param>
        /// <returns>The code number.</returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}
