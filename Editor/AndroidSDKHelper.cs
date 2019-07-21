/*
 * Copyright (c) 2019 KOGA Mitsuhiro
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */

#if PLATFORM_ANDROID
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public static class AndroidSDKHelper
{
#if UNITY_EDITOR_WIN
	[MenuItem("Android/Open terminal with android platform tools", false, 100)]
	private static void OpenTerminalWithAdbPathOnWindows()
	{
		var androidPlatformTools = GetAndroidPlatformTools();
		var cmd = Environment.GetEnvironmentVariable("ComSpec");

		if (!Directory.Exists(androidPlatformTools))
		{
			UnityEngine.Debug.LogWarningFormat("Android SDK not found: {0}", androidPlatformTools);
			return;
		}

		if (!File.Exists(cmd))
		{
			UnityEngine.Debug.LogWarningFormat("cmd not found: {0}", cmd);
			return;
		}

		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = cmd,
				UseShellExecute = true,
				Arguments = string.Format("/K set \"PATH={0};%PATH%\"", androidPlatformTools)
			}
		};
		process.Start();
	}

	[MenuItem("Android/Open terminal with android sdk tools", false, 110)]
	private static void OpenTerminalWithAndroidToolsPathOnWindows()
	{
		var toolInfo = GetAndroidTools();
		var androidSdkTools = toolInfo.Path;
		var jdkPath = GetJdkPath();
		var cmd = Environment.GetEnvironmentVariable("ComSpec");

		if (!Directory.Exists(androidSdkTools))
		{
			UnityEngine.Debug.LogWarningFormat("Android SDK not found: {0}", androidSdkTools);
			return;
		}

		if (!Directory.Exists(jdkPath))
		{
			UnityEngine.Debug.LogWarningFormat("JDK not found: {0}", jdkPath);
			return;
		}

		if (!File.Exists(cmd))
		{
			UnityEngine.Debug.LogWarningFormat("cmd not found: {0}", cmd);
			return;
		}

		var startInfo = new ProcessStartInfo
		{
			FileName = cmd,
			UseShellExecute = true,
			Arguments = string.Format("/K set \"PATH={0};%PATH%\" & set \"JAVA_HOME={1}\"", androidSdkTools, jdkPath)
		};
		if (toolInfo.UseEmbedded)
		{
			startInfo.Verb = "runas";
		}

		Process.Start(startInfo);
	}
#endif

#if UNITY_EDITOR_OSX
	[MenuItem("Android/Open terminal with android platform tools", false, 100)]
	private static void OpenTerminalWithAdbPathOnMac()
	{
		var androidPlatformTools = GetAndroidPlatformTools();

		if (!Directory.Exists(androidPlatformTools))
		{
			UnityEngine.Debug.LogFormat("Android SDK not found: {0}", androidPlatformTools);
			return;
		}

		var scriptFile = Path.GetTempFileName();
		var contents = new[]
		{
			"#!/bin/sh",
			string.Format("PATH=\"{0}:$PATH\" /bin/bash --login", androidPlatformTools)
		};
		CreateScript(scriptFile, contents);
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "open",
				Arguments = string.Format("-a Terminal {0}", scriptFile)
			}
		};
		process.Start();
	}

	[MenuItem("Android/Open terminal with android sdk tools", false, 110)]
	private static void OpenTerminalWithAndroidToolsPathOnMac()
	{
		var toolInfo = GetAndroidTools();
		var androidSdkTools = toolInfo.Path;
		var jdkPath = GetJdkPath();

		if (!Directory.Exists(androidSdkTools))
		{
			UnityEngine.Debug.LogWarningFormat("Android SDK not found: {0}", androidSdkTools);
			return;
		}

		if (!Directory.Exists(jdkPath))
		{
			UnityEngine.Debug.LogWarningFormat("JDK not found: {0}", jdkPath);
			return;
		}

		var scriptFile = Path.GetTempFileName();
		var contents = new[]
		{
			"#!/bin/sh",
			string.Format("PATH=\"{0}:$PATH\" JAVA_HOME=\"{1}\" /bin/bash --login", androidSdkTools, jdkPath)
		};
		CreateScript(scriptFile, contents);
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "open",
				Arguments = string.Format("-a Terminal {0}", scriptFile)
			}
		};
		process.Start();
	}

	[DllImport("libc", EntryPoint = "chmod", SetLastError = true)]
	private static extern int sys_chmod(string path, uint mode);

	private static void CreateScript(string path, string[] content)
	{
		File.WriteAllLines(path, content);
		if (File.Exists(path))
		{
			sys_chmod(path, Convert.ToUInt32("755", 8));
		}
	}
