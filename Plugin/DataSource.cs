// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSource.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;

    using Models;

    public class DataSource
    {
        public DataSource(string databasePath, IntPtr mainWnd)
        {
            this.DatabasePath = databasePath;
            this.MainWnd = mainWnd;
            this.Broker = Activator.CreateInstance(Type.GetTypeFromProgID("Broker.Application", true));

            if (this.Broker.ActiveDocument == null)
            {
                var processes = Process.GetProcesses().Where(x => x.ProcessName.Contains("AmiBroker") && x.MainWindowHandle != this.MainWnd);
                
                foreach (var proc in processes)
                {
                    MessageBox.Show("Please close AmiBroker application with Process ID: " + proc.Id, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    proc.WaitForExit();
                }

                this.Broker = Activator.CreateInstance(Type.GetTypeFromProgID("Broker.Application", true));
            }
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

        public Quotation[] GetQuotes(string ticker, Periodicity periodicity, int limit, Quotation[] existingQuotes)
        {
            // TODO: Return the list of quotes for the specified ticker.
            return new Quotation[] { };
        }
    }
}
