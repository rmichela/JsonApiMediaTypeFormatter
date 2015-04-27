using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonApi
{
    internal class PropertyFieldInfo
    {
        private readonly PropertyInfo _propertyInfo;
        private readonly FieldInfo _fieldInfo;
        private readonly MemberInfo _memberInfo;

        public PropertyFieldInfo(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            _memberInfo = propertyInfo;
        }

        public PropertyFieldInfo(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
            _memberInfo = fieldInfo;
        }

        public string Name { get { return _memberInfo.Name; } }

        public Type OfType
        {
            get { 
                if (_propertyInfo != null) return _propertyInfo.PropertyType;
                if (_fieldInfo != null) return _fieldInfo.FieldType;
                throw new InvalidOperationException();
            }
        }

        public bool IsDefined(Type attributeType, bool inheret)
        {
            return _memberInfo.IsDefined(attributeType, inheret);
        }

        public object GetValue(object obj)
        {
            if (_propertyInfo != null) return _propertyInfo.GetValue(obj);
            if (_fieldInfo != null) return _fieldInfo.GetValue(obj);
            throw new InvalidOperationException();
        }

        public T GetCustomAttribute<T>() where T : Attribute
        {
            return (_propertyInfo != null ? _propertyInfo.GetCustomAttribute<T>() : null) ??
                   (_fieldInfo != null ? _fieldInfo.GetCustomAttribute<T>() : null);
        }
    }
}
