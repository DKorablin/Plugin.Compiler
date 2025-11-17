using System;
using System.Collections.Generic;
using System.IO;

namespace Plugin.Compiler.Bll.ScriptGenerator
{
	internal class FrameworkBatScript
	{
		/// <summary>BAT file header for dynamic compilation</summary>
		private const String ScriptArgs1 = @"
/*
@echo off && cls
set WinDirNet=%WinDir%\Microsoft.NET\Framework
IF EXIST ""%WinDirNet%\v2.0.50727\csc.exe"" set csc=""%WinDirNet%\v2.0.50727\csc.exe""
IF EXIST ""%WinDirNet%\v3.5\csc.exe"" set csc=""%WinDirNet%\v3.5\csc.exe""
IF EXIST ""%WinDirNet%\v4.0.30319\csc.exe"" set csc=""%WinDirNet%\v4.0.30319\csc.exe""
%csc% /nologo {Assemblies} /out:""%~0.exe"" %0
""%~0.exe""
del ""%~0.exe""
PAUSE
exit
*/
";

		private readonly AssemblyCollection _references;
		private readonly String _sourceCode;

		public FrameworkBatScript(AssemblyCollection references, String sourceCode)
		{
			this._references = references;
			this._sourceCode = sourceCode;
		}

		public String GenerateScript()
		{
			List<String> references = new List<String>();
			foreach(String assembly in this._references)
				if(File.Exists(assembly))//Physical file
					references.Add("/reference=\"" + assembly + '\"');

			String scriptHeader = ScriptArgs1.Replace("{Assemblies}", String.Join(" ", references.ToArray()));
			return scriptHeader + this._sourceCode;
		}
	}
}