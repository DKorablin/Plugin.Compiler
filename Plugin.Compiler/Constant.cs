using System;

namespace Plugin.Compiler
{
	internal static class Constant
	{
		public static class PluginMessage
		{
			/// <summary>Number of arguments passed from the source plugin for execution, which are used to initialize the compiler</summary>
			public const Int32 DynamicMethodArgsLength = 2;
			/// <summary>Execute a dynamic method</summary>
			public const String InvokeDynamicMethod = "InvokeDynamicMethod";
		}

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

			public static class BatFramework
			{
				/// <summary>Template for launching a wrapper in a script file</summary>
				public const String ProgramMainArg4 = @"
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
					: String.Join(",", Array.ConvertAll(argumentsType, arg => arg + " a" + (index++)));

				return template
					.Replace("{Using}", namespaces)//Pluggable namespaces
					.Replace("{Namespace}", Constant.Code.NamespaceName)//The namespace in which the assembly is created
					.Replace("{ClassName}", className)//Prefix of the created class
					.Replace("{MethodName}", Constant.Code.MethodName)//The name of the method to be called
					.Replace("{ReturnType}", returnType == null ? "void" : returnType)
					.Replace("{ArgumentsType}", arguments)
					.Replace("{SourceCode}", sourceCode);//Custom source code
			}
		}
		#endregion CodeTemplate

		public enum SupportedLanguage
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