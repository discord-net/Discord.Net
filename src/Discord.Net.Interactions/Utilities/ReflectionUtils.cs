using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal static class ReflectionUtils<T>
    {
        private static readonly TypeInfo ObjectTypeInfo = typeof(object).GetTypeInfo();
        internal static T CreateObject(TypeInfo typeInfo, InteractionService commandService, IServiceProvider services = null) =>
            CreateBuilder(typeInfo, commandService)(services);

        internal static Func<IServiceProvider, T> CreateBuilder(TypeInfo typeInfo, InteractionService commandService)
        {
            var constructor = GetConstructor(typeInfo);
            var parameters = constructor.GetParameters();
            var properties = GetProperties(typeInfo);

            return (services) =>
            {
                var args = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    args[i] = GetMember(commandService, services, parameters[i].ParameterType, typeInfo);

                var obj = InvokeConstructor(constructor, args, typeInfo);
                foreach (var property in properties)
                    property.SetValue(obj, GetMember(commandService, services, property.PropertyType, typeInfo));
                return obj;
            };
        }

        private static T InvokeConstructor(ConstructorInfo constructor, object[] args, TypeInfo ownerType)
        {
            try
            {
                return (T)constructor.Invoke(args);
            }
            catch (Exception ex)
            {
                throw new TargetInvocationException($"Failed to create \"{ownerType.FullName}\".", ex);
            }
        }
        private static ConstructorInfo GetConstructor(TypeInfo ownerType)
        {
            var constructors = ownerType.DeclaredConstructors.Where(x => !x.IsStatic).ToArray();
            if (constructors.Length == 0)
                throw new MissingMethodException($"No constructor found for \"{ownerType.FullName}\".");
            else if (constructors.Length > 1)
                throw new InvalidOperationException($"Multiple constructors found for \"{ownerType.FullName}\".");
            return constructors[0];
        }
        private static PropertyInfo[] GetProperties(TypeInfo ownerType)
        {
            var result = new List<PropertyInfo>();
            while (ownerType != ObjectTypeInfo)
            {
                foreach (var prop in ownerType.DeclaredProperties)
                {
                    if (prop.SetMethod?.IsStatic == false && prop.SetMethod?.IsPublic == true)
                        result.Add(prop);
                }
                ownerType = ownerType.BaseType.GetTypeInfo();
            }
            return result.ToArray();
        }
        private static object GetMember(InteractionService commandService, IServiceProvider services, Type memberType, TypeInfo ownerType)
        {
            if (memberType == typeof(InteractionService))
                return commandService;
            if (memberType == typeof(IServiceProvider) || memberType == services.GetType())
                return services;
            var service = services.GetService(memberType);
            if (service != null)
                return service;
            throw new InvalidOperationException($"Failed to create \"{ownerType.FullName}\", dependency \"{memberType.Name}\" was not found.");
        }

        internal static Func<T, object[], Task> CreateMethodInvoker(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            var paramsExp = new Expression[parameters.Length];

            var instanceExp = Expression.Parameter(typeof(T), "instance");
            var argsExp = Expression.Parameter(typeof(object[]), "args");

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                var indexExp = Expression.Constant(i);
                var accessExp = Expression.ArrayIndex(argsExp, indexExp);
                paramsExp[i] = Expression.Convert(accessExp, parameter.ParameterType);
            }

            var callExp = Expression.Call(Expression.Convert(instanceExp, methodInfo.ReflectedType), methodInfo, paramsExp);
            var finalExp = Expression.Convert(callExp, typeof(Task));
            var lambda = Expression.Lambda<Func<T, object[], Task>>(finalExp, instanceExp, argsExp).Compile();

            return lambda;
        }

        /// <summary>
        /// Create a type initializer using compiled lambda expressions
        /// </summary>
        internal static Func<IServiceProvider, T> CreateLambdaBuilder(TypeInfo typeInfo, InteractionService commandService)
        {
            var constructor = GetConstructor(typeInfo);
            var parameters = constructor.GetParameters();
            var properties = GetProperties(typeInfo);

            var lambda = CreateLambdaMemberInit(typeInfo, constructor);

            return (services) =>
            {
                var args = new object[parameters.Length];
                var props = new object[properties.Length];

                for (int i = 0; i < parameters.Length; i++)
                    args[i] = GetMember(commandService, services, parameters[i].ParameterType, typeInfo);

                for (int i = 0; i < properties.Length; i++)
                    props[i] = GetMember(commandService, services, properties[i].PropertyType, typeInfo);

                var instance = lambda(args, props);

                return instance;
            };
        }

        internal static Func<object[], T> CreateLambdaConstructorInvoker(TypeInfo typeInfo)
        {
            var constructor = GetConstructor(typeInfo);
            var parameters = constructor.GetParameters();

            var argsExp = Expression.Parameter(typeof(object[]), "args");

            var parameterExps = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var indexExp = Expression.Constant(i);
                var accessExp = Expression.ArrayIndex(argsExp, indexExp);
                parameterExps[i] = Expression.Convert(accessExp, parameters[i].ParameterType);
            }

            var newExp = Expression.New(constructor, parameterExps);

            return Expression.Lambda<Func<object[], T>>(newExp, argsExp).Compile();
        }

        /// <summary>
        ///     Create a compiled lambda property setter.
        /// </summary>
        internal static Action<T, object> CreateLambdaPropertySetter(PropertyInfo propertyInfo)
        {
            var instanceParam = Expression.Parameter(typeof(T), "instance");
            var valueParam = Expression.Parameter(typeof(object), "value");

            var prop = Expression.Property(instanceParam, propertyInfo);
            var assign = Expression.Assign(prop, Expression.Convert(valueParam, propertyInfo.PropertyType));

            return Expression.Lambda<Action<T, object>>(assign, instanceParam, valueParam).Compile();
        }

        internal static Func<T, object> CreateLambdaPropertyGetter(PropertyInfo propertyInfo)
        {
            var instanceParam = Expression.Parameter(typeof(T), "instance");
            var prop = Expression.Property(instanceParam, propertyInfo);
            return Expression.Lambda<Func<T, object>>(prop, instanceParam).Compile();
        }

        internal static Func<T, object> CreateLambdaPropertyGetter(Type type, PropertyInfo propertyInfo)
        {
            var instanceParam = Expression.Parameter(typeof(T), "instance");
            var instanceAccess = Expression.Convert(instanceParam, type);
            var prop = Expression.Property(instanceAccess, propertyInfo);
            return Expression.Lambda<Func<T, object>>(prop, instanceParam).Compile();
        }

        internal static Func<object[], object[], T> CreateLambdaMemberInit(TypeInfo typeInfo, ConstructorInfo constructor, Predicate<PropertyInfo> propertySelect = null)
        {
            propertySelect ??= x => true;

            var parameters = constructor.GetParameters();
            var properties = GetProperties(typeInfo).Where(x => propertySelect(x)).ToArray();

            var argsExp = Expression.Parameter(typeof(object[]), "args");
            var propsExp = Expression.Parameter(typeof(object[]), "props");

            var parameterExps = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var indexExp = Expression.Constant(i);
                var accessExp = Expression.ArrayIndex(argsExp, indexExp);
                parameterExps[i] = Expression.Convert(accessExp, parameters[i].ParameterType);
            }

            var newExp = Expression.New(constructor, parameterExps);

            var memberExps = new MemberAssignment[properties.Length];

            for (var i = 0; i < properties.Length; i++)
            {
                var indexEx = Expression.Constant(i);
                var accessExp = Expression.Convert(Expression.ArrayIndex(propsExp, indexEx), properties[i].PropertyType);
                memberExps[i] = Expression.Bind(properties[i], accessExp);
            }
            var memberInit = Expression.MemberInit(newExp, memberExps);
            var lambda = Expression.Lambda<Func<object[], object[], T>>(memberInit, argsExp, propsExp).Compile();

            return (args, props) =>
            {
                var instance = lambda(args, props);

                return instance;
            };
        }
    }
}
