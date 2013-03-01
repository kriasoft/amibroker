// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoSite.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System;
    using System.Runtime.InteropServices;

    public enum InfoSiteCategory
    {
        Market,
        Group,
        Sector,
        Industry,
        Watchlist
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct InfoSite
    {
        public int StructSize;

        public GetStockQtyDelegate GetStockQty;

        public SetCategoryNameDelegate SetCategoryName;

        public GetCategoryNameDelegate GetCategoryName;

        public SetIndustrySectorDelegate SetIndustrySector;

        public GetIndustrySectorDelegate GetIndustrySector;

        public AddStockNewDelegate AddStockNew;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate int GetStockQtyDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate int SetCategoryNameDelegate(int category, int item, string name);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        public delegate string GetCategoryNameDelegate(int category, int item);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate int SetIndustrySectorDelegate(int industry, int sector);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate int GetIndustrySectorDelegate(int industry);

        // Only available if called from AmiBroker 5.27 or higher
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate IntPtr AddStockNewDelegate([MarshalAs(UnmanagedType.LPStr)] string ticker);
    }
}
