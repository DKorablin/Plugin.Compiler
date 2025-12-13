using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Plugin.Compiler.UI;

namespace Plugin.Compiler
{
	public class PluginSettings : INotifyPropertyChanged
	{
		private String _defaultCompilerLanguage;
		private Boolean _showNonPublicMembers;
		private String _vbCode;
		private String _csCode;
		private String _mcCode;
		private String _vjCode;

		[Category("UI")]
		[Description("Setting the default compilation language")]
		[Editor(typeof(LanguageEditor), typeof(UITypeEditor))]
		public String DefaultCompilerLanguage {
			get => this._defaultCompilerLanguage;
			set => this.SetField(ref this._defaultCompilerLanguage, value, nameof(this.DefaultCompilerLanguage));
		}

		[DefaultValue(false)]
		[Category("UI")]
		[Description("Show all objects in loaded assemblies or only objects with public access level")]
		public Boolean ShowNonPublicMembers
		{
			get => this._showNonPublicMembers;
			set => this.SetField(ref this._showNonPublicMembers, value, nameof(this.ShowNonPublicMembers));
		}

		[Category("Template")]
		[DefaultValue(Constant.Code.VB)]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		[DisplayName("VB Code")]
		[Description("Template for complete code formatting in VB Script language. Available keys: {Using}, {Namespace}, {ClassName}, {MethodName}, {SourceCode}")]
		public String VbCode
		{
			get => this._vbCode ?? Constant.Code.VB;
			set => this.SetField(ref this._vbCode, TestTemplate(value, Constant.Code.VB), nameof(this.VbCode));
		}

		[Category("Template")]
		[DefaultValue(Constant.Code.CS)]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		[DisplayName("C# Code")]
		[Description("Fully formatted template form C# language. Available keys: {Using}, {Namespace}, {ClassName}, {MethodName}, {SourceCode}, {ReturnType}, {ArgumentsType}")]
		public String CsCode
		{
			get => this._csCode ?? Constant.Code.CS;
			set => this.SetField(ref this._csCode, TestTemplate(value, Constant.Code.CS), nameof(this.CsCode));
		}

		[Category("Template")]
		[DefaultValue(Constant.Code.MCPP)]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		[DisplayName("MC++ Code")]
		[Description("Fully formatted template for Managed C++ language. Available keys: {Using}, {Namespace}, {ClassName}, {MethodName}, {SourceCode}, {ReturnType}, {ArgumentsType}")]
		public String McCode
		{
			get => this._mcCode ?? Constant.Code.MCPP;
			set => this.SetField(ref this._mcCode, TestTemplate(value, Constant.Code.MCPP), nameof(this.McCode));
		}

		[Category("Template")]
		[DefaultValue(Constant.Code.VJ)]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		[DisplayName("Visual J#")]
		[Description("Fully formatted template for Java# language. Available keys: {Using}, {Namespace}, {ClassName}, {MethodName}, {SourceCode}, {ReturnType}, {ArgumentsType}")]
		public String VjCode
		{
			get => this._vjCode ?? Constant.Code.VJ;
			set => this.SetField(ref this._vjCode, TestTemplate(value, Constant.Code.VJ), nameof(this.VjCode));
		}

		internal String GetCodeTemplate(String language)
		{
			switch(language)
			{
			case "vbscript":
				return this.VbCode;
			case "csharp":
				return this.CsCode;
			case "cpp":
				return this.McCode;
			case "vjsharp":
				return this.VjCode;
			default: throw new NotSupportedException(String.Format("Language {0} does not supported by partial compiler.",language));
			}
		}

		private static String TestTemplate(String value, String defaultValue)
			=> (value ?? String.Empty).Trim().Length == 0 || value.Equals(defaultValue)
				? null
				: value;

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private Boolean SetField<T>(ref T field, T value, String propertyName)
		{
			if(EqualityComparer<T>.Default.Equals(field, value))
				return false;

			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
		#endregion INotifyPropertyChanged
	}
}