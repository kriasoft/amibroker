// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSource.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin
{
    using System;

    using AmiBroker.Plugin.Models;

    public class DataSource
    {
        public DataSource(string databasePath, IntPtr mainWnd)
        {
            this.DatabasePath = databasePath;
            this.MainWnd = mainWnd;
            this.Broker = Activator.CreateInstance(Type.GetTypeFromProgID("Broker.Application", true));
        }

        public string DatabasePath { get; set; }

        /// <summary>
        /// Gets the pointer to AmiBroker's main window.
        /// </summary>
        public IntPtr MainWnd { get; private set; }

        /// <summary>
        /// Gets AmiBroker's OLE automation object.
        /// </summary>
        public dynamic Broker { get; private set; }

        public Quotation[] GetQuotes(string ticker, Periodicity periodicity, int limit, Quotation[] lastQuotes = null)
        {
            // TODO: Return the list of quotes for the specified ticker.

            return new Quotation[] { };
        }
    }
}
