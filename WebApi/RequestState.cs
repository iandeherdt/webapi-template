using System;
using System.Collections.Generic;
using System.Web;

namespace WebApi
{
    public interface IRequestState
    {
        /// <summary>
        /// Stores the object with it's type name as the key. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="something"></param>
        void Store<T>(T something) where T : class;

        void Store<T>(string key, T something) where T : class;

        /// <summary>
        /// Retrieves an object using it's type name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>() where T : class;

        T Get<T>(string key) where T : class;
    }

    public class PerThreadRequestState : IRequestState
    {
        [ThreadStatic]
        private static IDictionary<string, object> _state;

        public static IDictionary<string, object> State
        {
            get { return _state ?? (_state = new Dictionary<string, object>()); }
        }

        public void Store<T>(T something) where T : class
        {
            Store(typeof(T).FullName, something);
        }

        public void Store<T>(string key, T something) where T : class
        {
            lock (State)
            {
                State[key] = something;
            }
        }

        public T Get<T>() where T : class
        {
            return Get<T>(typeof(T).FullName);
        }

        public T Get<T>(string key) where T : class
        {
            lock (State)
            {
                if (State.ContainsKey(key))
                {
                    return State[key] as T;
                }
                return default(T);
            }
        }
    }

    public class PerHttpRequestState : IRequestState
    {
        public void Store<T>(T something) where T : class
        {
            Store(typeof(T).FullName, something);
        }

        public void Store<T>(string key, T something) where T : class
        {
            CheckWithinWebContext();
            HttpContext.Current.Items[key] = something;
        }

        public T Get<T>() where T : class
        {
            return Get<T>(typeof(T).FullName);
        }

        public T Get<T>(string key) where T : class
        {
            CheckWithinWebContext();
            return HttpContext.Current.Items[key] as T;
        }

        private void CheckWithinWebContext()
        {
            if (HttpContext.Current == null)
                throw new NotSupportedException("PerHttpRequestState can only be used within a web context");
        }
    }
}
