using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyModel;

namespace P7.Core.Reflection
{
    public class TypeHelper<T> : TypeGlobals
    {

        public static bool IsPublicClassType(Type type)
        {
            return IsType(type) && type.IsPublicClass();
        }

        public static bool IsType(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public static bool IsSubclassOf(Type type)
        {
            return (type != null && type.GetTypeInfo().IsClass && type.GetTypeInfo().IsSubclassOf(typeof(T)));
        }

        public static IEnumerable<Type> FindDerivedTypes(Assembly assembly)
        {
            var baseType = typeof(T);
            return assembly.GetTypes().Where(baseType.IsAssignableFrom);
        }

        public static Assembly FindAssembyByName(string name)
        {
            /*
            IEnumerable<AssemblyName> assemblyNames = DependencyContext.Default.GetDefaultAssemblyNames();
            var resultQ = (
              from aName in assemblyNames
                             where System.String.Compare(name, aName.Name, System.StringComparison.OrdinalIgnoreCase) == 0
              select aName);
            var assemblyName = resultQ.FirstOrDefault();
            Assembly assembly = Assembly.Load(assemblyName);
            return assembly;
            */

            var assemblyParts = ApplicationParts.OfType<AssemblyPart>();
            var result = (
               from  assemblyPart in assemblyParts
               let assemblyName = assemblyPart.Assembly.GetName()
               where System.String.Compare(name, assemblyName.Name, System.StringComparison.OrdinalIgnoreCase) == 0
               select assemblyPart.Assembly);
            return result.FirstOrDefault();
        }

        /*
         * [full namespaced path to Class], [name of assembly]

i.e. COA.MEF.Calculator.Globalization.Views.Home, COA.MEF.Calculator.Globalization
         * */
        /// <summary>
        ///  Get type by full name
        /// </summary>
        /// <param name="fullType">[full namespaced path to Class], [name of assembly]  i.e. COA.MEF.Calculator.Globalization.Views.Home, COA.MEF.Calculator.Globalization </param>
        /// <returns></returns>
        public static Type GetTypeByFullName(string fullType)
        {
            var splitFullType = fullType.Split(',');
            var typeName = splitFullType[0].TrimEnd().TrimStart();
            var assemblyName = splitFullType[1].TrimEnd().TrimStart();
            var assembly = FindAssembyByName(assemblyName);
            var type = assembly?.GetType(typeName);
            return type;
        }

        public static IEnumerable<Type> FindTypesInAssembly(Assembly assembly, bool includeSubClass = false)
        {
            // Go through all assemblies referenced by the application and search for types matching a predicate
            IEnumerable<Type> typesSoFar = Type.EmptyTypes;
            Predicate<Type> predicate = TypeHelper<T>.IsType;
            Type[] typesInAsm;
            try
            {
                typesInAsm = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                typesInAsm = ex.Types;
            }
            typesSoFar = typesSoFar.Concat(typesInAsm);

            return
                typesSoFar.Where(
                    type =>
                        type.IsPublicClass() &&
                        (predicate(type) || (!includeSubClass || IsSubclassOf(type))));
        }

        public static IEnumerable<Type> FindTypesInAssembly(Assembly assembly, Predicate<Type> predicate)
        {
            // Go through all assemblies referenced by the application and search for types matching a predicate
            IEnumerable<Type> typesSoFar = Type.EmptyTypes;

            Type[] typesInAsm;
            try
            {
                typesInAsm = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                typesInAsm = ex.Types;
            }
            typesSoFar = typesSoFar.Concat(typesInAsm);

            return typesSoFar.Where(type => predicate(type));
        }


        public static IEnumerable<Type> FindTypesInAssembly2(Assembly assembly)
        {
            Predicate<Type> predicate = TypeHelper<T>.IsType;
            return FindTypesInAssembly2(assembly, predicate);
        }

        public static IEnumerable<Type> FindTypesInAssembly2(Assembly assembly,
            Predicate<Type> predicate)
        {
            // Go through all assemblies referenced by the application and search for types matching a predicate
            IEnumerable<Type> typesSoFar = Type.EmptyTypes;


            Type[] typesInAsm;
            try
            {
                typesInAsm = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                typesInAsm = ex.Types;
            }
            typesSoFar = typesSoFar.Concat(typesInAsm);
            return typesSoFar.Where(type => type.IsPublicClass() && predicate(type));
        }

        public static IEnumerable<Type> FindTypesInAssemblies(IEnumerable<Assembly> assemblies,
            Predicate<Type> predicate)
        {
            // Go through all assemblies referenced by the application and search for types matching a predicate
            IEnumerable<Type> typesSoFar = Type.EmptyTypes;

            foreach (Assembly assembly in assemblies)
            {
                typesSoFar = typesSoFar.Concat(FindTypesInAssembly2(assembly, predicate));
            }
            return typesSoFar;
        }
        public static IEnumerable<Type> FindTypesInAssembliesWithInterfaceImplementation<TInteface>( )
        {
            return FindTypesInAssemblies(TypeHelper<TInteface>.IsType);
        }
        public static IEnumerable<Type> FindTypesInAssemblies(Predicate<Type> predicate)
        {
            var assemblyParts = ApplicationParts.OfType<AssemblyPart>();
            var result = (
               from assemblyPart in assemblyParts
               let assembly = assemblyPart.Assembly
               select assembly);
            return FindTypesInAssemblies(result, predicate);
        }

        public static IEnumerable<Type> FindTypesWithCustomAttribute<TAttributeType>()
        {
            var registrationTypes = FindTypesInAssemblies(IsType).ToList();
            var types = new List<Type>();
            foreach (Type type in registrationTypes)
            {
                if (type.GetTypeInfo().GetCustomAttributes(typeof(TAttributeType), true).Any())
                {
                    types.Add(type);
                }
            }
            return types;
        }
        public static IEnumerable<Type> FindTypesWithCustomAttribute<TAttributeType>(IEnumerable<Type> toBeEvaluatedTypes)
        {
            var types = new List<Type>();
            foreach (Type type in toBeEvaluatedTypes)
            {
                if (type.GetTypeInfo().GetCustomAttributes(typeof(TAttributeType), true).Any())
                {
                    types.Add(type);
                }
            }
            return types;
        }
    }
}