using System;
using GraphInterface.Interfaces;

namespace GraphInterface.Services
{
    public class GraphInterfaceMemoryCacheService<T> : IGraphInterfaceCacheService<T> where T : class
    {
        private readonly TimeSpan _defaultExpirationTime;
        private T _value = null;
        private DateTime? _expiration;

        public GraphInterfaceMemoryCacheService(TimeSpan defaultExpirationTime)
        {
            _defaultExpirationTime = defaultExpirationTime;
        }
        public GraphInterfaceMemoryCacheService()
        {
            _defaultExpirationTime = TimeSpan.FromHours(1);
        }
        public void Expire()
        {
            _value = null;
            _expiration = null;
        }

        public T Get()
        {
            if (!Has())
            {
                throw new Exception("Cache data missing");
            }

            return _value;
        }

        public bool Has()
        {
            if (_value == null || _expiration == null) return false;

            if (DateTime.Now >= _expiration.GetValueOrDefault())
            {
                Expire();
                return false;
            }

            return true;
        }

        public void Set(T value)
        {
            _value = value;
            _expiration = DateTime.Now.Add(_defaultExpirationTime);
        }
        public void Set(T value, TimeSpan expiration)
        {
            _value = value;
            _expiration = DateTime.Now.Add(expiration);
        }
    }
}