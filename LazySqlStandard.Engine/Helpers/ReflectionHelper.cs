using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace LazySql.Engine.Helpers
{
    internal static class ReflectionHelper
    {
        public static IList CreateList(Type type)
        {
            return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
        }

        public static T InvokeStaticGenericMethod<T>(Type typeOfClass, string methodName, Type typeArgument, object[] parameters) => InvokeStaticGenericMethod<T>(typeOfClass, methodName, new[] { typeArgument }, parameters);

        public static T InvokeStaticGenericMethod<T>(Type typeOfClass, string methodName, Type[] typeArguments, object[] parameters)
        {
            MethodInfo method = typeOfClass.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            return (T)method!.MakeGenericMethod(typeArguments).Invoke(null, parameters);
        }
        public static T InvokeStaticMethod<T>(Type typeOfClass, string methodName, object[] parameters)
        {
            MethodInfo method = typeOfClass.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
                throw new NotSupportedException();
            return (T)method.Invoke(null, parameters);
        }

        public static void InvokeStaticMethod(Type typeOfClass, string methodName, object[] parameters)
        {
            MethodInfo method = typeOfClass.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, parameters);
        }
    }
}
