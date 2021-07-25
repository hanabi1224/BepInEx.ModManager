using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BepInEx.ModManager.Shared
{
    public interface IDisposableObservable : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this object has been disposed.
        /// </summary>
        bool IsDisposed { get; }
    }

    public abstract class DisposableBase : IDisposableObservable
    {
        public bool IsDisposed { get; private set; }

        ~DisposableBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                DisposeResources();
            }

            IsDisposed = true;
        }

        protected abstract void DisposeResources();
    }
}
