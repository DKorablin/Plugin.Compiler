using System;
using System.Xml.Linq;
using Plugin.Compiler.Bll;
using System.IO;
using SAL.Flatbed;

namespace Plugin.Compiler.Xml
{
	internal class ProjectXmlLoader
	{
		private readonly PluginWindows _plugin;
		public ProjectXmlLoader(PluginWindows plugin)
			=> this._plugin = plugin;

		public PartialCompiler LoadCompilerString(IPluginDescription pluginDescription, String xml)
		{
			if(String.IsNullOrEmpty(xml))
				throw new ArgumentNullException(nameof(xml));

			return xml[0] == '<'
				? this.LoadCompiler(pluginDescription, XDocument.Parse(xml, LoadOptions.PreserveWhitespace))
				: null;
		}

		public PartialCompiler LoadCompilerFile(IPluginDescription plugin, String filePath)
		{
			if(String.IsNullOrEmpty(filePath))
				throw new ArgumentNullException(nameof(filePath));
			if(!File.Exists(filePath))
				throw new FileNotFoundException($"File {filePath} not found");

			return this.LoadCompiler(plugin, XDocument.Load(filePath, LoadOptions.PreserveWhitespace));
		}

		private PartialCompiler LoadCompiler(IPluginDescription plugin, XDocument doc)
		{
			PartialCompiler result = new PartialCompiler(this._plugin, plugin)
			{
				ClassName = doc.Root.Attribute("ClassName").Value,
				CompiledAssemblyFilePath = doc.Root.Attribute("CompiledAssemblyFilePath").Value,
				LanguageId = Int32.Parse(doc.Root.Attribute("LanguageId").Value),
				IsFullSourceCode = Boolean.Parse(doc.Root.Attribute("IsFullSourceCode").Value),
				IsIncludeDebugInfo = Boolean.Parse(doc.Root.Attribute("IsIncludeDebugInfo").Value),
				CompilerVersion = doc.Root.Attribute("CompilerVersion").Value
			};

			XElement references = doc.Root.Element("References");
			foreach(XElement asm in references.Elements("Assembly"))
			{
				String assembly = asm.Attribute("Name").Value;
				String[] namespaces = asm.Value.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				result.References.AddNamespace(assembly, namespaces);
			}
			result.SourceCode = doc.Root.Element("Code").Value.Replace("\n", Environment.NewLine);
			return result;
		}

		public static String SaveCompiler(PartialCompiler compiler)
		{
			_ = compiler ?? throw new ArgumentNullException(nameof(compiler));

			XDocument doc = new XDocument();
			XElement root = new XElement("Compiler");
			root.Add(new XAttribute("ClassName", compiler.ClassName));
			root.Add(new XAttribute("CompiledAssemblyFilePath", compiler.CompiledAssemblyFilePath));
			root.Add(new XAttribute("LanguageId", compiler.LanguageId));
			root.Add(new XAttribute("IsFullSourceCode", compiler.IsFullSourceCode));
			root.Add(new XAttribute("IsIncludeDebugInfo", compiler.IsIncludeDebugInfo));
			root.Add(new XAttribute("CompilerVersion", compiler.CompilerVersion));
			doc.Add(root);
			doc.Root.Add(new XElement("Code", compiler.SourceCode));

			XElement references = new XElement("References");
			doc.Root.Add(references);
			foreach(String assembly in compiler.References)
			{
				XElement asmElement = new XElement("Assembly");
				asmElement.Add(new XAttribute("Name", assembly));
				asmElement.Value = String.Join(";", compiler.References[assembly]);
				references.Add(asmElement);
			}
			return doc.ToString();
		}
	}
}