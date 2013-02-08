// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginType.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    /// <summary>
    /// Possible types of plugins currently 4 types are defined
    /// </summary>
    public enum PluginType
    {
        /// <summary>
        /// For AFL function plugins
        /// </summary>
        Afl = 1,

        /// <summary>
        /// For data feed plugins (requires 3.81 or higher)
        /// </summary>
        Data = 2,

        /// <summary>
        /// For combined AFL/Data plugins
        /// </summary>
        AflAndData = 3,

        /// <summary>
        /// For optimization engine plugins (requires v5.12 or higher)
        /// </summary>
        Optimized = 4
    }
}
