using System;
using System.Reflection;

namespace DataTypeManager
{
    public interface IReflectionHelper
    {
        Array CreateArrayInstance(Type type, int[] dimensions);

        Object CreateInstance(Type type);

        Object CreateInstance(Type type, params object[] args);
        
        Type MakeGenericType(Type type, Type genericParam);

        MethodInfo[] GetMethods(Type type, string methodName);
    }
}