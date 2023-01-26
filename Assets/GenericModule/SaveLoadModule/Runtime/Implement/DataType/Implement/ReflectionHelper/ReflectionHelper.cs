using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DataType.Implement
{
    public class ReflectionHelper : IReflectionHelper
    {
        //private variable

        private readonly Type _serializeFieldAttributeType = typeof(SerializeField);
        private readonly Type _serializableAttributeType;
        private readonly Type _nonSerializableAttributeType;


        public ReflectionHelper(IReflectionHelperInstaller reflectionHelperInstaller)
        {
            _serializableAttributeType = reflectionHelperInstaller.SerializableAttributeType;
            _nonSerializableAttributeType = reflectionHelperInstaller.NonSerializableAttributeType;
        }



        public System.Object CreateInstance(Type type) => Activator.CreateInstance(type);

        public System.Object CreateInstance(Type type, params object[] args) => Activator.CreateInstance(type, args);

        public Array CreateArrayInstance(Type type, int length) => Array.CreateInstance(type, new int[] { length });

        public Array CreateArrayInstance(Type type, int[] dimensions) => Array.CreateInstance(type, dimensions);

        public Type MakeGenericType(Type type, Type genericArgs) => type.MakeGenericType(genericArgs);

        public MethodInfo[] GetMethods(Type type, string methodName) => type.GetMethods().Where(t => t.Name == methodName).ToArray();
        public bool CheckIsEnum(Type type) => type.IsEnum;

        public bool CheckIsImplementsInterface(Type type, Type interfaceType) => (type.GetInterface(interfaceType.Name) != null);

        public bool CheckIsArray(Type type) => type.IsArray;

        public bool CheckIsGenericType(Type type) => type.IsGenericType;
        public int GetArrayRank(Type type) => type.GetArrayRank();

        public Type GetGenericTypeDefinition(Type type) => type.GetGenericTypeDefinition();

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

        public Type[] GetElementTypes(Type type)
        {
            if (CheckIsGenericType(type))
                return GetGenericArguments(type);
            
            if (type.IsArray)
                return new Type[] { GetElementType(type) };
            
            return null;
        }

        public Type[] GetGenericArguments(Type type) => type.GetGenericArguments();
        
        
        //private method
        private Type GetElementType(Type type) => type.GetElementType();

    }
}