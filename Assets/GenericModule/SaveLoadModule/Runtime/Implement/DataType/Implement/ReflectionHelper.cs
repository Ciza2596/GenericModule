

using System;
using System.Reflection;

namespace DataType
{
    public class ReflectionHelper: IReflectionHelper
    {
        public object CreateInstance(Type type) => throw new NotImplementedException();

        public object CreateInstance(Type type, params object[] args) => throw new NotImplementedException();

        public Array CreateArrayInstance(Type type, int length) => throw new NotImplementedException();

        public Array CreateArrayInstance(Type type, int[] dimensions) => throw new NotImplementedException();

        public Type MakeGenericType(Type type, Type genericParam) => throw new NotImplementedException();

        public MethodInfo[] GetMethods(Type type, string methodName) => throw new NotImplementedException();
        public bool CheckIsEnum(Type type) => throw new NotImplementedException();

        public bool CheckIsImplementsInterface(Type type, Type interfaceType) => throw new NotImplementedException();

        public bool CheckIsArray(Type type) => throw new NotImplementedException();

        public bool CheckIsGenericType(Type type) => throw new NotImplementedException();
        int IReflectionHelper.GetArrayRank(Type type) => throw new NotImplementedException();

        public Type GetGenericTypeDefinition(Type type) => throw new NotImplementedException();

        public void GetArrayRank(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
