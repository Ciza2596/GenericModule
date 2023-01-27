using System;

namespace DataType
{
	[UnityEngine.Scripting.Preserve]
	public abstract class ES3ObjectType : DataType
	{
		public ES3ObjectType(Type type) : base(type) {}

		protected abstract void WriteObject(object obj, IWriter writer);
		protected abstract object ReadObject<T>(IReader reader);

		protected virtual void ReadObject<T>(IReader reader, object obj)
		{
			throw new NotSupportedException("ReadInto is not supported for type "+type);
		}

		public override void Write(object obj, IWriter writer)
		{
            if (!WriteUsingDerivedType(obj, writer))
            {
                var baseType = ES3Reflection.BaseType(obj.GetType());
                if (baseType != typeof(object))
                {
                    var es3Type = ES3TypeMgr.GetOrCreateES3Type(baseType, false);
                    // If it's a Dictionary or Collection, we need to write it as a field with a property name.
                    if (es3Type != null && (es3Type.isDictionary || es3Type.isCollection))
                        writer.WriteProperty("_Values", obj, es3Type);
                }

                WriteObject(obj, writer);
            }
        }

		public override object Read<T>(IReader reader)
		{
			string propertyName;
			while(true)
			{
				propertyName = ReadPropertyName(reader);

				if(propertyName == DataType.TYPE_FIELD_NAME)
					return ES3TypeMgr.GetOrCreateES3Type(reader.ReadType()).Read<T>(reader);
				else
				{
					reader.OverridePropertiesName = propertyName;

					return ReadObject<T>(reader);
				}
			}
		}

		public override void ReadInto<T>(IReader reader, object obj)
		{
			string propertyName;
			while(true)
			{
				propertyName = ReadPropertyName(reader);

				if(propertyName == TYPE_FIELD_NAME)
				{
					ES3TypeMgr.GetOrCreateES3Type(reader.ReadType()).ReadInto<T>(reader, obj);
					return;
				}
                // This is important we return if the enumerator returns null, otherwise we will encounter an endless cycle.
                else if (propertyName == null)
					return;
				else
				{
					reader.overridePropertiesName = propertyName;
					ReadObject<T>(reader, obj);
				}
			}
		}
	}
}