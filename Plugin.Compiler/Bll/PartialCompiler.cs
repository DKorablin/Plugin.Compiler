using System;
using System.CodeDom.Compiler;
using System.Text;
using SAL.Flatbed;

namespace Plugin.Compiler.Bll
{
	internal class PartialCompiler : DynamicCompiler
	{
		private PluginWindows Plugin { get; }

		/// <summary>Передаётся полный исходный код</summary>
		public Boolean IsFullSourceCode { get; set; }

		public PartialCompiler(PluginWindows plugin)
			: this(plugin, null)
		{ }

		public PartialCompiler(PluginWindows plugin, IPluginDescription pluginDescription)
			: base(pluginDescription)
			=> this.Plugin = plugin;

		/// <summary>Получить исходный код для компиляции</summary>
		/// <returns>Исходный код для компиляции</returns>
		protected override String GetSourceCode()
			=> this.GetFullSourceCode();

		/// <summary>Получить полный исходный код, который будет скомпилирован и выполнен</summary>
		/// <param name="languageId">Идентификатор компилятора, который используется для компиляции кода</param>
		/// <param name="sourceCode">Пользовательский исходный код, который будет помещён в оболочку</param>
		/// <returns>Полный исходный код для компиляции</returns>
		public String GetFullSourceCode(Int32 languageId, String sourceCode)
		{
			CompilerInfo info = this.GetSupportedCompiler(languageId)
				?? throw new ArgumentNullException(nameof(info), $"Supported compiler not found by languageId: {languageId}");

			return this.GetFullSourceCode(info, sourceCode);
		}

		/// <summary>Получить полный исходный код</summary>
		/// <returns>Полный исходный код для компиляции</returns>
		public String GetFullSourceCode()
			=> this.IsFullSourceCode
				? this.SourceCode
				: this.GetFullSourceCode(this.GetSupportedCompiler(this.LanguageId), this.SourceCode);

		/// <summary>Получить полный исходный код, который будет скомпилирован и выполнен</summary>
		/// <param name="info">Информация о компиляторе, который используется для компиляции кода</param>
		/// <param name="sourceCode">Пользовательский исходный код, который будет помещён в оболочку</param>
		/// <returns>Полный исходный код для компиляции</returns>
		public String GetFullSourceCode(CompilerInfo info, String sourceCode)
			=> this.GetFullSourceCode(this.GetSupportedLanguage(info), sourceCode);

		/// <summary>Получить полный исходный код, который будет скомпилирован и выполнен</summary>
		/// <param name="language">Язык на котором написан исходный код</param>
		/// <param name="sourceCode">Пользовательский исходный код, который будет помещён в оболочку</param>
		/// <returns>Полный исходный код для компиляции</returns>
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

		/// <summary>Получить код для сохранения в bat файл</summary>
		/// <returns>Отформатированный код для выполнения batch'ем</returns>
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

		/// <summary>Полчить исходный код без лишнего форматирования</summary>
		/// <param name="fullSourceCode">Полный исходный код, который будет помещён в оболочку</param>
		/// <returns>Отчищенный исходный код</returns>
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