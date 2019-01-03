using System;

namespace IpBlocker.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static T To<T>(this object obj)
        {
            if (obj == null)
            {
                return default(T);
            }

            if (typeof(T) == obj.GetType())
            {
                return (T) obj;
            }

            var str = Convert.ToString(obj);

            return str.To<T>();
        }
    }
}