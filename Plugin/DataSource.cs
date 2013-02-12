// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSource.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin
{
    using System;

    public class DataSource
    {
        public DataSource(string databasePath, IntPtr mainWnd)
        {
            this.DatabasePath = databasePath;
            this.MainWnd = mainWnd;
            this.Broker = Activator.CreateInstance(Type.GetTypeFromProgID("Broker.Application", true));
        }

        public string DatabasePath { get; set; }

        public IntPtr MainWnd { get; private set; }

        public dynamic Broker { get; private set; }
    }
}
