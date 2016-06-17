using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.IO;
using System.Diagnostics;

public class VunglePostBuilder : MonoBehaviour
{
	private static string _postBuildDirectoryKey { get { return "VunglePostBuildPath-" + PlayerSettings.productName; } }
	private static string postBuildDirectory
	{
		get
		{
			return EditorPrefs.GetString( _postBuildDirectoryKey );
		}
		set
		{
			EditorPrefs.SetString( _postBuildDirectoryKey, value );
		}
	}


	[PostProcessBuild( 800 )]
	private static void onPostProcessBuildPlayer( BuildTarget target, string pathToBuiltProject )
	{
		if (target.ToString().Equals("iPhone") || target.ToString().Equals("iOS"))
		{
			postBuildDirectory = pathToBuiltProject;

			// grab the path to the postProcessor.py file
			var scriptPath = Path.Combine( Application.dataPath, "Supersonic/Editor/VunglePostProcessor.py");

			// sanity check
			if( !File.Exists( scriptPath ) )
			{
				UnityEngine.Debug.LogError( "Vungle post builder could not find the VunglePostProcessor.py file. Did you accidentally delete it?" );
				return;
			}

			var pathToNativeCodeFiles = Path.Combine( Application.dataPath, "Plugins/ios/SUVungleAdapter/Vungle/VungleSDK.embeddedframework" );

			var args = string.Format( "\"{0}\" \"{1}\" \"{2}\"", scriptPath, pathToBuiltProject, pathToNativeCodeFiles );
			var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "python2.6",
					Arguments = args,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				}
			};

			proc.Start();
			proc.WaitForExit();

			UnityEngine.Debug.Log( "Vungle post processor completed" );
		}
	}
}
