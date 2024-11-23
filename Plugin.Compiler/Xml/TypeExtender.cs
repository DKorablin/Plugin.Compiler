using System;

namespace Plugin.Compiler.Xml
{
	internal static class TypeExtender
	{
		public static Type GetRealType(this Type type)
		{
			if(type.IsGenericType)
			{
				Type genericType = type.GetGenericTypeDefinition();
				if(genericType == typeof(System.Nullable<>)
					|| genericType == typeof(System.Collections.Generic.IEnumerator<>)
					|| genericType == typeof(System.Collections.Generic.IEnumerable<>))
					return type.GetGenericArguments()[0].GetRealType();
			}
			if(type.HasElementType)
				//if(type.BaseType == typeof(Array))//+Для out и ref параметров
				return type.GetElementType().GetRealType();
			return type;
		}
		/// <summary>Базовый тип по которому возможны основные манипуляции</summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Boolean IsConvertible(this Type type)
			=> type.GetInterface("IConvertible") != null;
	}
}