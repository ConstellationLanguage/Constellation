using UnityEngine;
namespace ConstellationEditor {
    [System.Serializable]
    public class ConstellationsTabPanel {

        const int panelHeight = 35;

        private string removeNode = "";

        public ConstellationsTabPanel () {
        }

        public string Draw (string[] constellationsPath, ConstellationInstanceObject[] instancesPath) {
                GUI.color = Color.white;
                GUILayout.BeginHorizontal();

                foreach (var path in constellationsPath)
                {
                    var constellationPath = path.Split('/');
                    var name = constellationPath[constellationPath.Length - 1].Split('.')[0];
                    if (instancesPath != null)
                        foreach (var instanceName in instancesPath)
                        {
                            if (path == instanceName.InstancePath)
                                GUI.color = Color.yellow;
                        }

                    if (GUILayout.Button(name, "MiniToolbarButton", GUILayout.MaxWidth(125), GUILayout.MinWidth(125)))
                    {
                        return path;
                    }

                    if (GUILayout.Button("X", "MiniToolbarButton", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                    {
                        removeNode = path;
                    }
                    GUI.color = Color.grey;
                    GUILayout.Space(10);
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            return null;
        }

        public int GetHeight()
        {
            return panelHeight;
        }

        public string ConstellationToRemove () {
            var nodeToRemove = removeNode;
            removeNode = null;
            return nodeToRemove;
        }
    }
}