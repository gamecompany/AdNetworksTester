using UnityEngine;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Threading;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Callbacks;
//If you get a "missing assembly reference" on the below line, this is because you do not have Unity iOS Build Support installed.
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif


[InitializeOnLoad]
public class mNectarUnityPlugin : MonoBehaviour
{
	static mNectarUnityPlugin ()
	{

	}

	static void Update ()
	{
		// Called once per frame, by the editor.
	}
#if UNITY_IOS
    [PostProcessBuildAttribute]
	public static void OnPostprocessBuild (BuildTarget target, string pathToBuiltProject)
	{
		if (target != BuildTarget.iOS)
			return;

		//NOTE: these patches are not robust to Xcode generation by Append, only Rebuild
		PatchXcodeProject(pathToBuiltProject);

		//SelectXcodeBuildConfiguration(pathToBuiltProject, "Release");

	}
#endif

	public static string checkPBXProjectPath (string projectPath)
	{
		//In versions of Unity < 5.1.3p2,
		// the xcode project path returned by PBXProject.GetPBXProjectPath
		// is incorrect. We fix it here.

		string projectBundlePath = Path.GetDirectoryName(projectPath);

		if (projectBundlePath.EndsWith(".xcodeproj"))
			return projectPath;
		else
			return projectBundlePath + ".xcodeproj/project.pbxproj";
	}
#if UNITY_IOS
    public static void PatchXcodeProject (string pathToBuiltProject)
	{
		PBXProject project = new PBXProject();

		string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);

		projectPath = checkPBXProjectPath(projectPath);

		project.ReadFromFile(projectPath);

		string guid = project.TargetGuidByName("Unity-iPhone");

		project.AddFileToBuild(guid, project.AddFile("usr/lib/libicucore.tbd", "Frameworks/libicucore.tbd", PBXSourceTree.Sdk));


		project.WriteToFile(projectPath);
	}
#endif
	public static void SelectXcodeBuildConfiguration (string pathToBuiltProject, string configuration)
	{
		string schemePath = GetDefaultSharedSchemePath(pathToBuiltProject);

		if (!File.Exists(schemePath))
			TriggerXcodeDefaultSharedSchemeGeneration(pathToBuiltProject);

		if (!File.Exists(schemePath))
		{
			//Debug.Log("Xcode scheme project generation failed. You will need to manually select the 'Release' configuration. The deployed iOS application performance will be disastrous, otherwise.");
			return;
		}

		XmlDocument xml = new XmlDocument();
		xml.Load(schemePath);
		XmlNode node = xml.SelectSingleNode("Scheme/LaunchAction");
		node.Attributes["buildConfiguration"].Value = configuration;
		xml.Save(schemePath);
	}
	public static void TriggerXcodeDefaultSharedSchemeGeneration (string pathToBuiltProject)
	{
		// Launch Xcode to trigger the scheme generation.
		ProcessStartInfo proc = new ProcessStartInfo();

		proc.FileName = "open";
		proc.WorkingDirectory = pathToBuiltProject;
		proc.Arguments = "Unity-iPhone.xcodeproj";
		proc.WindowStyle = ProcessWindowStyle.Hidden;
		proc.UseShellExecute = true;
		Process.Start(proc);

		Thread.Sleep(3000);
	}

	public static string GetDefaultSharedSchemePath (string pathToBuiltProject)
	{
		return Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme");
	}

}

#endif
