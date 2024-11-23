using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AlphaOmega.Reflection;
using Microsoft.Win32;
using SAL.Flatbed;

namespace Plugin.Compiler.Bll
{
	internal class DynamicCompiler
	{
		private String _returnType;
		private String _className;
		private static CompiledAssemblyLinks AssemblyLinks = new CompiledAssemblyLinks();
		private String _compilerVersion;

		/// <summary>Список ссылок и пространств имён, которые будут добавлены в исходный код</summary>
		public AssemblyCollection References { get; internal set; }

		/// <summary>Наименование плагина, который добавляется в исходный код</summary>
		public String ClassName
		{
			get => this._className;
			internal set => this._className = String.IsNullOrEmpty(value)
				? "Undefined"
				: value.Trim().Replace(' ', '_').Replace('.', '_');
		}

		/// <summary>Полное наименование вызваемого метода</summary>
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

		/// <summary>Входящие аргументы</summary>
		public String[] ArgumentsType { get; set; }

		/// <summary>Результат выполнения метода</summary>
		public String ReturnType
		{
			get => this._returnType;
			set =>this._returnType = value == null || value == "System.Void"
					? null
					: value;
		}

		/// <summary>Исходный код для компиляции</summary>
		public String SourceCode { get; set; }

		/// <summary>Путь к файлу куда компилировать сборку</summary>
		/// <remarks>TODO: Пока параметр не используется</remarks>
		public String CompiledAssemblyFilePath { get; set; }

		/// <summary>Версия в которой компилировать сборку</summary>
		/// <remarks>По умолчанию используется версия v2.0</remarks>
		public String CompilerVersion
		{
			get => this._compilerVersion ?? Constant.DefaultCompilerVersion;
			set => this._compilerVersion = value;
		}

		/// <summary>Включить в сборку отладочную информацию</summary>
		public Boolean IsIncludeDebugInfo { get; set; }

		/// <summary>Идентификатор языка на которм написан код</summary>
		public Int32 LanguageId { get; set; }

		/// <summary>Создание экземпляра класса</summary>
		public DynamicCompiler()
			: this(null)
		{ }

		/// <summary>Создание экземпляра класса с указанием стартового префикса класса</summary>
		/// <param name="pluginName">Наименование плагина для которого компилируется код</param>
		public DynamicCompiler(IPluginDescription pluginDescription)
		{
			this.ClassName = pluginDescription == null ? "Undefined" : String.Join("_", pluginDescription.ID.Split('-'));
			this.References = new AssemblyCollection();
		}

		#region Methods
		/// <summary>Получить массив установленных версий фреймворка</summary>
		/// <returns>Массив номеров установленных версий</returns>
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

		/// <summary>Получить список поддерживаемых компиляторов</summary>
		/// <returns>Список поддерживаемых компиляторов</returns>
		public CompilerInfo[] GetSupportedCompilers()
			=> CodeDomProvider.GetAllCompilerInfo();

		/// <summary>Получить поддерживаемый компилятор</summary>
		/// <param name="index">Индекс компилятора из списка поддерживаемых</param>
		/// <returns>Информация по найденному компилятору или null</returns>
		public CompilerInfo GetSupportedCompiler(Int32 index)
		{
			CompilerInfo[] compilers = this.GetSupportedCompilers();
			return compilers.Length > index ? compilers[index] : null;
		}

		/// <summary>Получить поддерживаемый язык из информации о компиляторе</summary>
		/// <param name="compiler">Информация о компиляторе</param>
		/// <returns>Наименование языка из информации о компиляторе</returns>
		public String GetSupportedLanguage(CompilerInfo info)
		{
			String[] names = info.GetLanguages();
			Int32 length = names.Length;
			return length > 0 ? names[length - 1] : String.Empty;
		}

		/// <summary>Получить список поддерживаемых языков</summary>
		/// <returns>Массив поддерживаемых языков компилятором</returns>
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

		/// <summary>Скомпилировать сборку</summary>
		/// <returns>Полученная сборка</returns>
		public Assembly CompileAssembly()
		{//TODO: Not thread safe
			Assembly result;
			CompilerInfo info = this.GetSupportedCompiler(this.LanguageId);

			String sourceCode = this.GetSourceCode();

			result = DynamicCompiler.AssemblyLinks.TryGet(sourceCode);
			if(result == null)
			{
				result = this.CompileAssembly(info, sourceCode);
				DynamicCompiler.AssemblyLinks.Add(sourceCode, result);
			}
			return result;
		}

		/// <summary>Сохранить исходный код в виде bat файла</summary>
		/// <param name="filePath">Путь к файлу в который записать код</param>
		public virtual String GetBatchCode()
		{
			List<String> references = new List<String>();

			foreach(String assembly in this.References)
				if(File.Exists(assembly))//Physical file
					references.Add("/reference:\"" + assembly + '\"');

			String batchHeader = Constant.Code.Batch.CompilerArgs1.Replace("{Assemblies}", String.Join(" ", references.ToArray()));

			String sourceCode = this.GetSourceCode();
			return batchHeader + sourceCode;
		}

