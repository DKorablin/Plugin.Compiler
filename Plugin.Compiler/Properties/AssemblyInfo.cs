using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("425f8b0c-f049-44ee-8375-4cc874d6bf94")]
[assembly: System.CLSCompliant(true)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/project/Default.aspx?File=102")]
#else

[assembly: AssemblyDescription("Compile runtime .NET code for various functions")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2010-2024")]
#endif

/*if $(ConfigurationName) == Release (
..\..\..\..\..\ILMerge.exe  "/out:$(ProjectDir)..\..\bin\$(TargetFileName)" "$(TargetPath)" "$(TargetDir)FastColoredTextBox.dll" "/lib:..\..\..\..\SAL\bin"
)*/