using System;

namespace GraphInterface.Interfaces
{
    public interface IGraphInterfaceCacheService
    {
        public T Get<T>(string key) where T : class;
        public void Set<T>(string key, T value) where T : class;
        public void Set<T>(string key, T value, TimeSpan expiration) where T : class;
        public void Expire(string key);
        public bool Has(string key);
    }
}