// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Periodicity.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    public enum Periodicity
    {
        OneSecond = 1,
        OneMinute = 60,
        FiveMinutes = 300,
        FifteenMinutes = 900,
        OneHour = 3600,
        EndOfDay = 86400
    }
}
