namespace Plugin.Compiler.Bll.ScriptGenerator
{
	/// <summary>The type of the script to generate</summary>
	internal enum ScriptType
	{
		Unknown = 0,
		/// <summary>Compile assembly using currently supported CLR version</summary>
		Assembly = 1,
		/// <summary>Generate .NET Framework (&lt;=4.8) batch script</summary>
		BatFramework = 2,
		/// <summary>Generate .NET (4.8+) batch script</summary>
		BatNet = 3,
		/// <summary>Generate .NET Framework (&lt;=4.8) PowerShell script</summary>
		PowerShellFramework = 4,
		/// <summary>Generate .NET (4.8+) PowerShell script</summary>
		PowerShellNet = 5,
	}
}