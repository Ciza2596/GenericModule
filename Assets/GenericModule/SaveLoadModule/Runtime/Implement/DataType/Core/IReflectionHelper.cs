using System;
using System.Reflection;

namespace DataType
{
    public interface IReflectionHelper
    {
        void Initialize(IDataTypeController dataTypeController);
        
        Object CreateInstance(Type type);
        Object CreateInstance(Type type, params object[] args);
        
        
        Array CreateArrayInstance(Type type, int length);
        Array CreateArrayInstance(Type type, int[] dimensions);

        
        Type MakeGenericType(Type type, Type genericArgs);

        MethodInfo[] GetMethods(Type type, string methodName);
        bool CheckIsEnum(Type type);
        bool CheckIsImplementsInterface(Type type, Type interfaceType);
        bool CheckIsArray(Type type);
        bool CheckIsGenericType(Type type);
        bool CheckIsAssignableFrom(Type a, Type b);

        
        int GetArrayRank(Type type);
        Type GetGenericTypeDefinition(Type type);
        string GetTypeName(Type type);

        Type GetType(string typeString);

        Type[] GetElementTypes(Type type);
        Type[] GetGenericArguments(Type type);
        Type GetBaseType(Type type);
        IProperty[] AddSerializableProperties(Type type, string[] propertyNames);
    }
}