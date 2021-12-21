using System;

namespace GraphInterface.Interfaces
{
    public interface IGraphInterfaceCacheService<T> where T : class
    {
        public T Get();
        public void Set(T value);
        public void Set(T value, TimeSpan expiration);
        public void Expire();
        public bool Has();
    }
}