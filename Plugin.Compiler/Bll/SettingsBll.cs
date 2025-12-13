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

		/// <summary>Get plugin method rows</summary>
		/// <param name="pluginType">Plugin type</param>
		/// <returns>Rows describing all plugin methods</returns>
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
		}

		/// <summary>Get the plugin method string</summary>
		/// <param name="plugin">Plugin handle</param>
		/// <param name="methodName">Method name</param>
		/// <returns>String describing the plugin method</returns>
		public SettingsDataSet.PluginTableRow GetPluginMethodRow(IPluginDescription plugin, String methodName)
		{
			if(String.IsNullOrEmpty(methodName))
				throw new ArgumentNullException(nameof(methodName));

			return this[plugin].FirstOrDefault(p => p.MethodName == methodName);
		}

		/// <summary>Delete the method string associated with the plugin</summary>
		/// <param name="plugin">Plugin handle where the method should be deleted</param>
		/// <param name="methodName">Name of the method to delete</param>
		/// <returns>Method deletion successful</returns>
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

		/// <summary>Get the compiler with full compilation information</summary>
		/// <param name="pluginName">Plugin name</param>
		/// <param name="methodName">Method name</param>
		/// <returns>Compiler</returns>
		public PartialCompiler AttachPluginMethodRow(IPluginDescription plugin, String methodName)
		{
			PartialCompiler compiler = new PartialCompiler(this._plugin, plugin);
			return this.AttachPluginMethodRow(plugin, methodName, ref compiler)
				? compiler
				: null;
		}

		/// <summary>Fill the compiler with data</summary>
		/// <param name="pluginName">Plugin name</param>
		/// <param name="methodName">Method name</param>
		/// <param name="compiler">Compiler to be filled</param>
		/// <returns>A row describing information about the compiled module</returns>
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

		/// <summary>Edit or add method information</summary>
		/// <param name="pluginType">Plugin name</param>
		/// <param name="assemblyPath">Path to the compiled assembly</param>
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

		/// <summary>Get a class of references and namespaces by plugin name and method name</summary>
		/// <param name="plugin">Plugin handle</param>
		/// <param name="methodName">Method name</param>
		/// <returns>Collection of assemblies and namespaces</returns>
		public AssemblyCollection GetPluginReferences(IPluginDescription plugin, String methodName)
		{
			var row = this.GetPluginMethodRow(plugin, methodName);
			return row == null
				? new AssemblyCollection()
				: this.GetPluginReferenceCollection(row.PluginID);
		}

		/// <summary>Get a class of assembly references and namespaces by plugin and method ID</summary>
		/// <param name="pluginId">Method and plugin ID</param>
		/// <returns>Collection of assemblies and namespaces</returns>
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

		/// <summary>Get rows of references to other assemblies</summary>
		/// <param name="pluginId">Plugin method ID</param>
		/// <returns>A set of rows with assembly references</returns>
		protected SettingsDataSet.ReferenceTableRow[] GetPluginReferences(Int32 pluginId)
			=> base.DataSet.ReferenceTable.Where(p => p.PluginID == pluginId).OrderBy(p => p.Assembly).ToArray();

		/// <summary>Set references for the method and assembly</summary>
		/// <param name="pluginType">Plugin type</param>
		/// <param name="methodName">Method name</param>
		/// <param name="collection">Collection of references for compilation</param>
		public void SetPluginReferences(IPluginDescription plugin, String methodName, AssemblyCollection collection)
		{
			SettingsDataSet.PluginTableRow row = this.GetPluginMethodRow(plugin, methodName)
				?? throw new MissingMethodException($"Method not found. Plugin: {plugin.Name} ({plugin.ID}) Method: {methodName}");

			this.SetPluginReferences(row.PluginID, collection);
		}

		/// <summary>Set references for the method and assembly</summary>
		/// <param name="pluginId">Identifier of the plugin to set references for</param>
		/// <param name="collection">Collection of references</param>
		private void SetPluginReferences(Int32 pluginId, AssemblyCollection collection)
		{
			this.RemovePluginReferences(pluginId);

			foreach(String assembly in collection)
			{
				String namespaces = String.Join(";", collection[assembly]);
				base.DataSet.ReferenceTable.AddReferenceTableRow(pluginId, assembly, namespaces);
			}
		}

		/// <summary>Remove all assembly references for a method in a specific plugin</summary>
		/// <param name="pluginId">The ID of the plugin for which to remove all references</param>
		private void RemovePluginReferences(Int32 pluginId)
		{
			foreach(var reference in this.GetPluginReferences(pluginId))
				reference.Delete();
		}
	}
}