#endif

#if UNITY_2018_1_OR_NEWER
	private const BindingFlags BINDING_ATTR = BindingFlags.Static | BindingFlags.NonPublic;
	private static readonly MethodInfo mInfo = typeof(BuildPipeline).GetMethod("GetBuildToolsDirectory", BINDING_ATTR);

	private static string GetBuildToolsDirectory(BuildTarget target)
	{
		return mInfo.Invoke(null, new object[] {target}) as string;
	}

	private static string GetJdkPlatform()
	{
		switch (Application.platform)
		{
			case RuntimePlatform.WindowsEditor:
				return "Windows";
			case RuntimePlatform.OSXEditor:
				return "MacOS";
			case RuntimePlatform.LinuxEditor:
				return "Linux";
		}

		return string.Empty;
	}
#endif

	private static string GetJdkPath()
	{
		var jdkPath = string.Empty;
#if UNITY_2018_1_OR_NEWER
		var jdkUseEmbedded = EditorPrefs.GetBool("JdkUseEmbedded", false);
		if (!jdkUseEmbedded)
		{
			jdkPath = EditorPrefs.GetString("JdkPath");
			if (!Directory.Exists(jdkPath))
			{
				jdkUseEmbedded = true;
			}
		}

		if (jdkUseEmbedded)
		{
			var buildToolsDirectory = GetBuildToolsDirectory(BuildTarget.Android);
			jdkPath = Path.Combine(buildToolsDirectory, Path.Combine("OpenJDK", GetJdkPlatform()));
		}
#else
		jdkPath = EditorPrefs.GetString("JdkPath");
#endif
		return jdkPath;
	}

	private struct ToolInfo
	{
		public string Path;
		public bool UseEmbedded;
	}

	private static ToolInfo GetAndroidSdkRoot()
	{
		var androidPlatformTools = string.Empty;
		var sdkUseEmbedded = false;
#if UNITY_2019_1_OR_NEWER
		sdkUseEmbedded = EditorPrefs.GetBool("SdkUseEmbedded", false);
		if (!sdkUseEmbedded)
		{
			androidPlatformTools = EditorPrefs.GetString("AndroidSdkRoot");
			if (!Directory.Exists(androidPlatformTools))
			{
				sdkUseEmbedded = true;
			}
		}

		if (sdkUseEmbedded)
		{
			var androidPlaybackEngineDirectory =
				BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None);
			androidPlatformTools = Path.Combine(androidPlaybackEngineDirectory, "SDK");
		}
#else
		androidPlatformTools = EditorPrefs.GetString("AndroidSdkRoot");
#endif
		return new ToolInfo
		{
			Path = androidPlatformTools,
			UseEmbedded = sdkUseEmbedded
		};
	}

	private static string GetAndroidPlatformTools()
	{
		var sdkRoot = GetAndroidSdkRoot().Path;
		return Path.Combine(sdkRoot, "platform-tools");
	}

	private static ToolInfo GetAndroidTools()
	{
		var toolInfo = GetAndroidSdkRoot();
		return new ToolInfo
		{
			Path = Path.Combine(toolInfo.Path, Path.Combine("tools", "bin")),
			UseEmbedded = toolInfo.UseEmbedded
		};
	}
}
#endif
