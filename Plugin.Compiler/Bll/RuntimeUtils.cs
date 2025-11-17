using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace Plugin.Compiler.Bll
{
	internal static class RuntimeUtils
	{
		public static String CurrentRuntimeVersion
		{
			get
			{
				var version = Environment.Version;
				// Map CLR version to framework version
				switch(version.Major)
				{
				case 1:
					return version.Minor == 0 ? "v1.0" : "v1.1";
				case 2:
					return "v2.0"; // Could also be 3.0 or 3.5, but they all use CLR 2.0
				case 4:
					return "v4.0"; // Could be 4.0 through 4.8.1, but they all use CLR 4.0
				default://.NET 5+
					return version.ToString();
				}
			}
		}

		/// <summary>Get an array of installed framework versions</summary>
		/// <returns>Array of installed version numbers</returns>
		public static IEnumerable<String> GetInstalledFrameworkVersions()
		{
#if NETFRAMEWORK
			RegistryKey componentsKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Net Framework Setup\NDP\");
			foreach(String keyName in componentsKey.GetSubKeyNames())
				if(keyName.StartsWith("v") && keyName.Contains("."))
					if(keyName.Split('.').Length > 2)
						yield return keyName.Substring(0, keyName.LastIndexOf('.'));
					else
						yield return keyName;
#else
			foreach(var versionPath in RuntimeUtils.EnumerateNetDirectories())
				yield return versionPath.Key.ToString();
#endif
		}

		public static IEnumerable<KeyValuePair<Version, String>> EnumerateNetDirectories()
		{
			String dotnetPath = Environment.GetEnvironmentVariable("DOTNET_ROOT");
			if(String.IsNullOrEmpty(dotnetPath))
			{
				String programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				dotnetPath = Path.Combine(programFiles, "dotnet");
			}

			String sdkPath = Path.Combine(dotnetPath, "sdk");
			if(Directory.Exists(sdkPath))
			{
				foreach(String dir in Directory.GetDirectories(sdkPath))
				{
					String dirVersion = Path.GetFileName(dir);
					if(Version.TryParse(dirVersion.Split('-')[0], out var version))
						yield return new KeyValuePair<Version, String>(version, dir);
				}
			}

			// Also check shared runtimes
			String runtimePath = Path.Combine(dotnetPath, "shared", "Microsoft.NETCore.App");
			if(Directory.Exists(runtimePath))
			{
				foreach(String dir in Directory.GetDirectories(runtimePath))
				{
					String dirVersion = Path.GetFileName(dir);
					if(Version.TryParse(dirVersion, out var version))
						yield return new KeyValuePair<Version, String>(version, dir);
				}
			}
		}
	}
}