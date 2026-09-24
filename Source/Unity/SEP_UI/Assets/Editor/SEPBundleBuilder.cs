using System.IO;
using UnityEditor;
using UnityEngine;

// Builds the sep_prefab asset bundle that SEP_UI_Loader loads with
// AssetBundle.LoadFromFile("GameData/SurfaceExperimentPackage/Resources/sep_prefab").
//
// The bundle is listed explicitly instead of relying on the assetBundleName tags:
// the window icons are tagged "sep_images", but SEP never loads that bundle, so
// they have to be pulled into sep_prefab as implicit dependencies.
//
// KSPedia (sep_kspedia.ksp) is a KSPAssets bundle with a dependency on the stock
// squadcore bundle; build it with KSPAssets > Asset Compiler instead.
public static class SEPBundleBuilder
{
	static readonly string[] Prefabs =
	{
		"Assets/Prefabs/SEP_Window.prefab",
		"Assets/Prefabs/SEP_Compact.prefab",
		"Assets/Prefabs/SEP_Vessel.prefab",
		"Assets/Prefabs/SEP_Experiment.prefab",
		"Assets/Prefabs/SEP_CelestialBody.prefab",
	};

	[MenuItem("SEP/Build sep_prefab (Windows)")]
	public static void BuildWindows()
	{
		Build(BuildTarget.StandaloneWindows64);
	}

	[MenuItem("SEP/Build sep_prefab (Linux)")]
	public static void BuildLinux()
	{
		Build(BuildTarget.StandaloneLinux64);
	}

	// Entry point for -batchmode -executeMethod SEPBundleBuilder.BuildAll
	public static void BuildAll()
	{
		BuildWindows();
		BuildLinux();
	}

	static void Build(BuildTarget target)
	{
		string outDir = Path.Combine("AssetBundles", target.ToString());
		Directory.CreateDirectory(outDir);

		AssetBundleBuild[] builds =
		{
			new AssetBundleBuild { assetBundleName = "sep_prefab", assetNames = Prefabs },
		};

		AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
			outDir, builds, BuildAssetBundleOptions.ChunkBasedCompression, target);

		if (manifest == null)
		{
			Debug.LogError("[SEPBundleBuilder] build failed for " + target);
			return;
		}

		foreach (string dep in AssetDatabase.GetDependencies(Prefabs, true))
			Debug.Log("[SEPBundleBuilder] " + target + " includes " + dep);

		Debug.Log("[SEPBundleBuilder] built " + Path.Combine(outDir, "sep_prefab"));
	}
}
