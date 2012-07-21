using System.Threading;

namespace Tazqon.Commons.Components
{
    class SafeInteger
    {
        /// <summary>
        /// The main-counter as int.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// An object to handle the thread-safety.
        /// </summary>
        public ReaderWriterLockSlim Lock { get; private set; }

        /// <summary>
        /// Enabled the thread-locker.
        /// </summary>
        public bool Wrapper { get; private set; }

        public SafeInteger(int Index, bool Wrapper)
        {
            this.Index = Index;
            this.Lock = new ReaderWriterLockSlim();
            this.Wrapper = Wrapper;
        }

        /// <summary>
        /// Get the next integer with thread-safe locking.
        /// </summary>
        /// <returns></returns>
        public int Push()
        {
            if (Wrapper)
            Lock.EnterWriteLock();

            int Output = Index++;

            if (Wrapper)
            Lock.ExitWriteLock();

            return Output;
        }
    }
}
