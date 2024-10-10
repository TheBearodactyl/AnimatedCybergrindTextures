using System;
using System.Reflection;

namespace AnimatedCybergrindTextures.Utils
{
    public static class ReflectionUtils
    {
        private const BindingFlags BindingFlagsFields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        public static object GetPrivate<T>(T instance, Type classType, string field)
        {
            var privateField = classType.GetField(field, BindingFlagsFields);
            return privateField != null ? privateField.GetValue(instance) : null;
        }
    }
}