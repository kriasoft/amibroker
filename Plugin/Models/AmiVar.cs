// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmiVar.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System.Runtime.InteropServices;

    [StructLayoutAttribute(LayoutKind.Sequential, Size = 8)]
    public unsafe struct AmiVar
    {
        public int Type;

        [StructLayoutAttribute(LayoutKind.Explicit, Size = 4)]
        public unsafe struct Val
        {
            [FieldOffset(0)]
            public float Value;

            [FieldOffset(0)]
            public float* Array;

            [FieldOffset(0)]
            public char* String;

            [FieldOffset(0)]
            public void* Disp;
        }
    }
}
