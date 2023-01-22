

using System;
using System.Collections;

namespace DataTypeManager
{
    public abstract class DataType
    {
        public const string typeFieldName = "__type";

		public ES3Member[] members;
		public Type type;
		public bool isPrimitive = false;
		public bool isValueType = false;
		public bool isCollection = false;
		public bool isDictionary = false;
        public bool isTuple = false;
        public bool isEnum = false;
		public bool isES3TypeUnityObject = false;
		public bool isReflectedType = false;
		public bool isUnsupported = false;
        public int priority = 0;

		protected DataType(Type type)
		{
			DataTypeManager.Add(type, this);
			this.type = type;
			this.isValueType = ES3Reflection.IsValueType(type);
		}

		public abstract void Write(object obj, IWriter writer);
		public abstract object Read<T>(IReader reader);

		public virtual void ReadInto<T>(IReader reader, object obj)
		{
			throw new NotImplementedException("Self-assigning Read is not implemented or supported on this type.");
		}

		protected bool WriteUsingDerivedType(object obj, IWriter writer)
		{
			var objType = obj.GetType();
				
			if(objType != this.type)
			{
				writer.WriteType(objType);
				DataTypeManager.GetOrCreateDataType(objType).Write(obj, writer);
				return true;
			}
			return false;
		}

		protected void ReadUsingDerivedType<T>(IReader reader, object obj)
		{
			DataTypeManager.GetOrCreateDataType(reader.ReadType()).ReadInto<T>(reader, obj);
		}

		internal string ReadPropertyName(IReader reader)
		{
			if(reader.overridePropertiesName != null)
			{
				string propertyName = reader.overridePropertiesName;
				reader.overridePropertiesName = null;
				return propertyName;
			}
			return reader.ReadPropertyName();
		}
	
		#region Reflection Methods

		protected void WriteProperties(object obj, IWriter writer)
		{
			if(members == null)
				GetMembers(writer.settings.safeReflection);
			for(int i=0; i<members.Length; i++)
			{
				var property = members[i];
				writer.WriteProperty(property.name, property.reflectedMember.GetValue(obj), DataTypeManager.GetOrCreateDataType(property.type), writer.settings.memberReferenceMode);
			}
		}

		protected object ReadProperties(IReader reader, object obj)
		{
            // Iterate through each property in the file and try to load it using the appropriate
            // ES3Member in the members array.
            foreach (string propertyName in reader.Properties)
			{
				// Find the property.
				ES3Member property = null;
				for(int i=0; i<members.Length; i++)
				{
					if(members[i].name == propertyName)
					{
						property = members[i];
						break;
					}
				}

                // If this is a class which derives directly from a Collection, we need to load it's dictionary first.
                if(propertyName == "_Values")
                {
                    var baseType = DataTypeManager.GetOrCreateDataType(ES3Reflection.BaseType(obj.GetType()));
                    if(baseType.isDictionary)
                    {
                        var dict = (IDictionary)obj;
                        var loaded = (IDictionary)baseType.Read<IDictionary>(reader);
                        foreach (DictionaryEntry kvp in loaded)
                            dict[kvp.Key] = kvp.Value;
                    }
                    else if(baseType.isCollection)
                    {
                        var loaded = (IEnumerable)baseType.Read<IEnumerable>(reader);

                        var type = baseType.GetType();

                        if (type == typeof(ES3ListType))
                            foreach (var item in loaded)
                                ((IList)obj).Add(item);
                        else if (type == typeof(ES3QueueType))
                        {
                            var method = baseType.type.GetMethod("Enqueue");
                            foreach (var item in loaded)
                                method.Invoke(obj, new object[] { item });
                        }
                        else if (type == typeof(ES3StackType))
                        {
                            var method = baseType.type.GetMethod("Push");
                            foreach (var item in loaded)
                                method.Invoke(obj, new object[] { item });
                        }
                        else if (type == typeof(ES3HashSetType))
                        {
                            var method = baseType.type.GetMethod("Add");
                            foreach (var item in loaded)
                                method.Invoke(obj, new object[] { item });
                        }
                    }
                }

                if (property == null)
					reader.Skip();
				else
				{
					var type = DataTypeManager.GetOrCreateDataType(property.type);

					if(ES3Reflection.IsAssignableFrom(typeof(DictionaryDataType), type.GetType()))
						property.reflectedMember.SetValue(obj, ((DictionaryDataType)type).Read(reader));
					else if(ES3Reflection.IsAssignableFrom(typeof(CollectionDataType), type.GetType()))
						property.reflectedMember.SetValue(obj, ((CollectionDataType)type).Read(reader));
					else
					{
						object readObj = reader.Read<object>(type);
						property.reflectedMember.SetValue(obj, readObj);
					}
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
			var serializedMembers = ES3Reflection.GetSerializableMembers(type, safe, memberNames);
			members = new ES3Member[serializedMembers.Length];
			for(int i=0; i<serializedMembers.Length; i++)
				members[i] = new ES3Member(serializedMembers[i]);
		}

		#endregion
    }
}
