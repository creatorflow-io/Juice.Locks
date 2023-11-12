using Microsoft.Extensions.DependencyInjection;

namespace Juice.Locks.Redis
{
    public static class LocksServiceCollectionExtensions
    {
        /// <summary>
        /// Add Redis distributed lock service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedLock(this IServiceCollection services, Action<RedisOptions> configure)
        {
            services.Configure<RedisOptions>(configure);
            return services.AddSingleton<IDistributedLock, RedLock>();
        }
    }
}
