using System.Collections.Generic;
using UnityEngine;

namespace AIMiniGame.Scripts.Framework.Resource {
    public class ResourceCache {
        private Dictionary<string, Object> m_cache = new Dictionary<string, Object>();

        public T Get<T>(string key) where T : Object {
            if (m_cache.TryGetValue(key, out Object asset)) {
                return asset as T;
            }

            return null;
        }

        public void Add(string key, Object asset) {
            m_cache.TryAdd(key, asset);
        }

        public void Remove(string key) {
            if (m_cache.ContainsKey(key)) {
                m_cache.Remove(key);
            }
        }

        public void Clear() {
            m_cache.Clear();
        }
    }
}