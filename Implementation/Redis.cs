using CacheEngine.Model;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace CacheEngine.Implementation
{
    public class Redis : CacheEngine.Interface.IRedis
    {
        public Redis(string connection)
        {
            RedisConnectorHelper.redisConn = connection;
        }
        public bool Contains(string key, string name, int database = 0)
        {
            Boolean exist = false;
            try
            {
                var db = RedisConnectorHelper.Connection.GetDatabase(database);
                exist = db.KeyExists($"{key}:{name}");
            }
            catch (Exception ex)
            {
                throw;
            }
            return exist;
        }

        public void Add(string key, string name, object data, int expiration, ExpireType expireType = ExpireType.Minute, int database = 0)
        {
            var db = RedisConnectorHelper.Connection.GetDatabase(database);
            String dt = JsonConvert.SerializeObject(data);
            db.StringSet($"{key}:{name}", dt, new TimeSpan(0, getExpirationInMinites(expireType, expiration), 0));
        }

        public object Get(string key, string name, int database = 0)
        {
            var db = RedisConnectorHelper.Connection.GetDatabase(database);
            string data = db.StringGet($"{key}:{name}");
            return data;
        }
        public T Get<T>(string key, string name, int database = 0)
        {
            var db = RedisConnectorHelper.Connection.GetDatabase(database);
            string data = db.StringGet($"{key}:{name}");
            var conv = JsonConvert.DeserializeObject<T>(data);
            return conv;
        }
        public void Remove(string key, string name, int database = 0)
        {
            var db = RedisConnectorHelper.Connection.GetDatabase(database);
            db.KeyDelete($"{key}:{name}");
        }
        private int getExpirationInMinites(ExpireType expireType, int expiration)
        {
            switch (expireType)
            {
                case ExpireType.Minute:
                    return expiration;
                case ExpireType.Hour:
                    return expiration * 60;
                case ExpireType.Day:
                    return expiration * (60 * 24);
                case ExpireType.Year:
                    return expiration * (60 * 24 * 365);
                default:
                    return expiration;
            }
        }

        internal class RedisConnectorHelper
        {
            internal static string redisConn;
            static RedisConnectorHelper()
            {
                RedisConnectorHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                {
                    //var configurationBuilder = new ConfigurationBuilder();
                    //var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                    //configurationBuilder.AddJsonFile(path, false);

                    //var root = configurationBuilder.Build();
                    //var appSetting = root.GetSection("ApplicationSettings:Database:Redis");
                    //var redisConn = appSetting.Value;
                    if (string.IsNullOrEmpty(redisConn))
                    {
                        throw new ArgumentNullException("redisConn", "Redis Connection has not been set");
                    }
                    var options = ConfigurationOptions.Parse(redisConn);
                    options.AbortOnConnectFail = false;
                    return ConnectionMultiplexer.Connect(options);
                });
            }

            private static Lazy<ConnectionMultiplexer> lazyConnection;

            public static ConnectionMultiplexer Connection
            {
                get
                {
                    return lazyConnection.Value;
                }
            }
        }

        
    }
}
