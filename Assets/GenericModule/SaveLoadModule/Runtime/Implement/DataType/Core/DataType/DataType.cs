using System;
using System.Collections;

namespace DataType
{
    public abstract class DataType
    {
        //protected variable
        protected readonly IDataTypeController _dataTypeController;
        protected readonly IReflectionHelper _reflectionHelper;

        protected IProperty[] _properties;

        //public variable

        public const string TYPE_TAG = "@type";
        public Type Type { get; }
        public bool IsPrimitive { get; protected set; } = false;
        public bool IsCollection { get; protected set; } = false;
        public bool IsDictionary { get; protected set; } = false;
        public bool IsEnum { get; protected set; } = false;
        public bool IsUnsupported { get; protected set; }


        //constructor
        protected DataType(Type type, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper)
        {
            Type = type;
            _dataTypeController = dataTypeController;
            _reflectionHelper = reflectionHelper;
        }

        //public method
        public abstract void Write(object obj, IWriter writer);
        public abstract object Read<T>(IReader reader);

        public virtual void ReadInto<T>(IReader reader, object obj)
        {
            throw new NotImplementedException("Self-assigning Read is not implemented or supported on this type.");
        }

        protected bool WriteUsingDerivedType(object obj, IWriter writer)
        {
            var type = obj.GetType();

            if (type != Type)
            {
                writer.WriteType(type);
                var dataType = _dataTypeController.GetOrCreateDataType(type);
                dataType.Write(obj, writer);
                return true;
            }

            return false;
        }

        protected string ReadPropertyName(IReader reader)
        {
            if (reader.OverridePropertyName != null)
            {
                string propertyName = reader.OverridePropertyName;
                reader.SetOverridePropertyName(null);
                return propertyName;
            }

            return reader.ReadPropertyName();
        }

        protected void WriteProperties(object obj, IWriter writer)
        {
            if (_properties == null)
                GetProperties();

            foreach (var property in _properties)
            {
                var name = property.Name;

                var value = property.GetValue(obj);

                var type = property.Type;
                var dataType = _dataTypeController.GetOrCreateDataType(type);

                writer.WriteProperty(name, value, dataType);
            }
        }

        protected object ReadProperties(IReader reader, object obj)
        {
            // Iterate through each property in the file and try to load it using the appropriate
            // ES3Member in the members array.
            
            foreach (string propertyName in reader.PropertyNames)
            {
                // Find the property.
                IProperty property = null;

                foreach (var dataTypeProperty in _properties)
                {
                    if (dataTypeProperty.Name == propertyName)
                    {
                        property = dataTypeProperty;
                        break;
                    }
                }

                // If this is a class which derives directly from a Collection, we need to load it's dictionary first.
                if (propertyName == "_Values")
                {
                    var baseType = _reflectionHelper.GetBaseType(obj.GetType());
                    var baseDataType = _dataTypeController.GetOrCreateDataType(baseType);
                    if (baseDataType.IsDictionary)
                    {
                        var dict = (IDictionary)obj;
                        var loaded = (IDictionary)baseDataType.Read<IDictionary>(reader);
                        foreach (DictionaryEntry kvp in loaded)
                            dict[kvp.Key] = kvp.Value;
                    }
                    else if (baseDataType.IsCollection)
                    {
                        var loaded = (IEnumerable)baseDataType.Read<IEnumerable>(reader);

                        var type = baseDataType.GetType();

                        if (type == typeof(ListDataType))
                            foreach (var item in loaded)
                                ((IList)obj).Add(item);
                        else if (type == typeof(QueueDataType))
                        {
                            var method = baseDataType.Type.GetMethod("Enqueue");
                            foreach (var item in loaded)
                                method.Invoke(obj, new object[] { item });
                        }
                        else if (type == typeof(StackDataType))
                        {
                            var method = baseDataType.Type.GetMethod("Push");
                            foreach (var item in loaded)
                                method.Invoke(obj, new object[] { item });
                        }
                        else if (type == typeof(HashSetDataType))
                        {
                            var method = baseDataType.Type.GetMethod("Add");
                            foreach (var item in loaded)
                                method.Invoke(obj, new object[] { item });
                        }
                    }
                }

                if (property == null)
                    reader.Skip();
                else
                {
                    var dataType = _dataTypeController.GetOrCreateDataType(property.Type);

                    if (_reflectionHelper.CheckIsAssignableFrom(typeof(DictionaryDataType), dataType.GetType()))
                        property.SetValue(obj, ((DictionaryDataType)dataType).Read(reader));
                    else if (_reflectionHelper.CheckIsAssignableFrom(typeof(CollectionDataType), dataType.GetType()))
                        property.SetValue(obj, ((CollectionDataType)dataType).Read(reader));
                    else
                    {
                        var readObj = reader.Read<object>(dataType);
                        property.SetValue(obj, readObj);
                    }
                }
            }

            return obj;
        }

        protected void GetProperties(string[] propertyNames = null) =>
            _properties = _reflectionHelper.AddSerializableProperties(Type, propertyNames);
    }
}