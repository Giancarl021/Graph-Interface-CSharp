using System;
using System.Threading.Tasks;

namespace GraphInterface.Interfaces
{
    public interface IGraphInterfaceCacheService
    {
        public Task<T> Get<T>(string key) where T : class;
        public Task Set<T>(string key, T value) where T : class;
        public Task Set<T>(string key, T value, TimeSpan expiration) where T : class;
        public Task Expire(string key);
        public Task<bool> Has(string key);
    }
}