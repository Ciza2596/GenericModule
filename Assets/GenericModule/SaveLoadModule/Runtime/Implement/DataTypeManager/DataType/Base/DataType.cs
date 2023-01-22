using System;
using System.Collections;

namespace DataTypeManager
{
    public abstract class DataType
    {
        //protected variable
        protected IProperty[] Properties { get; private set; }

        //public variable
        
        public Type Type { get; }
        public bool IsCollection { get; protected set; } = false;
        public bool IsDictionary { get; protected set; } = false;
        public int Priority { get; protected set; } = 0;

        
        //public method
        public DataType(Type type)
        {
            DataTypeManager.Add(type, this);
            Type = type;
        }

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
                DataTypeManager.GetOrCreateDataType(type).Write(obj, writer);
                return true;
            }

            return false;
        }

        protected void ReadUsingDerivedType<T>(IReader reader, object obj)
        {
            DataTypeManager.GetOrCreateDataType(reader.ReadType()).ReadInto<T>(reader, obj);
        }

        internal string ReadPropertyName(IReader reader) => reader.ReadPropertyName();
        // {
        //     if (reader.OverridePropertiesName != null)
        //     {
        //         string propertyName = reader.OverridePropertiesName;
        //         reader.OverridePropertiesName = null;
        //         return propertyName;
        //     }
        //
        //     return reader.ReadPropertyName();
        // }

        #region Reflection Methods

        protected void WriteProperties(object obj, IWriter writer)
        {
            // if (Properties == null)
            //     GetMembers(writer.settings.safeReflection);

            foreach (var property in Properties)
            {
                writer.WriteProperty(property.Name, property.GetValue(obj), DataTypeManager.GetOrCreateDataType(property.Type));
            }
            
            // for (int i = 0; i < Properties.Length; i++)
            // {
            //     var property = Properties[i];
            //     writer.WriteProperty(property.Name, property.GetValue(obj),
            //         DataTypeManager.GetOrCreateDataType(property.Type), writer.settings.memberReferenceMode);
            // }
        }

        protected object ReadProperties(IReader reader, object obj)
        {
            // Iterate through each property in the file and try to load it using the appropriate
            // ES3Member in the members array.
            foreach (string propertyName in reader.Properties)
            {
                // Find the property.
                IProperty property = null;
                for (int i = 0; i < Properties.Length; i++)
                {
                    if (Properties[i].Name == propertyName)
                    {
                        property = Properties[i];
                        break;
                    }
                }

                // If this is a class which derives directly from a Collection, we need to load it's dictionary first.
                if (propertyName == "_Values")
                {
                    var baseType = new EnumDataType(null); //DataTypeManager.GetOrCreateDataType(ES3Reflection.BaseType(obj.GetType()));
                    if (baseType.IsDictionary)
                    {
                        var dict = (IDictionary)obj;
                        var loaded = (IDictionary)baseType.Read<IDictionary>(reader);
                        foreach (DictionaryEntry kvp in loaded)
                            dict[kvp.Key] = kvp.Value;
                    }
                    else if (baseType.IsCollection)
                    {
                        var loaded = (IEnumerable)baseType.Read<IEnumerable>(reader);

                        var type = baseType.GetType();

                        if (type == typeof(ListDataType))
                            foreach (var item in loaded)
                                ((IList)obj).Add(item);
                        else if (type == typeof(QueueDataType))
                        {
                            var method = baseType.Type.GetMethod("Enqueue");
                            foreach (var item in loaded)
                                method.Invoke(obj, new object[] { item });
                        }
                        else if (type == typeof(StackDataType))
                        {
                            var method = baseType.Type.GetMethod("Push");
                            foreach (var item in loaded)
                                method.Invoke(obj, new object[] { item });
                        }
                        else if (type == typeof(HashSetDataType))
                        {
                            var method = baseType.Type.GetMethod("Add");
                            foreach (var item in loaded)
                                method.Invoke(obj, new object[] { item });
                        }
                    }
                }

                if (property == null)
                    reader.Skip();
                else
                {
                    // var type = DataTypeManager.GetOrCreateDataType(property.type);
                    //
                    // if (ES3Reflection.IsAssignableFrom(typeof(DictionaryDataType), type.GetType()))
                    //     property.reflectedMember.SetValue(obj, ((DictionaryDataType)type).Read(reader));
                    // else if (ES3Reflection.IsAssignableFrom(typeof(CollectionDataType), type.GetType()))
                    //     property.reflectedMember.SetValue(obj, ((CollectionDataType)type).Read(reader));
                    // else
                    // {
                    //     object readObj = reader.Read<object>(type);
                    //     property.reflectedMember.SetValue(obj, readObj);
                    // }
                }
            }

            return obj;
        }

        protected void GetMembers(bool safe)
        {
            GetMembers(safe, null);
        }

        protected void GetMembers(bool safe, string[] memberNames)
        {
            // var serializedMembers = ES3Reflection.GetSerializableMembers(_type, safe, memberNames);
            // members = new IMember[serializedMembers.Length];
            // for (int i = 0; i < serializedMembers.Length; i++)
            //     members[i] = new ES3Member(serializedMembers[i]);
        }

        #endregion
    }
}