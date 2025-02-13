using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class ReflectionUtils
    {

        public static bool IsSameAs(this MemberInfo member, MemberInfo otherMember)
        {
            //Need to check on declaringtype because if we got it from a different class we have a different ReflectedType
            return member.DeclaringType == otherMember.DeclaringType && member.Name == otherMember.Name;
        }

        public static object CopyMatchingPropertiesToNew(this object source, Type targetType, bool alsoComplexProperties = false, IEnumerable<string> excludedProperties = null)
        {
            return CallDynamic(typeof(ReflectionUtils), nameof(CopyMatchingPropertiesToNew), targetType, source, alsoComplexProperties, excludedProperties);
        }

        public static T CopyMatchingPropertiesToNew<T>(this object source, bool alsoComplexProperties = false, IEnumerable<string> excludedProperties = null)
            where T : new()
        {
            var result = new T();
            if (typeof(T).IsValueType)
            {
                var resultObj = (object)result; //box
                source.CopyMatchingPropertiesTo(resultObj, alsoComplexProperties, excludedProperties);
                result = (T)resultObj; //unbox
            }
            else
                source.CopyMatchingPropertiesTo(result, alsoComplexProperties, excludedProperties);
            return result;
        }

        public static void CopyMatchingPropertiesTo(this object source, object target, bool alsoComplexProperties = false, IEnumerable<string> excludedProperties = null)
        {
            var targetIsValueType = target != null && target.GetType().IsValueType;
            if (source is IDictionary)
            {
                var dictionary = (IDictionary)source;
                var targetProps =
                    target.GetType().GetProperties().Cast<MemberInfo>()
                    .Union(target.GetType().GetFields().Cast<MemberInfo>())
                    .ToDictionary(x => x.Name, x => x);
                foreach (DictionaryEntry prop in dictionary)
                {
                    var propName = prop.Key.ToString();
                    MemberInfo targetMember;
                    if (targetProps.TryGetValue(propName, out targetMember))
                    {
                        if (targetIsValueType) //Note we're assuming "target" argument is boxed beforehand otherwise this won't work
                        {
                            if (targetMember is PropertyInfo targetProperty)
                                targetProperty.SetValue(target, ConvertUtils.ChangeType(prop.Value, targetProperty.PropertyType));
                            else if (targetMember is FieldInfo targetField)
                                targetField.SetValue(target, ConvertUtils.ChangeType(prop.Value, targetField.FieldType));
                        }
                        else
                        {
                            if (targetMember is PropertyInfo targetProperty)
                                targetProperty.GetSetterAction()(target, ConvertUtils.ChangeType(prop.Value, targetProperty.PropertyType));
                            else if (targetMember is FieldInfo targetField)
                                targetField.GetSetterAction()(target, ConvertUtils.ChangeType(prop.Value, targetField.FieldType));
                        }
                    }
                }
            }
            else
                foreach (var propertyMatch in (
                    from s in source.GetType().GetProperties()
                    join t in target.GetType().GetProperties() on s.Name equals t.Name
                    where (alsoComplexProperties || (s.PropertyType.IsSimpleType() && t.PropertyType.IsSimpleType()))
                    && (excludedProperties == null || !excludedProperties.Contains(s.Name))
                    select new { s, t })
                )
                {
                    //TODO: Fields
                    if (targetIsValueType)
                        propertyMatch.t.SetValue(target, ConvertUtils.ChangeType(propertyMatch.s.GetGetterFunc()(source), propertyMatch.t.PropertyType));
                    else
                        propertyMatch.t.GetSetterAction()(target, ConvertUtils.ChangeType(propertyMatch.s.GetGetterFunc()(source), propertyMatch.t.PropertyType));
                }
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsClass)
                return null;
            else
                return Activator.CreateInstance(type);
        }

        public static object CallDynamic(this object container, string methodName, Type typeArgument, params object[] arguments)
        {
            return CallDynamic<object>(container, methodName, typeArgument, arguments);
        }

        public static Tresult CallDynamic<Tresult>(this object container, string methodName, Type typeArgument, params object[] arguments)
        {
            return CallDynamic<Tresult>(container, methodName, typeArgument.ToSingletonCollection(), arguments);
        }
        public static object CallDynamic(this object container, string methodName, IEnumerable<Type> typeArguments, params object[] arguments)
        {
            return CallDynamic<object>(container, methodName, typeArguments, arguments);
        }

        public static MethodInfo FindMethod(this Type containerType, BindingFlags bindingFlags, string methodName, params Type[] parameterTypes)
        {
            return containerType.GetMethods(bindingFlags).Single(x => x.Name == methodName && x.GetParameters().Length == parameterTypes.Length && x.GetParameters().Select((y, n) => y.ParameterType == parameterTypes[n]).All(b => b));
        }

        public static Tresult CallDynamic<Tresult>(this object container, string methodName, IEnumerable<Type> typeArguments, params object[] arguments)
        {
            Type type;
            BindingFlags flags;
            if (container is Type) //Static
            {
                type = (Type)container;
                container = null;
                flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | BindingFlags.FlattenHierarchy;
            }
            else
            {
                type = container.GetType();
                flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            }
            var possibleMethods = type.GetMethods(flags)
                .Where(m => m.Name == methodName
                    && (typeArguments != null && typeArguments.Any()
                        ? m.IsGenericMethod && m.GetGenericArguments().Length == typeArguments.Count()
                        : !m.IsGenericMethod
                        )
                    && m.GetParameters().Length == arguments.Length
                 )
                .ToList();

            MethodInfo methodToUse = null;
            if (possibleMethods.Count() == 1)
                methodToUse = possibleMethods.Single();
            else
            {
                foreach (var method in possibleMethods)
                {
                    MethodInfo genericMethod = null;
                    bool skip = false;
                    if (method.IsGenericMethod)
                    {
                        try
                        {
                            genericMethod = method.MakeGenericMethod(typeArguments.ToArray());
                        }
                        catch (ArgumentException)
                        {
                            skip = true; //Type arguments incorrect
                        }
                    }
                    if (!skip &&
                        (genericMethod ?? method)
                        .GetParameters()
                        .CombineWith(arguments)
                        .All(x => x.Item1.ParameterType.IsAssignableFrom(x.Item2.GetType())))
                    {
                        if (methodToUse != null)
                            throw new AmbiguousMatchException("Multiple matching methods found");
                        methodToUse = method;
                    }
                }
            }

            if (methodToUse == null)
                throw new ArgumentException("No matching method found");

            if (methodToUse.IsGenericMethod)
                methodToUse = methodToUse.MakeGenericMethod(typeArguments.ToArray());

            return (Tresult)methodToUse.Invoke(container, arguments);
        }

        public static IEnumerable<Type> GetAssignableTypes(this Assembly assembly, Type baseOrInterfaceType, bool onlyRealTypes)
        {
            return
                assembly.GetTypes()
                .Where(t =>
                    baseOrInterfaceType.IsAssignableFrom(t)
                    && (!onlyRealTypes || (!t.ContainsGenericParameters && !t.IsAbstract && !t.IsInterface))
                );
        }

        private static bool TypeArgumentsMatch(Type[] first, Type[] second)
        {
            for (int i = 0; i < second.Length; i++)
                if (second[i] != null && first[i] != second[i])
                    return false;
            return true;
        }

        public static IEnumerable<Type> FindMatchingRealTypes(this Assembly assembly, Type genericTypeDefinition, params Type[] typeParameters)
        {
            return
                assembly.GetTypes()
                .Where(t => !t.ContainsGenericParameters && !t.IsAbstract && !t.IsInterface)
                .Where(t =>
                    t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeDefinition && TypeArgumentsMatch(i.GetGenericArguments(), typeParameters))
                ); //TODO: Base classes
        }

        public static IEnumerable<T> GetInstancesOfAll<T>(this Assembly assembly, params object[] constructorParameters)
        {
            return assembly.GetInstancesOfAll<T>(typeof(T), constructorParameters).Cast<T>();
        }

        public static IEnumerable<T> GetInstancesOfAll<T>(this Assembly assembly, Type baseOrInterfaceType, params object[] constructorParameters)
        {
            return
                assembly.GetAssignableTypes(baseOrInterfaceType, true)
                .Select(t =>
                    constructorParameters == null || constructorParameters.Length == 0
                    ? (T)Activator.CreateInstance(t)
                    : (T)Activator.CreateInstance(t, constructorParameters)
                );
        }

    }
}
