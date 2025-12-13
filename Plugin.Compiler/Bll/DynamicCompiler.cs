using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SAL.Flatbed;
using System.Linq;
using AlphaOmega.Reflection;

#if NETFRAMEWORK
using System.CodeDom.Compiler;
#else
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
					: String.Join(",", Array.ConvertAll(this.ArgumentsType, arg => arg + " a" + (index++)));
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
			get => this._compilerVersion ?? RuntimeUtils.CurrentRuntimeVersion;
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
		/// <summary>Compile the assembly</summary>
		/// <returns>Resulting assembly</returns>
		public Assembly CompileAssembly()
		{//TODO: Not thread safe
			Assembly result;
			String sourceCode = this.GetSourceCode();

			result = DynamicCompiler.AssemblyLinks.TryGet(sourceCode);
			if(result == null)
			{
#if NETFRAMEWORK
				result = this.CompileAssembly(sourceCode);
#else
				result = this.CompileAssemblyRoslyn(sourceCode);
#endif
				DynamicCompiler.AssemblyLinks.Add(sourceCode, result);
			}
			return result;
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
		/// <param name="sourceCode">The full source code for generating the assembly</param>
		/// <returns>The generated assembly</returns>
		private Assembly CompileAssembly(String sourceCode)
		{
			CompilerInfo info = CompilerInfo2.GetSupportedCompiler(this.LanguageId)?.CompilerInfo
				?? throw new InvalidOperationException("CompilerInfo is null");

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

				CompilerResults result = compiler.CompileAssemblyFromSource(parameters, sourceCode);
				if(result.Errors.HasErrors)
					throw new CompilerException(sourceCode, result);

				return result.CompiledAssembly;
			}
		}
#else
		private static LanguageVersion GetLanguageVersion(String frameworkVersion)
		{
			if(String.IsNullOrEmpty(frameworkVersion))
				return LanguageVersion.Latest;

			// Parse version number
			if(Version.TryParse(frameworkVersion.TrimStart('v').Split('-')[0], out Version version))
			{
				if(version.Major >= 8) return LanguageVersion.CSharp12;
				if(version.Major >= 7) return LanguageVersion.CSharp11;
				if(version.Major >= 6) return LanguageVersion.CSharp10;
				if(version.Major >= 5) return LanguageVersion.CSharp9;
			}

			return LanguageVersion.Latest;
		}

		private Assembly CompileAssemblyRoslyn(String sourceCode)
		{
			LanguageVersion langVersion = GetLanguageVersion(this.CompilerVersion);

			var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(langVersion);
			var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode, parseOptions);

			List<MetadataReference> references = new List<MetadataReference>();
			// Add core assemblies
			Assembly[] coreAssemblies = new[]
			{
				typeof(Object).Assembly,
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
					path = Assembly.Load(assembly).Location;
					if(String.IsNullOrEmpty(path))
					{
						String simpleName = assembly.Substring(0, assembly.IndexOf(','));
						path = Assembly.Load(simpleName).Location;
					}
				}
				else
					path = Assembly.Load(assembly).Location;

				if(!String.IsNullOrEmpty(path) && File.Exists(path))
					references.Add(MetadataReference.CreateFromFile(path));
			}

			var compilation = CSharpCompilation.Create(
				this.ClassName + "_" + Guid.NewGuid().ToString("N"),
				new[] { syntaxTree },
				references,
				new CSharpCompilationOptions(
					OutputKind.DynamicallyLinkedLibrary,
					optimizationLevel: this.IsIncludeDebugInfo ? OptimizationLevel.Debug : OptimizationLevel.Release
				));

			using(var ms = new MemoryStream())
			{
				var emitResult = compilation.Emit(ms);
				if(!emitResult.Success)
				{
					throw new CompilerException(sourceCode, emitResult.Diagnostics);
				}
				ms.Position = 0;
				if(!String.IsNullOrWhiteSpace(this.CompiledAssemblyFilePath))
					using(var fs = new FileStream(this.CompiledAssemblyFilePath, FileMode.Create, FileAccess.Write))
					{
						ms.CopyTo(fs);
						ms.Position = 0;
					}
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