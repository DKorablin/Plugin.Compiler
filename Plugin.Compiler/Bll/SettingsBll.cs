using System;
using System.Linq;
using AlphaOmega.Bll;
using System.IO;
using SAL.Flatbed;

namespace Plugin.Compiler.Bll
{
	internal class SettingsBll : BllBase<SettingsDataSet,SettingsDataSet.PluginTableRow>
	{
		private readonly PluginWindows _plugin;

		/// <summary>Получить ряды методов плагина</summary>
		/// <param name="pluginType">Тип плагина</param>
		/// <returns>Ряды описывающие все методы плагина</returns>
		public SettingsDataSet.PluginTableRow[] this[IPluginDescription plugin]
		{
			get
			{
				if(plugin == null)
					throw new ArgumentNullException(nameof(plugin));

				return base.DataSet.PluginTable.Where(p => p.PluginName == plugin.ID).ToArray();
			}
		}

		public SettingsBll(PluginWindows plugin)
			: base(0)
		{
			this._plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));

			using(Stream stream = this._plugin.Host.Plugins.Settings(this._plugin).LoadAssemblyBlob(Constant.SettingsFileName))
				if(stream != null)
					base.DataSet.ReadXml(stream);
		}

		public override void Save()
		{
			using(MemoryStream stream = new MemoryStream())
			{
				base.DataSet.WriteXml(stream);
				this._plugin.Host.Plugins.Settings(this._plugin).SaveAssemblyBlob(Constant.SettingsFileName, stream);
			}
			//base.Save();
		}

		/// <summary>Получить строку метода плагина</summary>
		/// <param name="plugin">Описатель плагина</param>
		/// <param name="methodName">Наименование метода</param>
		/// <returns>Ряд описывающий мтод плагина</returns>
		public SettingsDataSet.PluginTableRow GetPluginMethodRow(IPluginDescription plugin, String methodName)
		{
			if(String.IsNullOrEmpty(methodName))
				throw new ArgumentNullException(nameof(methodName));

			return this[plugin].FirstOrDefault(p => p.MethodName == methodName);
		}

		/// <summary>Удалить строку метода привязанную к плагину</summary>
		/// <param name="plugin">Описатель плагина, где необходимо удалить метод</param>
		/// <param name="methodName">Наименование метода для удаления</param>
		/// <returns>Удаление метода прошло успешно</returns>
		public Boolean RemovePluginMethodRow(IPluginDescription plugin, String methodName)
		{
			SettingsDataSet.PluginTableRow row = this.GetPluginMethodRow(plugin, methodName);
			if(row == null)
				return false;

			this.RemovePluginReferences(row.PluginID);
			row.Delete();
			this.Save();
			return true;
		}

		/// <summary>Получить компилятор с полной информацией для компиляции</summary>
		/// <param name="pluginName">Наименование плагина</param>
		/// <param name="methodName">Наименование метода</param>
		/// <returns>Компилятор</returns>
		public PartialCompiler AttachPluginMethodRow(IPluginDescription plugin, String methodName)
		{
			PartialCompiler compiler = new PartialCompiler(this._plugin, plugin);
			return this.AttachPluginMethodRow(plugin, methodName, ref compiler)
				? compiler
				: null;
		}

		/// <summary>Заполнить компилятор данными</summary>
		/// <param name="pluginName">Наименование плагина</param>
		/// <param name="methodName">Наименование метода</param>
		/// <param name="compiler">Компилятор, который необходимо заполнить</param>
		/// <returns>Ряд, описывающий информацию по компилируемому модулю</returns>
		public Boolean AttachPluginMethodRow(IPluginDescription plugin, String methodName, ref PartialCompiler compiler)
		{
			SettingsDataSet.PluginTableRow row = this.GetPluginMethodRow(plugin, methodName);
			if(row == null)
				return false;

			compiler.ClassName = row.MethodName;
			compiler.References = this.GetPluginReferenceCollection(row.PluginID);
			compiler.CompiledAssemblyFilePath = row.AssemblyPath;
			compiler.IsIncludeDebugInfo = row.IsIncludeDebug;
			compiler.IsFullSourceCode = row.IsFullSource;
			compiler.CompilerVersion = row.CompilerVersion;
			compiler.LanguageId = row.LanguageId;
			compiler.SourceCode = row.SourceCode;
			compiler.ReturnType = row.ReturnTypeI;
			return true;
		}

		/// <summary>Изменить или добавить информацию о методе</summary>
		/// <param name="pluginType">Наименование плагина</param>
		/// <param name="assemblyPath">Путь к скомпилированной сборки</param>
		public void ModifyPluginRow(IPluginDescription plugin, PartialCompiler compiler)
		{
			var row = GetPluginMethodRow(plugin, compiler.ClassName);
			Boolean newRow = row == null;
			if(newRow)
				row = base.DataSet.PluginTable.NewPluginTableRow();

			row.BeginEdit();
			row.PluginName = plugin.ID;
			row.MethodName = compiler.ClassName;
			row.IsFullSource = compiler.IsFullSourceCode;
			row.IsIncludeDebug = compiler.IsIncludeDebugInfo;
			row.CompilerVersion = compiler.CompilerVersion;
			row.LanguageId = compiler.LanguageId;
			row.SourceCode = compiler.SourceCode;
			row.ReturnTypeI = compiler.ReturnType;
			row.DateSaved = DateTime.Now;

			row.AssemblyPath = compiler.CompiledAssemblyFilePath;

			if(newRow)
				base.DataSet.PluginTable.AddPluginTableRow(row);
			else
				row.AcceptChanges();

			this.SetPluginReferences(row.PluginID, compiler.References);
			this.Save();
		}

		/// <summary>Получить класс ссылок и пространств имён по наименованию плагина и наименованию метода</summary>
		/// <param name="plugin">Описатель плагина</param>
		/// <param name="methodName">Наименование метода</param>
		/// <returns>Коллекция сборок и пространств имён</returns>
		public AssemblyCollection GetPluginReferences(IPluginDescription plugin, String methodName)
		{
			var row = this.GetPluginMethodRow(plugin, methodName);
			return row == null
				? new AssemblyCollection()
				: this.GetPluginReferenceCollection(row.PluginID);
		}

		/// <summary>Получить класс ссылок на сборки и пространств имён по идентификатору плагина и метода</summary>
		/// <param name="pluginId">Идентификатор метода и плагина</param>
		/// <returns>Коллекция сборок и пространств имён</returns>
		public AssemblyCollection GetPluginReferenceCollection(Int32 pluginId)
		{
			AssemblyCollection result = new AssemblyCollection();
			var rows = this.GetPluginReferences(pluginId);
			foreach(var row in rows)
			{
				String[] namespaces= row.Namespace.Split(new Char[]{';'},StringSplitOptions.RemoveEmptyEntries);
				result.AddNamespace(row.Assembly, namespaces);
			}
			return result;
		}

		/// <summary>Получить ряды ссылок на другие сборки</summary>
		/// <param name="pluginId">Идентификатор метода плагина</param>
		/// <returns>Набор сторок с ссылками сборки</returns>
		protected SettingsDataSet.ReferenceTableRow[] GetPluginReferences(Int32 pluginId)
			=> base.DataSet.ReferenceTable.Where(p => p.PluginID == pluginId).OrderBy(p => p.Assembly).ToArray();

		/// <summary>Установить ссылки для метода и сборки</summary>
		/// <param name="pluginType">Тип плагина</param>
		/// <param name="methodName">Наименование метода</param>
		/// <param name="collection">Коллекция ссылок для компиляции</param>
		public void SetPluginReferences(IPluginDescription plugin, String methodName, AssemblyCollection collection)
		{
			SettingsDataSet.PluginTableRow row = this.GetPluginMethodRow(plugin, methodName)
				?? throw new MissingMethodException($"Method not found. Plugin: {plugin.Name} ({plugin.ID}) Method: {methodName}");

			this.SetPluginReferences(row.PluginID, collection);
		}

		/// <summary>Установить ссылки для метода и сборки</summary>
		/// <param name="pluginId">Идентификатор плагина для которого установить ссылки</param>
		/// <param name="collection">Коллекция ссылок</param>
		private void SetPluginReferences(Int32 pluginId, AssemblyCollection collection)
		{
			this.RemovePluginReferences(pluginId);

			foreach(String assembly in collection)
			{
				String namespaces = String.Join(";", collection[assembly]);
				base.DataSet.ReferenceTable.AddReferenceTableRow(pluginId, assembly, namespaces);
			}
		}

		/// <summary>Удалить все ссылки на сборки для метода определённого плагина</summary>
		/// <param name="pluginId">Идентификатор плагина у которого удалить все ссылки</param>
		private void RemovePluginReferences(Int32 pluginId)
		{
			foreach(var reference in this.GetPluginReferences(pluginId))
				reference.Delete();
		}
	}
}