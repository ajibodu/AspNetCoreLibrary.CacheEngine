using CacheEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CacheEngine.Interface
{
    public interface IRedis
    {
        bool Contains(string key, string name, int database = 0);
        void Add(string key, string name, object data, int expiration, ExpireType expireType = ExpireType.Minute, int database = 0);
        object Get(string key, string name, int database = 0);
        T Get<T>(string key, string name, int database = 0);
        void Remove(string key, string name, int database = 0);
    }
}
