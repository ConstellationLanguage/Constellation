using UnityEngine;
namespace ConstellationEditor {
    public class ConstellationsTabPanel {
        private IGUI GUI;
        private string removeNode = "";

        public ConstellationsTabPanel (IGUI _gui) {
            GUI = _gui;
        }

        public string Draw (string[] constellationsPath, ConstellationInstanceObject[] instancesPath) {
                GUI.SetColor (Color.white);
                GUILayout.BeginHorizontal();

                foreach (var path in constellationsPath)
                {
                    var constellationPath = path.Split('/');
                    var name = constellationPath[constellationPath.Length - 1].Split('.')[0];
                    if (instancesPath != null)
                        foreach (var instanceName in instancesPath)
                        {
                            if (path == instanceName.InstancePath)
                                GUI.SetColor(Color.yellow);
                        }

                    if (GUILayout.Button(name, "MiniToolbarButton", GUILayout.MaxWidth(125), GUILayout.MinWidth(125)))
                    {
                        return path;
                    }

                    if (GUILayout.Button("X", "MiniToolbarButton", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                    {
                        removeNode = path;
                    }
                    GUI.SetColor(Color.grey);
                    GUILayout.Space(10);
                }
                GUI.SetColor(Color.white);
                GUILayout.EndHorizontal();
            return null;
        }

        public string ConstellationToRemove () {
            var nodeToRemove = removeNode;
            removeNode = null;
            return nodeToRemove;
        }
    }
}