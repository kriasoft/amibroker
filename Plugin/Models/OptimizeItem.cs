// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptimizeItem.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System.Runtime.InteropServices;

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct OptimizeItem
    {
        public string Name;

        public float Default;

        public float Min;

        public float Max;

        public float Step;

        public double Current;

        public float Best;
    }
}
