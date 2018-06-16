using UnityEditor;
using UnityEngine;

public class BuildPackage {
	[MenuItem("Examples/Location of Unity application")]
	public static void ExportPackage()
	{
		AssetDatabase.ExportPackage("Assets", "../../Package/Constellation.unitypackage", ExportPackageOptions.Recurse);
	}

}
