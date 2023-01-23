using System;
using System.Reflection;

namespace DataTypeManager
{
    public interface IReflectionHelper
    {
        Object CreateInstance(Type type);
        Object CreateInstance(Type type, params object[] args);
        
        
        Array CreateArrayInstance(Type type, int length);
        Array CreateArrayInstance(Type type, int[] dimensions);

        
        Type MakeGenericType(Type type, Type genericParam);

        MethodInfo[] GetMethods(Type type, string methodName);
    }
}