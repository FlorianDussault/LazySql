namespace LazySql;

/// <summary>
/// Reflection Helper
/// </summary>
internal static class ReflectionHelper
{
    /// <summary>
    /// Create a List<T> object
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IList CreateList(Type type)
    {
        return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
    }

    /// <summary>
    /// Invoke a static method
    /// </summary>
    /// <typeparam name="T">Type of return</typeparam>
    /// <param name="typeOfClass">Type of class</param>
    /// <param name="methodName">Method Name</param>
    /// <param name="parameters">Parameters</param>
    /// <returns>Return from method</returns>
    /// <exception cref="NotSupportedException"></exception>
    public static T InvokeStaticMethod<T>(Type typeOfClass, string methodName, object[] parameters)
    {
        MethodInfo method = typeOfClass.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
            throw new NotSupportedException();
        return (T)method.Invoke(null, parameters);
    }
}