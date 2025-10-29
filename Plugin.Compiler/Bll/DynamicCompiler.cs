using System;
#if NETFRAMEWORK
using System.CodeDom.Compiler;
#endif
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AlphaOmega.Reflection;
using Microsoft.Win32;
using SAL.Flatbed;
#if !NETFRAMEWORK
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
#endif

namespace Plugin.Compiler.Bll
{
	internal class DynamicCompiler
	{
		private String _returnType;
		private String _className;
		private static CompiledAssemblyLinks AssemblyLinks = new CompiledAssemblyLinks();
		private String _compilerVersion;

		/// <summary>List of references and namespaces that will be added to the source code</summary>
		public AssemblyCollection References { get; internal set; }

		/// <summary>Name of the plugin that is added to the source code</summary>
		public String ClassName
		{
			get => this._className;
			internal set => this._className = String.IsNullOrEmpty(value)
				? "Undefined"
				: value.Trim().Replace(' ', '_').Replace('.', '_');
		}

		/// <summary>Full name of the called method</summary>
		public String FullMethodDescription
		{
			get
			{
				String returnName = this.ReturnType == null ? "void" : this.ReturnType;
				Int32 index = 0;
				String arguments = this.ArgumentsType == null
					? "params Object[] args"
					: String.Join(",", Array.ConvertAll(this.ArgumentsType, delegate(String arg) { return arg + " a" + (index++); }));
				return returnName + " " + this.ClassName + "(" + arguments + ")";
			}
		}

		/// <summary>Input arguments</summary>
		public String[] ArgumentsType { get; set; }

		/// <summary>The result of the method execution</summary>
		public String ReturnType
		{
			get => this._returnType;
			set =>this._returnType = value == null || value == "System.Void"
					? null
					: value;
		}

		/// <summary>Source code for compilation</summary>
		public String SourceCode { get; set; }

		/// <summary>Path to the file where the assembly is compiled</summary>
		/// <remarks>TODO: This parameter is not currently used</remarks>
		public String CompiledAssemblyFilePath { get; set; }

		/// <summary>Version to compile the assembly with</summary>
		/// <remarks>By default, v2.0 is used</remarks>
		public String CompilerVersion
		{
			get => this._compilerVersion ?? Constant.DefaultCompilerVersion;
			set => this._compilerVersion = value;
		}

		/// <summary>Include debug information in the build</summary>
		public Boolean IsIncludeDebugInfo { get; set; }

		/// <summary>Identifier of the language in which the code is written</summary>
		public Int32 LanguageId { get; set; }

		/// <summary>Creating a class instance</summary>
		public DynamicCompiler()
			: this(null)
		{ }

		/// <summary>Creating a class instance with a specified class start prefix</summary>
		/// <param name="pluginName">Name of the plugin for which the code is compiled</param>
		public DynamicCompiler(IPluginDescription pluginDescription)
		{
			this.ClassName = pluginDescription == null ? "Undefined" : String.Join("_", pluginDescription.ID.Split('-'));
			this.References = new AssemblyCollection();
		}

		#region Methods
		/// <summary>Get an array of installed framework versions</summary>
		/// <returns>Array of installed version numbers</returns>
		public static IEnumerable<String> GetFrameworkVersions()
		{
			RegistryKey componentsKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Net Framework Setup\NDP\");
			foreach(String keyName in componentsKey.GetSubKeyNames())
				if(keyName.StartsWith("v") && keyName.Contains("."))
					if(keyName.Split('.').Length > 2)
						yield return keyName.Substring(0, keyName.LastIndexOf('.'));
					else
						yield return keyName;
		}

#if NETFRAMEWORK
		/// <summary>Get a list of supported compilers</summary>
		public CompilerInfo[] GetSupportedCompilers()
			=> CodeDomProvider.GetAllCompilerInfo();

		/// <summary>Get a supported compiler</summary>
		public CompilerInfo GetSupportedCompiler(Int32 index)
		{
			CompilerInfo[] compilers = this.GetSupportedCompilers();
			return compilers.Length > index ? compilers[index] : null;
		}

