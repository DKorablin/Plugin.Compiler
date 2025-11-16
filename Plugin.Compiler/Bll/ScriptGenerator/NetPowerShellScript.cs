using System;
using System.Collections.Generic;
using System.IO;

namespace Plugin.Compiler.Bll.ScriptGenerator
{
	internal class NetPowerShellScript
	{
		private const String ScriptArgs2 = @"
# --- Configuration ---
$ProjectName = [System.IO.Path]::GetFileNameWithoutExtension($MyInvocation.MyCommand.Path)
$ScriptRoot  = $PSScriptRoot
$ProjectDir  = Join-Path $ScriptRoot $ProjectName
$OutLog      = Join-Path $ScriptRoot ($ProjectName + ""_out.log"")

# --- .props file content ---
# This is safe as a here-string because it's simple XML
$PropsContent = @""
<Project>
	<ItemGroup>{Assemblies}
	</ItemGroup>
	<PropertyGroup>
		<PauseOnExit>false</PauseOnExit>
	</PropertyGroup>
</Project>
""@

# The source code, converted to a safe string to avoid parsing errors.
$Base64CsCode = ""{SourceCode}""

# --- Main Script Logic ---
dotnet new winforms -n $ProjectName --force | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Error ""Failed to create dotnet project.""
    exit
}

try {
    Set-Location $ProjectDir

    $Bytes = [System.Convert]::FromBase64String($Base64CsCode)
    [System.IO.File]::WriteAllBytes(""Program.cs"", $Bytes)

    $PropsContent | Out-File ""Directory.Build.props"" -Encoding utf8

    $BuildLog = ""build_errors.log""
    dotnet build -c Release /p:OutputPath=""bin"" -fl1 ""/flp1:logfile=$BuildLog;errorsonly"" | Out-Null

    if ($LASTEXITCODE -ne 0) {
        Get-Content $BuildLog
    } else {
        $ExePath = "".\bin\$ProjectName.exe""
        & $ExePath > $OutLog
        Get-Content $OutLog
        Remove-Item $ExePath
    }
}
finally {
    # --- Cleanup ---
    Set-Location $ScriptRoot
    Remove-Item -Path $ProjectDir -Recurse -Force
}";
		private readonly AssemblyCollection _references;
		private readonly String _sourceCode;

		public NetPowerShellScript(AssemblyCollection references, String sourceCode)
		{
			this._references = references;
			this._sourceCode = sourceCode;
		}

		public String GenerateScript()
		{
			List<String> references = new List<String>();
			foreach(String assembly in this._references)
				if(File.Exists(assembly))
					references.Add(@$"
<Reference Include=""{Path.GetFileNameWithoutExtension(assembly)}"">
	<HintPath>{assembly}</HintPath>
</Reference>
");
			String script = ScriptArgs2
				.Replace("{Assemblies}", String.Join("", references.ToArray()))
				.Replace("{SourceCode}",Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(this._sourceCode)));
			return script;
		}
	}
}