using System;
using System.Collections.Generic;
using System.IO;

namespace Plugin.Compiler.Bll.ScriptGenerator
{
	internal class FrameworkPowerShellScript
	{
		private const String ScriptArgs1 = @"
/*
$WinDirNet = ""$env:WinDir\Microsoft.NET\Framework""
$CscPaths = @(
    Join-Path $WinDirNet ""v2.0.50727\csc.exe"",
    Join-Path $WinDirNet ""v3.5\csc.exe"",
    Join-Path $WinDirNet ""v4.0.30319\csc.exe""
)

$Csc = $CscPaths | Where-Object { Test-Path $_ } | Select-Object -Last 1

if (-not $Csc) {
    Write-Error ""Could not find a .NET Framework csc.exe. Compilation failed.""
    exit 1
}

# --- Define File Paths ---
$ScriptPath = $MyInvocation.MyCommand.Path
$ExePath = ""$ScriptPath.exe""

& $Csc /nologo {Assemblies} /out:""$ExePath"" ""$ScriptPath""

if (Test-Path $ExePath) {
    & $ExePath
    Remove-Item $ExePath
} else {
    Write-Error ""Compilation failed. Executable not found.""
}
exit
*/
";
		private readonly AssemblyCollection _references;
		private readonly String _sourceCode;

		public FrameworkPowerShellScript(AssemblyCollection references, String sourceCode)
		{
			this._references = references;
			this._sourceCode = sourceCode;
		}

		public String GenerateScript()
		{
			List<String> references = new List<String>();
			foreach(String assembly in this._references)
				if(File.Exists(assembly))//Physical file
					references.Add("/reference:\"" + assembly + '\"');

			String scriptHeader = ScriptArgs1.Replace("{Assemblies}", String.Join(" ", references.ToArray()));
			return scriptHeader + this._sourceCode;
		}
	}
}