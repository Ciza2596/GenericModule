using System;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    internal class ReflectedDataType : DataType
    {
        public ReflectedDataType(Type type, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper)
            : base(type, dataTypeController, reflectionHelper)
        {
        }

        public override void Write(object obj, IWriter writer)
        {
            // Manage NULL values.
            if (obj is null)
            {
                writer.WriteNull();
                return;
            }

            // If this is a derived type, write the type as a property and use it's specific ES3Type.
            var objType = obj.GetType();
            if (objType != Type)
            {
                writer.WriteType(objType);
                var dataType = _dataTypeController.GetOrCreateDataType(objType);
                dataType.Write(obj, writer);
                return;
            }

            if (_properties is null)
                GetProperties();

            foreach (var property in _properties)
            {
                var name = property.Name;
                var type = property.Type;
                var value = property.GetValue(obj);

                var dataType = _dataTypeController.GetOrCreateDataType(type);
                writer.WriteProperty(name, value, dataType);
            }
        }

        public override object Read<T>(IReader reader)
        {
            if (_properties is null)
                GetProperties();

            var propertyName = reader.ReadPropertyName();

            // If we're loading a derived type, use it's specific ES3Type.
            if (propertyName == TYPE_TAG)
            {
                var type = reader.ReadType();
                var dataType = _dataTypeController.GetOrCreateDataType(type);

                return dataType.Read<T>(reader);
            }

            reader.SetOverridePropertyName(propertyName);
            
            var obj = _reflectionHelper.CreateInstance(Type);

            // Iterate through each property in the file and try to load it using the appropriate
            // Property in the members array.
            ReadProperties(reader, obj);

            return obj;
        }

        public override void ReadInto<T>(IReader reader, object obj)
        {
            if (_properties is null)
                GetProperties();

            string propertyName = reader.ReadPropertyName();

            // If we're loading a derived type, use it's specific ES3Type.
            if (propertyName == TYPE_TAG)
            {
                var type = reader.ReadType();
                var dataType = _dataTypeController.GetOrCreateDataType(type);
                dataType.ReadInto<T>(reader, obj);
                return;
            }

            reader.SetOverridePropertyName(propertyName);

            // Iterate through each property in the file and try to load it using the appropriate
            // Property in the members array.
            ReadProperties(reader, obj);
        }
    }
}