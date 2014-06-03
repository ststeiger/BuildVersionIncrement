using System;
using System.Collections.Generic;
using System.Reflection;
namespace Qreed.Reflection
{
	public static class ReflectionHelper
	{
		public static T GetAssemblyAttribute<T>(Assembly assembly)
		{
			return (T)((object)ReflectionHelper.GetAssemblyAttribute(typeof(T), assembly));
		}
		public static object GetAssemblyAttribute(Type attributeType, Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			object[] customAttributes = assembly.GetCustomAttributes(attributeType, true);
			object result;
			if (customAttributes == null || customAttributes.Length == 0)
			{
				result = null;
			}
			else
			{
				result = customAttributes[0];
			}
			return result;
		}
		public static List<Type> GetTypesThatDeriveFromType(Assembly asm, Type baseType, bool includeSelf, bool includeAbstract)
		{
			if (asm == null)
			{
				throw new ArgumentNullException("asm", "No assembly given");
			}
			Type[] types = asm.GetTypes();
			if (types == null || types.Length == 0)
			{
				throw new Exception("Failed getting types from assembly: " + asm.FullName);
			}
			List<Type> list = new List<Type>();
			Type[] array = types;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				if (type != baseType || includeSelf)
				{
					if (baseType.IsAssignableFrom(type))
					{
						if (includeAbstract || !type.IsAbstract)
						{
							list.Add(type);
						}
					}
				}
			}
			return list;
		}
	}
}
