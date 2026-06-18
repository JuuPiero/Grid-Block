using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static Dictionary<Type, object> _services = new();


    public static void Register(object service)
    {
        _services.Add(service.GetType(), service);
    }
    public static void Register<T>(object service)
    {
        _services.Add(typeof(T), service);
    }
    public static T Get<T>() where T : class
    {
        if(_services.TryGetValue(typeof(T), out var res))
        {
            return res as T;
        }
        return null;
    }

}