using System;
#if NETFRAMEWORK
using System.CodeDom.Compiler;
#endif
using System.Text;
using SAL.Flatbed;

namespace Plugin.Compiler.Bll
{
	internal class PartialCompiler : DynamicCompiler
	{
		private PluginWindows Plugin { get; }

		/// <summary>The full source code is specified</summary>
		public Boolean IsFullSourceCode { get; set; }

		public PartialCompiler(PluginWindows plugin)
			: this(plugin, null)
		{ }

		public PartialCompiler(PluginWindows plugin, IPluginDescription pluginDescription)
			: base(pluginDescription)
			=> this.Plugin = plugin;

		/// <summary>Get source code for compilation</summary>
		/// <returns>Source code for compilation</returns>
		protected override String GetSourceCode()
			=> this.GetFullSourceCode();

#if NETFRAMEWORK
		/// <summary>Get the full source code that will be compiled and executed</summary>
		/// <param name="languageId">The compiler identifier used to compile the code</param>
		/// <param name="sourceCode">The custom source code that will be wrapped</param>
		/// <returns>The full source code to compile</returns>
		public String GetFullSourceCode(Int32 languageId, String sourceCode)
		{
			var infoObj = this.GetSupportedCompiler(languageId);
			if(infoObj == null)
				throw new ArgumentException(nameof(languageId), $"Supported compiler not found by languageId: {languageId}");
			CompilerInfo info = infoObj;
			return this.GetFullSourceCode(info, sourceCode);
		}
#endif

		/// <summary>Get the full source code</summary>
		/// <returns>Full source code for compilation</returns>
		public String GetFullSourceCode()
			=> this.IsFullSourceCode
				? this.SourceCode
#if NETFRAMEWORK
				: this.GetFullSourceCode(this.GetSupportedLanguage(this.GetSupportedCompiler(this.LanguageId)), this.SourceCode);
#else
				: this.GetFullSourceCode("CS", this.SourceCode);
#endif

#if NETFRAMEWORK
		/// <summary>Get the full source code that will be compiled and executed</summary>
		/// <param name="info">Information about the compiler used to compile the code</param>
		/// <param name="sourceCode">Custom source code that will be wrapped</param>
		/// <returns>The full source code to compile</returns>
		public String GetFullSourceCode(CompilerInfo info, String sourceCode)
			=> this.GetFullSourceCode(this.GetSupportedLanguage(info), sourceCode);
#endif

		/// <summary>Get the full source code that will be compiled and executed</summary>
		/// <param name="language">The language the source code is written in</param>
		/// <param name="sourceCode">Custom source code that will be wrapped</param>
		/// <returns>The full source code for compilation</returns>
		public String GetFullSourceCode(String language, String sourceCode)
		{
			if(String.IsNullOrEmpty(language))
				throw new ArgumentNullException(nameof(language));

			StringBuilder namespaces = new StringBuilder();
			foreach(String asm in this.References)
				foreach(String ns in this.References[asm])
					namespaces.AppendLine(String.Format(Constant.NamespaceTemplateArgs1, ns));

			return Constant.Code.FormatCodeTemplate(
				this.Plugin.Settings.GetCodeTemplate(language),
				namespaces.ToString(),
				this.ClassName,
				sourceCode,
				this.ReturnType,
				this.ArgumentsType);
		}

		/// <summary>Get code for saving to a batch file</summary>
		/// <returns>Formatted code for execution by batch</returns>
		public override String GetBatchCode()
		{
			String sourceCodeWithHeader = base.GetBatchCode();

			StringBuilder namespaces = new StringBuilder();
			foreach(String asm in this.References)
				foreach(String ns in this.References[asm])
					namespaces.AppendLine(String.Format(Constant.NamespaceTemplateArgs1, ns));

			String runner = Constant.Code.FormatCodeTemplate(
				Constant.Code.Batch.RunnerArg4,
				namespaces.ToString(),
				this.ClassName,
				String.Empty,
				this.ReturnType,
				this.ArgumentsType);

			return sourceCodeWithHeader + runner;
		}

		/// <summary>Get the source code without unnecessary formatting</summary>
		/// <param name="fullSourceCode">The full source code that will be placed in the shell</param>
		/// <returns>The cleaned source code</returns>
		public String TryToClearFullSource(String fullSourceCode)
		{
			const String MethodBegin = "Method Begin";
			const String MethodEnd = "Method End";

			Int32 startIndex = fullSourceCode.IndexOf(MethodBegin);
			Int32 endIndex = fullSourceCode.IndexOf(MethodEnd);
			if(startIndex > -1 && endIndex > -1)
			{
				endIndex = fullSourceCode.LastIndexOf('\n', endIndex);
				return fullSourceCode.Substring(startIndex + MethodBegin.Length, endIndex - (startIndex + MethodBegin.Length)).Trim('\t','\r','\n',' ');
			} else
				return null;
		}
	}
}