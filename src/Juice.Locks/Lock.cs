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

        public event EventHandler<LockEventArgs> Released;


        #region IDisposable Support


        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //  dispose managed state (managed objects).
                    try
                    {
                        if (_locker.ReleaseLock(this))
                        {
                            IsReleased = true;
                        }
                    }
                    catch { }
                    try
                    {
                        var handler = Released;
                        if (handler != null)
                        {
                            handler(this, new LockEventArgs { Key = Key, Issuer = Value });
                        }
                    }
                    catch { }
                }
                Interlocked.Decrement(ref globalCounter);
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
