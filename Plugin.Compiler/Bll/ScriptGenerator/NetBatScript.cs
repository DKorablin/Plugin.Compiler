using System;
using System.Collections.Generic;
using System.IO;

namespace Plugin.Compiler.Bll.ScriptGenerator
{
	internal class NetBatScript
	{
		private const String ScriptArgs1 = @"
/*
@echo off && cls

set PROJECT_NAME=%~n0
set OUT_LOG=%PROJECT_NAME%_out.log

dotnet new winforms -n ""%PROJECT_NAME%"" --force >nul
if errorlevel 1 (
	exit /b
)

copy ""%~f0"" ""%PROJECT_NAME%\Program.cs"" >nul
cd ""%PROJECT_NAME%""

(
	echo ^<Project^>
	echo 	^<ItemGroup^>{Assemblies}
	echo 	^</ItemGroup^>
	echo 	^<PropertyGroup^>
	echo 		^<PauseOnExit^>false^</PauseOnExit^>
	echo 	^</PropertyGroup^>
	echo ^</Project^>
) > ""Directory.Build.props""

dotnet build -c Release /p:OutputPath=""bin"" -fl1 ""/flp1:logfile=%OUT_LOG%;errorsonly"" >nul
if errorlevel 1 (
	type ""%OUT_LOG%""
) else (
	""bin\%PROJECT_NAME%.exe"" > %OUT_LOG%
	type ""%OUT_LOG%""
	del ""bin\%PROJECT_NAME%.exe""
)

cd ..
rmdir /s /q ""%PROJECT_NAME%""
exit /b 0

*/
";

		private readonly AssemblyCollection _references;
		private readonly String _sourceCode;

		public NetBatScript(AssemblyCollection references, String sourceCode)
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
	echo		^<Reference Include=""{Path.GetFileNameWithoutExtension(assembly)}""^>
	echo			^<HintPath^>{assembly}^</HintPath^>
	echo		^</Reference^>
");
			String scriptHeader = ScriptArgs1.Replace("{Assemblies}", String.Join("", references.ToArray()));
			return scriptHeader + this._sourceCode;
		}
	}
}