using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task Expire(string key)
        {
            _values.Remove(key);
            await Task.CompletedTask;
        }

        public async Task<T> Get<T>(string key) where T : class
        {
            if (!await Has(key))
            {
                throw new Exception("Cache data missing");
            }

            var item = _values[key];

            Type type = item.Value.GetType();
            Type typeT = typeof(T);

            if (type != typeT && !typeT.IsAssignableFrom(type))
            {
                throw new Exception("Cache data type mismatch");
            }

            return item.Value as T;
        }

        public async Task<bool> Has(string key)
        {
            if (!_values.ContainsKey(key)) return false;

            if (DateTime.Now >= _values[key].Expiration)
            {
                await Expire(key);
                return false;
            }

            return true;
        }

        public async Task Set<T>(string key, T value) where T : class
        {
            await Set(key, value, _defaultExpirationTime);
        }
        public async Task Set<T>(string key, T value, TimeSpan expiration) where T : class
        {
            _values.Add(key, new GraphInterfaceMemoryCacheItem
            {
                Value = value,
                Expiration = DateTime.Now.Add(expiration)
            });

            await Task.CompletedTask;
        }
    }
}