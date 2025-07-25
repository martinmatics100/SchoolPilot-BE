//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using StackExchange.Redis.Extensions.Core;
//using StackExchange.Redis.Extensions.Core.Abstractions;
//using StackExchange.Redis.Extensions.Core.Configuration;
//using StackExchange.Redis.Extensions.Core.Implementations;
//using StackExchange.Redis.Extensions.Newtonsoft;

//public static class RedisServiceModule
//{
//    public static IServiceCollection AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
//    {
//        var environment = configuration["App:Environment"];
//        var isLocal = environment == "Local";

//        var redisConfig = new RedisConfiguration
//        {
//            PoolSize = 20,
//            Hosts = new[] { new RedisHost {
//                Host = configuration["Redis:Host"],
//                Port = int.Parse(configuration["Redis:Port"])
//            }},
//            ConnectTimeout = 5000,
//            Database = 0
//        };

//        if (!isLocal)
//        {
//            redisConfig.Password = configuration["Redis:Password"];
//            redisConfig.User = configuration["Redis:Username"];
//        }

//        services.AddSingleton(redisConfig);
//        services.AddSingleton<ISerializer, NewtonsoftSerializer>();
//        services.AddSingleton<IRedisConnectionPoolManager, RedisConnectionPoolManager>(); 
//        services.AddSingleton<IRedisClient, RedisClient>();
//        services.AddSingleton<IRedisDatabase>(provider =>
//            provider.GetRequiredService<IRedisClient>().GetDefaultDatabase());

//        return services;
//    }
//}