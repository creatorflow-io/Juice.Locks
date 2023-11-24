using System.Diagnostics;

namespace Juice.Locks
{
    public class Lock : ILock
    {
        protected static int globalCounter = 0;
        private IDistributedLock _locker;
        public Lock(IDistributedLock locker, string key, string value, DateTimeOffset expiresOn)
        {
            _locker = locker;
            Key = key;
            Value = value;
            ExpiresOn = expiresOn;
            Interlocked.Increment(ref globalCounter);
        }
        public string Key { get; init; }
        public string Value { get; init; }

        public DateTimeOffset ExpiresOn { get; init; }

        public bool IsExpired => ExpiresOn <= DateTimeOffset.UtcNow;

        public bool IsReleased { get; private set; }

        public event EventHandler<LockEventArgs>? Released;


        #region IDisposable Support


        private bool disposedValue = false; // To detect redundant calls
        private bool _isReleasing;
        protected virtual void Dispose(bool callFromLocker)
        {
            if (!disposedValue)
            {

                //  dispose managed state (managed objects).
                try
                {
                    if (_isReleasing)
                    {
                        return;
                    }
                    if (!callFromLocker)
                    {
                        _isReleasing = true;
                        if (_locker.ReleaseLock(this))
                        {
                            IsReleased = true;
                        }
                        _isReleasing = false;
                    }
                    else
                    {
                        IsReleased = true;
                    }
                }
                catch { }
                try
                {
                    var handler = Released;
                    if (handler != null && IsReleased)
                    {
                        handler(this, new LockEventArgs { Key = Key, Issuer = Value });
                    }
                    Released = null;
                }
                catch { }
                Interlocked.Decrement(ref globalCounter);
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool callFromLocker) above.
            var callFromLocker = new StackFrame(1, false).GetMethod()?.DeclaringType?.IsAssignableTo(typeof(IDistributedLock)) ?? false;
            Dispose(callFromLocker);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
