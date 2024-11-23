using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Plugin.Compiler.Bll;
using Plugin.Compiler.Xml;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.Compiler
{
	public class PluginWindows : IPlugin, IPluginSettings<PluginSettings>
	{
		#region Fields
		private TraceSource _trace;
		private PluginSettings _settings;
		private SettingsBll _settingsCompiler;
		private Dictionary<String, DockState> _documentTypes;
		#endregion Fields
		#region Properties
		internal TraceSource Trace => this._trace ?? (this._trace = PluginWindows.CreateTraceSource<PluginWindows>());

		internal IHost Host { get; }
		internal IHostWindows HostWindows => this.Host as IHostWindows;

		private IMenuItem MenuCompilers { get; set; }
		private IMenuItem PluginMenu { get; set; }

		/// <summary>Настройки для взаимодействия из хоста</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Настройки для взаимодействия из плагина</summary>
		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new PluginSettings();
					this.Host.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
				}
				return this._settings;
			}
		}

		internal SettingsBll SettingsCompiler
		{
			get
			{
				if(this._settingsCompiler == null)
				{
					SettingsBll settings = new SettingsBll(this);
					this.SettingsCompiler = settings;
				}
				return this._settingsCompiler;
			}
			set { this._settingsCompiler = value; }
		}

		private Dictionary<String, DockState> DocumentTypes
		{
			get
			{
				if(this._documentTypes == null)
					this._documentTypes = new Dictionary<String, DockState>()
					{
						{ typeof(DocumentCompiler).ToString(), DockState.Document },
					};
				return this._documentTypes;
			}
		}

		#endregion Properties
		#region Methods
		public PluginWindows(IHost host)
			=> this.Host = host ?? throw new ArgumentNullException(nameof(host));

		public IWindow GetPluginControl(String typeName, Object args)
			=> this.CreateWindow(typeName, false, args);

		/// <summary>Проверка на существование кода плагнина для вызова</summary>
		/// <param name="plugin">Описание плагина, который инициировал выполнение кода</param>
		/// <param name="methodName">Наименование выполняемого метода в динамическом коде</param>
		/// <exception cref="ArgumentNullException"><c>plugin</c> - is null</exception>
		/// <exception cref="ArgumentNullException"><c>plugin</c> - is null</exception>
		/// <returns>Данный метод создан и существует в настройках</returns>
		public Boolean IsMethodExists(IPluginDescription plugin, String methodName)
		{
			_ = plugin ?? throw new ArgumentNullException(nameof(plugin), "Caller plugin interface not specified");

			if(methodName == null || String.IsNullOrEmpty(methodName.Trim()))
				throw new ArgumentNullException(nameof(methodName), "MethodName required");

			return this.SettingsCompiler.GetPluginMethodRow(plugin, methodName) != null;
		}

		/// <summary>Получить список всех методов, которые созданы для этого плагина</summary>
		/// <param name="plugin">Описание плагина, который инициировал выполнение кода</param>
		/// <returns>Список методов созданных для плагина</returns>
		public String[] GetMethods(IPluginDescription plugin)
		{
			_ = plugin ?? throw new ArgumentNullException(nameof(plugin), "Caller plugin interface not specified");

			return this.SettingsCompiler[plugin].Select(p => p.MethodName).ToArray();
		}

		/// <summary>Удалить метод плагина</summary>
		/// <param name="plugin">Описание плагина</param>
		/// <param name="methodName">Наименование метода</param>
		/// <returns>Результат удаления метода</returns>
		public Boolean DeleteMethod(IPluginDescription plugin, String methodName)
		{
			_ = plugin ?? throw new ArgumentNullException(nameof(plugin), "Caller plugin interface not specified");

			if(methodName == null || String.IsNullOrEmpty(methodName.Trim()))
				throw new ArgumentNullException(nameof(methodName), "MethodName required");

			return this.SettingsCompiler.RemovePluginMethodRow(plugin, methodName);
		}

		/// <summary>Выполнить динамический метод для плагина, используя код сохранённый внупри плагина компилятора</summary>
		/// <param name="plugin">Описание плагина, который инициировал выполнение кода</param>
		/// <param name="methodName">Наименование выполняемого класса в динамическом коде</param>
		/// <param name="compilerArgs">Аргументы передаваемые извне в плагин</param>
		/// <returns>Результат выполнения динамического кода</returns>
		public Object InvokeDynamicMethod(IPluginDescription plugin, String methodName, params Object[] compilerArgs)
		{
			_ = plugin ?? throw new ArgumentNullException(nameof(plugin), "Caller plugin interface not specified");

			if(methodName == null || String.IsNullOrEmpty(methodName.Trim()))
				throw new ArgumentNullException(nameof(methodName), "MethodName required");

			PartialCompiler compiler = this.SettingsCompiler.AttachPluginMethodRow(plugin, methodName);
			if(compiler == null)
			{
				ProjectXmlLoader loader = new ProjectXmlLoader(this);
				compiler = loader.LoadCompilerString(plugin, methodName);
			}
			if(compiler == null)
				throw new NotImplementedException("Specified method not implemented. Please create it first.");

			/*if(!String.IsNullOrEmpty(row.AssemblyPath))
				{//Выпрлнить сборку из файла
					FileInfo fileInfo = new FileInfo(row.AssemblyPath);
					Assembly assembly;
					if(row.DateSaved > fileInfo.LastWriteTime)//Сохранённая сборка устарела. Обновить сборку
						assembly = compiler.CompileAssembly(info, sourceCode);
					else//Взять сборку с диска
					{
						Byte[] rawAssembly = File.ReadAllBytes(row.AssemblyPath);
						assembly = Assembly.Load(rawAssembly);
					}
					return compiler.InvokeAssembly<Object>(assembly,
						plugin,
						compilerArgs);
				} else*/
			if(compilerArgs != null)//TODO: Скорее всего надо будет сохранять типы
				compiler.ArgumentsType = Array.ConvertAll(compilerArgs, delegate(Object a) { return a == null ? "Object" : a.GetType().Name; });
			return compiler.ComplileAndInvoke<Object>(plugin.Instance, compilerArgs);
		}

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			IHostWindows host = this.HostWindows;
			if(host == null)
				this.Trace.TraceEvent(TraceEventType.Error, 10, "Plugin {0} requires {1}", this, typeof(IHostWindows));
			else
			{
				IMenuItem menuTools = host.MainMenu.FindMenuItem("Tools");
				if(menuTools == null)
					this.Trace.TraceEvent(TraceEventType.Error, 10, "Menu item 'Tools' not found");
				else
				{
					this.MenuCompilers = menuTools.FindMenuItem("Compilers");
					if(this.MenuCompilers == null)
					{
						this.MenuCompilers = menuTools.Create("Compilers");
						this.MenuCompilers.Name = "Tools.Compilers";
						menuTools.Items.Add(this.MenuCompilers);
					}

					this.PluginMenu = menuTools.Create("&CSC");
					this.PluginMenu.Name = "Tools.Compilers.CSC";
					this.PluginMenu.Click += (sender, e) => { this.CreateWindow(typeof(DocumentCompiler).ToString(), false); };

					this.MenuCompilers.Items.Insert(0, this.PluginMenu);
					return true;
				}
			}
			return false;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			if(this.PluginMenu != null)
				this.HostWindows.MainMenu.Items.Remove(this.PluginMenu);
			if(this.MenuCompilers != null && this.MenuCompilers.Items.Count == 0)
				this.HostWindows.MainMenu.Items.Remove(this.MenuCompilers);
			return true;
		}

		private IWindow CreateWindow(String typeName, Boolean searchForOpened, Object args = null)
			=> this.DocumentTypes.TryGetValue(typeName, out DockState state)
				? this.HostWindows.Windows.CreateWindow(this, typeName, searchForOpened, state, args)
				: null;

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}
		#endregion Methods
	}
}