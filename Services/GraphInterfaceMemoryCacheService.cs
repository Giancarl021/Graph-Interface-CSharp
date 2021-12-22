using System;
using System.Collections.Generic;
using GraphInterface.Interfaces;
using GraphInterface.Models.Helpers;

namespace GraphInterface.Services
{
    public class GraphInterfaceMemoryCacheService : IGraphInterfaceCacheService
    {
        private readonly TimeSpan _defaultExpirationTime;
        private readonly Dictionary<string, GraphInterfaceMemoryCacheItem> _values;

        public GraphInterfaceMemoryCacheService(TimeSpan defaultExpirationTime)
        {
            _defaultExpirationTime = defaultExpirationTime;
            _values = new Dictionary<string, GraphInterfaceMemoryCacheItem>();
        }
        public GraphInterfaceMemoryCacheService() : this(TimeSpan.FromHours(1)) {}
        public void Expire(string key)
        {
            _values.Remove(key);
        }

        public T Get<T>(string key) where T : class
        {
            if (!Has(key))
            {
                throw new Exception("Cache data missing");
            }

            var item = _values[key];

            if (item.Value.GetType() != typeof(T))
            {
                throw new Exception("Cache data type mismatch");
            }

            return item.Value as T;
        }

        public bool Has(string key)
        {
            if (!_values.ContainsKey(key)) return false;

            if (DateTime.Now >= _values[key].Expiration)
            {
                Expire(key);
                return false;
            }

            return true;
        }

        public void Set<T>(string key, T value) where T : class
        {
            Set(key, value, _defaultExpirationTime);
        }
        public void Set<T>(string key, T value, TimeSpan expiration) where T : class
        {
            _values.Add(key, new GraphInterfaceMemoryCacheItem
            {
                Value = value,
                Expiration = DateTime.Now.Add(expiration)
            });
        }
    }
}