		/// <summary>Get the supported language from the compiler information</summary>
		/// <param name="compiler">Compiler information</param>
		/// <returns>Language name from the compiler information</returns>
		public String GetSupportedLanguage(CompilerInfo info)
		{
			String[] names = info.GetLanguages();
			Int32 length = names.Length;
			return length > 0 ? names[length - 1] : String.Empty;
		}

		/// <summary>Get a list of supported languages</summary>
		/// <returns>Array of languages ​​supported by the compiler</returns>
		public IEnumerable<String> GetSupportedLanguages()
		{
			CompilerInfo[] languages = this.GetSupportedCompilers();
			foreach(var language in languages)
			{
				String[] names = language.GetLanguages();
				if(names.Length > 0)
					yield return names[names.Length - 1];
			}
		}
#else
		// Roslyn only supports C# here.
		public object[] GetSupportedCompilers() => new object[0];
		public object GetSupportedCompiler(Int32 index) => null;
		public String GetSupportedLanguage(object _) => "CS";
		public IEnumerable<String> GetSupportedLanguages() { yield return "CS"; }
#endif

		/// <summary>Compile the assembly</summary>
		/// <returns>Resulting assembly</returns>
		public Assembly CompileAssembly()
		{//TODO: Not thread safe
			Assembly result;
#if NETFRAMEWORK
			CompilerInfo info = this.GetSupportedCompiler(this.LanguageId);
#else
			object info = null;
#endif
			String sourceCode = this.GetSourceCode();

			result = DynamicCompiler.AssemblyLinks.TryGet(sourceCode);
			if(result == null)
			{
#if NETFRAMEWORK
				result = this.CompileAssembly(info as CompilerInfo, sourceCode);
#else
				result = this.CompileAssemblyRoslyn(sourceCode);
#endif
				DynamicCompiler.AssemblyLinks.Add(sourceCode, result);
			}
			return result;
		}

		/// <summary>Save the source code as a batch file</summary>
		public virtual String GetBatchCode()
		{
			List<String> references = new List<String>();

			foreach(String assembly in this.References)
				if(File.Exists(assembly))//Physical file
					references.Add("/reference=\"" + assembly + '\"');

			String batchHeader = Constant.Code.Batch.CompilerArgs1.Replace("{Assemblies}", String.Join(" ", references.ToArray()));

			String sourceCode = this.GetSourceCode();
			return batchHeader + sourceCode;
		}

		/// <summary>Compile and run the build</summary>
		/// <typeparam name="T">Execution result</typeparam>
		/// <param name="plugin">Calling plugin</param>
		/// <param name="args">Arguments passed to the code</param>
		/// <returns>Result of executing the dynamically compiled code</returns>
		public T CompileAndInvoke<T>(IPlugin plugin, params Object[] args)
		{
			Assembly assembly = this.CompileAssembly();
			return this.InvokeAssembly<T>(assembly, plugin, args);
		}

		/// <summary>Get source code for compilation</summary>
		protected virtual String GetSourceCode()
			=> this.SourceCode;

#if NETFRAMEWORK
		/// <summary>Compile an assembly from a class written entirely by the user</summary>
		/// <param name="language">The language in which the code is written</param>
		/// <param name="fullCode">The full source code for generating the assembly</param>
		/// <returns>The generated assembly</returns>
		private Assembly CompileAssembly(CompilerInfo info, String sourceCode)
		{
			if(info == null)
				throw new InvalidOperationException("CompilerInfo is null");

			if(!String.IsNullOrEmpty(this.CompilerVersion))
			{//Setting the compiler version. Because of .NET 5+ I have to change this object type from property to field
				IDictionary<String, String> data = (IDictionary<String, String>)info.GetType().InvokeMember("_providerOptions", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, info, null);
				if(data.Count > 0 && data.Keys.Contains("CompilerVersion"))
					data["CompilerVersion"] = this.CompilerVersion;
			}

			using(CodeDomProvider compiler = info.CreateProvider())
			{
				CompilerParameters parameters = info.CreateDefaultCompilerParameters();

				foreach(String assembly in this.References)
					if(File.Exists(assembly))//Physical path
						parameters.ReferencedAssemblies.Add(assembly);
					else
					{
						String path = AssemblyCache.QueryAssemblyInfo(assembly);
						parameters.ReferencedAssemblies.Add(path);
					}

				parameters.GenerateInMemory = true;
				if(this.IsIncludeDebugInfo)
				{
					parameters.IncludeDebugInformation = true;
					parameters.CompilerOptions += "/debug+ /debug:full /optimize-";
				} else
					parameters.CompilerOptions += "/optimize";
				
				parameters.OutputAssembly = this.CompiledAssemblyFilePath;
				parameters.MainClass = Constant.Code.NamespaceName + "." + this.ClassName;

				//TODO: It's not working after .NET 5+. See: https://stackoverflow.com/questions/73793505/system-platformnotsupportedexception-compiling-c-sharp-code-at-runtime-net-core
				CompilerResults result = compiler.CompileAssemblyFromSource(parameters, sourceCode);
				if(result.Errors.HasErrors)
					throw new CompilerException(sourceCode, result);

				return result.CompiledAssembly;
			}
		}
#else
		// Roslyn compilation for .NET 8+
		private Assembly CompileAssemblyRoslyn(String sourceCode)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

