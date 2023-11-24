using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Juice.Locks.InMemory
{
    internal class InMemoryLocker : IDistributedLock
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, Lock> _locks = new ConcurrentDictionary<string, Lock>();
        public InMemoryLocker(ILogger<InMemoryLocker> logger)
        {
            _logger = logger;
        }
        public ILock? AcquireLock(string key, string issuer, TimeSpan expiration)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                var flag = _locks.TryAdd(key, new Lock(this, key, issuer ?? "", DateTimeOffset.Now.Add(expiration)));
                if (flag || _locks[key].Value == issuer)
                {
                    return _locks[key];
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Acquire lock fail...{ex.Message}");
            }
            return default;
        }
        public Task<ILock?> AcquireLockAsync(string key, string issuer, TimeSpan expiration)
        {
            return Task.FromResult(AcquireLock(key, issuer, expiration));
        }
        public bool ReleaseLock(ILock @lock)
        {
            try
            {
                if (@lock == null)
                {
                    throw new ArgumentNullException(nameof(@lock));
                }
                if (_locks.TryRemove(@lock.Key, out var lockItem))
                {
                    @lock.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Release lock fail...{ex.Message}");
            }
            return false;
        }
        public Task<bool> ReleaseLockAsync(ILock @lock)
        {
            return Task.FromResult(ReleaseLock(@lock));
        }
    }
}
