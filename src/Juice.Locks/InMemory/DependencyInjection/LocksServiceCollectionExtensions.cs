using Microsoft.Extensions.DependencyInjection;

namespace Juice.Locks.InMemory
{
    public static class LocksServiceCollectionExtensions
    {
        /// <summary>
        /// Add InMemory distributed lock service
        /// <para>NOTE: this is not really distributed locker and it can not be shared between applications</para>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryLock(this IServiceCollection services)
        {
            return services.AddSingleton<IDistributedLock, InMemoryLocker>();
        }
    }
}
