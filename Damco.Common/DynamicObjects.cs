//Utilities to generate Types at runtime.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{

    /// <summary>
    /// Acts as a template to create the Custom attribute for Properties.
    /// </summary>
    public class NewCustomAttribute
    {
        public ConstructorInfo Constructor { get; set; }
        public object[] ConstructorValues { get; set; } = new object[] { };
        public PropertyInfo[] NamedProperties { get; set; }
        public object[] PropertyValues { get; set; }
    }

    /// <summary>
    /// Acts as template to create the Custom property.
    /// </summary>
    /// <remarks>
    /// Test description.
    /// </remarks>
    public class NewPropertyInfo
    {
        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public IEnumerable<NewCustomAttribute> CustomAttributes { get; set; }
    }

    /// <summary>
    /// Acts as Utility to generate Types at runtime. 
    /// Properties and attributes can also be added during runtime.
    /// </summary>

    public static class DynamicObjects
    {
        /// <summary>
        /// Method is used to create the new type on runtime using the input parameters.
        /// Firstly it creates generalize Assembly name "Damco.Common.DynamicObjects" in 
        /// which new types can be added on runtime based upong the input parameter
        /// </summary>
        /// <param name="name"> Name of the New Type</param>
        /// <param name="properties"> Custom Properties and it's attributes which is of type NewPropertyInfo class </param>
        /// <returns>Type which is according to the passed input parameters </returns>

        public static Type CreateNewType(string name, IEnumerable<NewPropertyInfo> properties)
        {
            // Let's start by creating a new assembly
            AssemblyName dynamicAssemblyName = new AssemblyName("Damco.Common.DynamicObjects");
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("Damco.Common.DynamicObjects");

            // Now let's build a new type
            TypeBuilder dynamicAnonymousType = dynamicModule.DefineType(name, TypeAttributes.Public); //, typeof(ValueType));

            // Let's add some fields to the type.
            foreach (var property in properties)
            {
                AddPropertyAndField(dynamicAnonymousType, property);
                //dynamicAnonymousType.DefineField(field.Key, field.Value, FieldAttributes.Private);
            }
            // Return the type to the caller
            return dynamicAnonymousType.CreateType();
        }

        private static void AddPropertyAndField(TypeBuilder typeBuilder, NewPropertyInfo newProperty)
        {
            var fieldName = $"_{newProperty.Name.Substring(0, 1).ToLower()}{newProperty.Name.Substring(1)}";

            FieldBuilder field = typeBuilder.DefineField(fieldName,
                newProperty.PropertyType,
                FieldAttributes.Private);

            // The last argument of DefineProperty is null, because the
            // property has no parameters. (If you don't specify null, you must
            // specify an array of Type objects. For a parameterless property,
            // use an array with no elements: new Type[] {})
            PropertyBuilder property = typeBuilder.DefineProperty(newProperty.Name,
                PropertyAttributes.HasDefault,
                newProperty.PropertyType,
                null);

            if (newProperty.CustomAttributes != null)
                foreach (var newAttribute in newProperty.CustomAttributes)
                {
                    var attributeBuilder = new CustomAttributeBuilder(
                        newAttribute.Constructor, newAttribute.ConstructorValues,
                        newAttribute.NamedProperties ?? new PropertyInfo[] { }, newAttribute.PropertyValues ?? new object[] { }
                    );
                    property.SetCustomAttribute(attributeBuilder);
                }

            // The property set and property get methods require a special
            // set of attributes.
            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            // Define the "get" accessor method for CustomerName.
            MethodBuilder propertyGetter =
                typeBuilder.DefineMethod($"get_{property.Name}",
                    getSetAttr,
                    property.PropertyType,
                    Type.EmptyTypes);

            ILGenerator propertyGetterIL = propertyGetter.GetILGenerator();
            propertyGetterIL.Emit(OpCodes.Ldarg_0);
            propertyGetterIL.Emit(OpCodes.Ldfld, field);
            propertyGetterIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for CustomerName.
            MethodBuilder propertySetter =
                typeBuilder.DefineMethod($"set_{property.Name}",
                    getSetAttr,
                    null,
                    new Type[] { property.PropertyType });

            ILGenerator propertySetterIL = propertySetter.GetILGenerator();
            propertySetterIL.Emit(OpCodes.Ldarg_0);
            propertySetterIL.Emit(OpCodes.Ldarg_1);
            propertySetterIL.Emit(OpCodes.Stfld, field);
            propertySetterIL.Emit(OpCodes.Ret);

            // Last, we must map the two methods created above to our PropertyBuilder to 
            // their corresponding behaviors, "get" and "set" respectively. 
            property.SetGetMethod(propertyGetter);
            property.SetSetMethod(propertySetter);
        }


      
        public static IQueryable SelectDynamicResult<TSource>(this IQueryable<TSource> source, LambdaExpression selector)
        {
            MethodInfo selectMethod = typeof(System.Linq.Queryable).GetMethods().First(method => method.Name == "Select" && method.GetParameters().Length == 2);
            MethodInfo madeSelectMethod = selectMethod.MakeGenericMethod(typeof(TSource), selector.ReturnType);
            return (IQueryable)madeSelectMethod.Invoke(null, new object[] { source, selector });
        }
    }
}
