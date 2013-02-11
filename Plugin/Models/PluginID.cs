// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginID.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System;

    /// <summary>
    /// Used to define a unique ID for the plug-in.
    /// </summary>
    internal class PluginID
    {
        private int id;

        public PluginID(string code)
        {
            if (code.Length != 4)
            {
                throw new ArgumentException("Plugin ID code must be 4 characters long.", "code");
            }

            this.id = code[0] << 24 | code[1] << 16 | code[2] << 8 | code[3] << 0;
        }

        public static implicit operator int(PluginID id)
        {
            return id.ToInt32();
        }

        public int ToInt32()
        {
            return this.id;
        }
    }
}
