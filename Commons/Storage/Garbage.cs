using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Tazqon.Commons.Storage
{
    class Garbage
    {
        /// <summary>
        /// Sets the Applications maximal workinsetsize.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="minimumWorkingSetSize"></param>
        /// <param name="maximumWorkingSetSize"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

        /// <summary>
        /// Timer that calls the GC.Collect each X times.
        /// </summary>
        public Timer CollectorTimer { get; private set; }

        /// <summary>
        /// Handler of timer.
        /// </summary>
        public Garbage()
        {
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);

            CollectorTimer = new Timer(HandleTimer, CollectorTimer, 0, 60000);
        }

        /// <summary>
        /// Optimized our Memory usage.
        /// </summary>
        /// <param name="e"></param>
        private void HandleTimer(object e)
        {
            GC.Collect();
        }
    }
}
