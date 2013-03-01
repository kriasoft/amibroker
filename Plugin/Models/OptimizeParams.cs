// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptimizeParams.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AmiBroker.Plugin.Models
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct OptimizeParams
    {
        /// <summary>
        /// 0 - gets defaults, 1 - retrieves settings from formula (setup phase), 2 - optimization phase
        /// </summary>
        public int Mode;

        /// <summary>
        /// 0 - none (regular optimization), 1-in-sample, 2 - out of sample
        /// </summary>
        public int WalkForwardMode;

        /// <summary>
        /// Optimization engine selected - 0 means - built-in exhaustive search
        /// </summary>
        public int Engine;

        /// <summary>
        /// Number of variables to optimize
        /// </summary>
        public int Qty;

        public int LastQty;

        /// <summary>
        /// Boolean flag 1 - means optimization can continue, 0 - means aborted by pressing "Cancel" in progress dialog or other error
        /// </summary>
        public int CanContinue;

        /// <summary>
        /// Boolean flag 1 - means that AmiBroker will first check if same parameter set wasn't used already
        /// </summary>
        public int DuplicateCheck;

        /// <summary>
        /// And if duplicate is found it won't run back-test, instead will return previously stored value
        /// </summary>
        public int Reserved;

        /// <summary>
        /// Pointer to info text buffer (providing text display in the progress dialog)
        /// </summary>
        public string InfoText;

        /// <summary>
        /// The size (in bytes) of info text buffer
        /// </summary>
        public int InfoTextSize;

        /// <summary>
        /// Current optimization step (used for progress indicator) - automatically increased with each iteration
        /// </summary>
        public long Step;

        /// <summary>
        /// Total number of optimization steps (used for progress indicator)
        /// </summary>
        public long NumSteps;

        public double TargetCurrent;

        public double TargetBest;

        /// <summary>
        /// Optimization step in which best was achieved
        /// </summary>
        public int TargetBestStep;

        /// <summary>
        /// Parameters to optimize. Size 100.
        /// </summary>
        public IntPtr ItemsPtr;
    }
}
