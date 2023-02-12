using System;

namespace DataType
{
    [UnityEngine.Scripting.Preserve]
    public abstract class ObjectType : DataType
    {
        public ObjectType(Type type, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper) : base(
            type, dataTypeController, reflectionHelper)
        {
        }

        protected abstract void WriteObject(object obj, IWriter writer);
        protected abstract object ReadObject<T>(IReader reader);

        protected virtual void ReadObject<T>(IReader reader, object obj)
        {
            throw new NotSupportedException($"[ObjectType::eadObject] ReadInto is not supported for type {Type}.");
        }

        public override void Write(object obj, IWriter writer)
        {
            if (!WriteUsingDerivedType(obj, writer))
            {
                var type = _reflectionHelper.GetBaseType(obj.GetType());
                if (type != typeof(object))
                {
                    var dataType = _dataTypeController.GetOrCreateDataType(type);
                    // If it's a Dictionary or Collection, we need to write it as a field with a property name.
                    if (dataType != null && (dataType.IsDictionary || dataType.IsCollection))
                        writer.WriteProperty("_Values", obj, dataType);
                }

                WriteObject(obj, writer);
            }
        }

        public override object Read<T>(IReader reader)
        {
            while (true)
            {
                var propertyName = ReadPropertyName(reader);

                if (propertyName == TYPE_TAG)
                    return _dataTypeController.GetOrCreateDataType(reader.ReadType()).Read<T>(reader);

                reader.SetOverridePropertyName(propertyName);
                return ReadObject<T>(reader);
            }
        }

        public override void ReadInto<T>(IReader reader, object obj)
        {
            while (true)
            {
                var propertyName = ReadPropertyName(reader);

                if (propertyName == TYPE_TAG)
                {
                    var readType = reader.ReadType();
                    var dataType = _dataTypeController.GetOrCreateDataType(readType);
                    dataType.ReadInto<T>(reader, obj);
                    return;
                }

                // This is important we return if the enumerator returns null, otherwise we will encounter an endless cycle.
                if (propertyName is null)
                    return;

                reader.SetOverridePropertyName(propertyName);
                ReadObject<T>(reader, obj);
            }
        }
    }
}