		/// <summary>Скомпилировать и запустить сборку</summary>
		/// <typeparam name="T">Результат выполнения</typeparam>
		/// <param name="plugin">Вызывающий плагин</param>
		/// <param name="args">Аргументы передаваемые в код</param>
		/// <returns>Результат выполнения динамически скомпилированного кода</returns>
		public T ComplileAndInvoke<T>(IPlugin plugin, params Object[] args)
		{
			Assembly assembly = this.CompileAssembly();
			return this.InvokeAssembly<T>(assembly, plugin, args);
		}

		/// <summary>Получить исходный код для компиляции</summary>
		/// <returns>Исходный код для компиляции</returns>
		protected virtual String GetSourceCode()
			=> this.SourceCode;

		/// <summary>Скомпилировать сборку из класса, полностью написанного пользователем</summary>
		/// <param name="language">Язык на котором написан код</param>
		/// <param name="fullCode">Полный исходный код для генерации сборки</param>
		/// <returns>Сгенерированная сборка</returns>
		private Assembly CompileAssembly(CompilerInfo info, String sourceCode)
		{
			if(!String.IsNullOrEmpty(this.CompilerVersion))
			{//Setting the compiler verion. Because of .NET 5+ I have to change this object type from property to field
				IDictionary<String, String> data = (IDictionary<String, String>)info.GetType().InvokeMember("_providerOptions", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, info, null);
				if(data.Count > 0 && data.Keys.Contains("CompilerVersion"))
					data["CompilerVersion"] = this.CompilerVersion;
			}

			using(CodeDomProvider compiler = info.CreateProvider())
			{
				//var compiler = CodeDomProvider.CreateProvider(language);
				CompilerParameters parameters = info.CreateDefaultCompilerParameters();

				foreach(String assembly in this.References)
					if(File.Exists(assembly))//Physical path
						parameters.ReferencedAssemblies.Add(assembly);
					else// if(assembly.Contains(","))//GAC Name
					{
						String path = AssemblyCache.QueryAssemblyInfo(assembly);
						parameters.ReferencedAssemblies.Add(path);
						//parameters.ReferencedAssemblies.Add(assembly.Substring(0, assembly.IndexOf(',')) + ".dll");
					}/* else
					parameters.ReferencedAssemblies.Add(assembly + ".dll");*/

				parameters.GenerateInMemory = true;
				if(this.IsIncludeDebugInfo)
				{
					parameters.IncludeDebugInformation = true;
					parameters.CompilerOptions += "/debug+ /debug:full /optimize-";
				} else
					parameters.CompilerOptions += "/optimize";
				
				parameters.OutputAssembly = this.CompiledAssemblyFilePath;
				//if(compiler.Supports(GeneratorSupport.EntryPointMethod))
				parameters.MainClass = Constant.Code.NamespaceName + "." + this.ClassName;

				//TODO: It's not working after .NET 5+. See: https://stackoverflow.com/questions/73793505/system-platformnotsupportedexception-compiling-c-sharp-code-at-runtime-net-core
				CompilerResults result = compiler.CompileAssemblyFromSource(parameters, sourceCode);
				if(result.Errors.HasErrors)
					throw new CompilerException(sourceCode, result);

				return result.CompiledAssembly;
			}
		}

		/// <summary>Выполнить сборку</summary>
		/// <typeparam name="T">Тип возвращаемого результата метода</typeparam>
		/// <param name="host">Хост, запускающий динамический код</param>
		/// <param name="plugin">Плагин, который вызвал код</param>
		/// <param name="args">Аргументы передаваемые в динамически скомпилированную сборку</param>
		/// <returns>Результат выполнения динамически скомпилированной сборки</returns>
		public T InvokeAssembly<T>(Assembly assembly,IPlugin plugin, params Object[] args)
		{
			String typeName = Constant.Code.NamespaceName + "." + this.ClassName;
			Object objClass = assembly.CreateInstance(typeName)
				?? throw new EntryPointNotFoundException(typeName);

			Type type = objClass.GetType();
			PropertyInfo pluginProperty = type.GetProperty("Plugin");
			if(pluginProperty != null && pluginProperty.CanWrite)
				pluginProperty.SetValue(objClass, plugin, null);

			//objClass.GetType().InvokeMember("Plugin", BindingFlags.SetProperty, null, objClass, new[] { plugin, });

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
					: String.Join(", ", Array.ConvertAll(args, delegate(Object item) { return item == null ? "null" : item.GetType().Name + " " + item.ToString(); }));
				throw new InvalidOperationException(String.Format("Method: {0} does not expect parameters {1}", fullMethodName, argsName));
				//InvalidOperationException exc=new InvalidOperationException(String.Format
			}
		}
		#endregion Methods
	}
}