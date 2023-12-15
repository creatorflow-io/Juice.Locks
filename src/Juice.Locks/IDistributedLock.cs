using System.Linq.Expressions;

namespace Juice.Locks
{
    public interface IDistributedLock
    {
        /// <summary>
        /// Try acquire a new lock with the specified key, issuer and expiration.
        /// <para>If the key does not exist or is locked by the same issuer, the lock will be returned, otherwise null</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="issuer"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        ILock? AcquireLock(string key, string issuer, TimeSpan expiration);
        /// <summary>
        /// Try acquire a new lock with the specified key, issuer and expiration.
        /// <para>If the key does not exist or is locked by the same issuer, the lock will be returned, otherwise null</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="issuer"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        Task<ILock?> AcquireLockAsync(string key, string issuer, TimeSpan expiration);
        /// <summary>
        /// Release the specified lock
        /// </summary>
        /// <param name="lock"></param>
        /// <returns></returns>
        bool ReleaseLock(ILock @lock);
        /// <summary>
        /// Release the specified lock
        /// </summary>
        /// <param name="lock"></param>
        /// <returns></returns>
        Task<bool> ReleaseLockAsync(ILock @lock);
        /// <summary>
        /// Release the locks by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ReleaseLockAsync(string key);
    }

    public static class LockerExtensions
    {
        /// <summary>
        /// Try acquire a new lock for the specified object by its key, issuer and expiration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="locker"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <param name="keySelector"></param>
        /// <param name="lockedBy"></param>
        /// <returns></returns>
        public static ILock? AcquireLock<T>(this IDistributedLock locker, T value, TimeSpan expiration,
            Expression<Func<T, object>> keySelector, string lockedBy)
        {
            var key = keySelector.Compile().Invoke(value).ToString() ?? "";
            key = (typeof(T).Name + ":" + key).Trim(':');
            return locker.AcquireLock(key, lockedBy, expiration);
        }

        /// <summary>
        /// Try acquire a new lock for the specified object by its key, issuer and expiration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="locker"></param>
        /// <param name="value"></param>
        /// <param name="expiration"></param>
        /// <param name="keySelector"></param>
        /// <param name="lockedBy"></param>
        /// <returns></returns>
        public static Task<ILock?> AcquireLockAsync<T>(this IDistributedLock locker, T value, TimeSpan expiration,
            Expression<Func<T, object>> keySelector, string lockedBy)
        {
            var key = keySelector.Compile().Invoke(value).ToString() ?? "";
            key = (typeof(T).Name + ":" + key).Trim(':');
            return locker.AcquireLockAsync(key, lockedBy, expiration);
        }
    }
}
