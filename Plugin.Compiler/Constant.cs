using System;

namespace Plugin.Compiler
{
	internal static class Constant
	{
		public static class PluginMessage
		{
			/// <summary>Кол-во аргуметов передаваемых из исходного плагина на выполнение, которые используются для инициализации компилятора</summary>
			public const Int32 DynamicMethodArgsLength = 2;
			/// <summary>Выполнить динамический метод</summary>
			public const String InvokeDynamicMethod = "InvokeDynamicMethod";
		}
		/// <summary>Версия компиляции по умолчанию</summary>
		public const String DefaultCompilerVersion = "v2.0";
		public const String SettingsFileName = "Plugin.Compiler.xml";
		public const String NamespaceTemplateArgs1 = @"using {0};";

		#region CodeTemplate
		public static class Code
		{
			public const String CS = @"{Using}

namespace {Namespace}
{
	public class {ClassName}
	{
		public {ReturnType} {MethodName}({ArgumentsType})
		{
// Method Begin
{SourceCode}
// Method End
		}
	}
}";
			public const String VB = @"{Using}

Namespace {Namespace}
	Public Class {ClassName}
		Public Function {MethodName}(ByVal ParamArray args As Object()) As Object
' Method Begin
{SourceCode}
' Method End
		End Function
	End Class
End Namespace";
			public const String MCPP = @"{Using}

namespace {Namespace}
{
	public __gc class {ClassName}
	{
		public: Object __gc* {MethodName}(params Object __gc* args __gc[])
		{
// Method Begin
{SourceCode}
// Method End
		}
	};
}
";

			public const String VJ = @"{Using}

public static class {Namespace}
{
	public class {ClassName}
	{
		public Object {MethodName}(params Object[] args)
		{
// Method Begin
{SourceCode}
// Method End
		}
	}
}";

			public static class Batch
			{
				/// <summary>Заголовок bat файла для динамической компиляции</summary>
				public const String CompilerArgs1 = @"
/*
@echo off && cls
set WinDirNet=%WinDir%\Microsoft.NET\Framework
IF EXIST ""%WinDirNet%\v2.0.50727\csc.exe"" set csc=""%WinDirNet%\v2.0.50727\csc.exe""
IF EXIST ""%WinDirNet%\v3.5\csc.exe"" set csc=""%WinDirNet%\v3.5\csc.exe""
IF EXIST ""%WinDirNet%\v4.0.30319\csc.exe"" set csc=""%WinDirNet%\v4.0.30319\csc.exe""
%csc% /nologo {Assemblies} /out:""%~0.exe"" %0
""%~0.exe""
del ""%~0.exe""
PAUSE
exit
*/
";
				/// <summary>Шаблон запуска обёртки в bat файле</summary>
				public const String RunnerArg4 = @"
class Program
{
	static void Main(String[] args) { new {Namespace}.{ClassName}().{MethodName}(args); }
}";
			}

			public const String NamespaceName = "Plugin.Compiler.Runtime";
			public const String MethodName = "PluginMain";

			public static String FormatCodeTemplate(String template, String namespaces, String className, String sourceCode, String returnType, String[] argumentsType)
			{
				Int32 index = 0;
				String arguments = argumentsType == null
					? "params Object[] args"
					: String.Join(",", Array.ConvertAll(argumentsType, delegate(String arg) { return arg + " a" + (index++); }));

				return template
					.Replace("{Using}", namespaces)//Подключаемые пространства имён
					.Replace("{Namespace}", Constant.Code.NamespaceName)//Пространство имён в котором создаётся сборка
					.Replace("{ClassName}", className)//Префикс создаваемого класса
					.Replace("{MethodName}", Constant.Code.MethodName)//Наименование метода, который будет вызван
					.Replace("{ReturnType}", returnType == null ? "void" : returnType)
					.Replace("{ArgumentsType}", arguments)
					.Replace("{SourceCode}", sourceCode);//Пользовательский исходный код
			}
		}
		#endregion CodeTemplate

		public enum SupportedLanguage : Int32
		{
			/// <summary>C#</summary>
			CS,
			/// <summary>VB.NET</summary>
			VB,
			/// <summary>Managed C++</summary>
			MCPP,
			/// <summary>Visual Java#</summary>
			VJ,
		}
	}
}