			List<MetadataReference> references = new List<MetadataReference>();
			// Add core assemblies
			Assembly[] coreAssemblies = new[]
			{
				typeof(object).Assembly,
				typeof(Console).Assembly,
				typeof(Enumerable).Assembly
			};
			foreach(var asm in coreAssemblies.Distinct())
				references.Add(MetadataReference.CreateFromFile(asm.Location));

			foreach(String assembly in this.References)
			{
				String path = null;
				if(File.Exists(assembly))
					path = Path.GetFullPath(assembly);
				else if(assembly.Contains(","))
				{
					try { path = Assembly.Load(assembly).Location; } catch { }
					if(String.IsNullOrEmpty(path))
					{
						String simpleName = assembly.Substring(0, assembly.IndexOf(','));
						try { path = Assembly.Load(simpleName).Location; } catch { }
					}
				}
				else
				{
					try { path = Assembly.Load(assembly).Location; } catch { }
				}
				if(!String.IsNullOrEmpty(path) && File.Exists(path))
					references.Add(MetadataReference.CreateFromFile(path));
			}

			var compilation = CSharpCompilation.Create(
				this.ClassName + "_" + Guid.NewGuid().ToString("N"),
				new[] { syntaxTree },
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

			using(var ms = new MemoryStream())
			{
				var emitResult = compilation.Emit(ms);
				if(!emitResult.Success)
				{
					throw new CompilerException(sourceCode, emitResult.Diagnostics);
				}
				ms.Position = 0;
				return Assembly.Load(ms.ToArray());
			}
		}
#endif

		/// <summary>Execute the build</summary>
		/// <typeparam name="T">The return type of the method</typeparam>
		/// <param name="host">The host running the dynamic code</param>
		/// <param name="plugin">The plugin that called the code</param>
		/// <param name="args">Arguments passed to the dynamically compiled assembly</param>
		/// <returns>The result of executing the dynamically compiled assembly</returns>
		public T InvokeAssembly<T>(Assembly assembly,IPlugin plugin, params Object[] args)
		{
			String typeName = Constant.Code.NamespaceName + "." + this.ClassName;
			Object objClass = assembly.CreateInstance(typeName)
				?? throw new EntryPointNotFoundException(typeName);

			Type type = objClass.GetType();
			PropertyInfo pluginProperty = type.GetProperty("Plugin");
			if(pluginProperty != null && pluginProperty.CanWrite)
				pluginProperty.SetValue(objClass, plugin, null);

			MethodInfo method = type.GetMethod(Constant.Code.MethodName);
			if(method.GetParameters().Length != args.Length)
				Array.Resize(ref args, method.GetParameters().Length);

			try
			{
				return (T)method.Invoke(objClass, args);
			} catch(TargetParameterCountException)
			{
				String fullMethodName = method.ReturnType.Name + " " + method.Name + "(" + String.Join(", ", Array.ConvertAll(method.GetParameters(), delegate(ParameterInfo parameter) { return parameter.ToString(); })) + ")";
				String argsName = args == null || args.Length == 0
					? "null"
					: String.Join(", ", Array.ConvertAll(args, item => item == null ? "null" : item.GetType().Name + " " + item.ToString()));
				throw new InvalidOperationException($"Method: {fullMethodName} does not expect parameters {argsName}");
			}
		}
		#endregion Methods
	}
}