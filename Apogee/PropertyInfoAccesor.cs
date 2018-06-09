using System.Reflection;

namespace Apogee
{
    public class PropertyInfoAccesor<T>
    {
        private PropertyInfo _propertyInfo;
        private object _instance;

        public PropertyInfoAccesor(PropertyInfo propertyInfo, object instance)
        {
            _propertyInfo = propertyInfo;
            _instance = instance;
        }

        public T Value
        {
            get => (T) _propertyInfo.GetValue(_instance);
            set => _propertyInfo.SetValue(_instance, value);
        }
    }
}