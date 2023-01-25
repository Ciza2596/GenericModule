using System;
using System.Reflection;
using UnityEngine;

namespace DataType.Implement
{
    public class ReflectionHelper : IReflectionHelper
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

        public string GetTypeString(Type type)
        {
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(sbyte))
                return "sbyte";
            if (type == typeof(char))
                return "char";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(double))
                return "double";
            if (type == typeof(float))
                return "float";
            if (type == typeof(int))
                return "int";
            if (type == typeof(uint))
                return "uint";
            if (type == typeof(long))
                return "long";
            if (type == typeof(ulong))
                return "ulong";
            if (type == typeof(short))
                return "short";
            if (type == typeof(ushort))
                return "ushort";
            if (type == typeof(string))
                return "string";
            if (type == typeof(Vector2))
                return "Vector2";
            if (type == typeof(Vector3))
                return "Vector3";

            Debug.LogError($"[ReflectionHelper::GetTypeString] Type of {type} doesnt be supported.");
            return string.Empty;
        }

        public Type GetType(string typeString)
        {
            switch (typeString)
            {
                case "bool":
                    return typeof(bool);
                case "byte":
                    return typeof(byte);
                case "sbyte":
                    return typeof(sbyte);
                case "char":
                    return typeof(char);
                case "decimal":
                    return typeof(decimal);
                case "double":
                    return typeof(double);
                case "float":
                    return typeof(float);
                case "int":
                    return typeof(int);
                case "uint":
                    return typeof(uint);
                case "long":
                    return typeof(long);
                case "ulong":
                    return typeof(ulong);
                case "short":
                    return typeof(short);
                case "ushort":
                    return typeof(ushort);
                case "string":
                    return typeof(string);
                case "Vector2":
                    return typeof(Vector2);
                case "Vector3":
                    return typeof(Vector3);
            }
            
            Debug.LogError($"[ReflectionHelper::GetType] TypeString of {typeString} doesnt be supported.");
            return null;
        }

        public Type[] GetElementTypes(Type type) => throw new NotImplementedException();

        public void GetArrayRank(Type type)
        {
            throw new NotImplementedException();
        }
    }
}