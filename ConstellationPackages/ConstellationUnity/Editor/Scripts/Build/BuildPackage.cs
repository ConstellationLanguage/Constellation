using UnityEditor;
using UnityEngine;

public class BuildPackage {
	public static void ExportPackage()
	{
		AssetDatabase.ExportPackage("Assets", "../../Package/Constellation.unitypackage", ExportPackageOptions.Recurse);
	}
}
