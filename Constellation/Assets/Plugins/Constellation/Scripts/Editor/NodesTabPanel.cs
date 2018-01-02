using UnityEngine;
public class ConstellationTabPanel {
    private IGUI GUI;
    private string removeNode = "";

    public ConstellationTabPanel (IGUI _gui) {
        GUI = _gui;
    }

    public string Draw (string[] constellationNames) {
        GUI.SetColor (Color.white);
        GUILayout.BeginHorizontal ();

        foreach (var path in constellationNames) {
            var names = path.Split ('/');
            var name = names[names.Length - 1].Split('.')[0];
            if (GUILayout.Button (name, "MiniToolbarButton", GUILayout.MaxWidth (125), GUILayout.MinWidth (125))) {
                return path;
            }

            if (GUILayout.Button ("X", "MiniToolbarButton", GUILayout.MaxWidth (20), GUILayout.MinWidth (20))) {
                removeNode = path;
            }
            GUI.SetColor (Color.grey);
            GUILayout.Space (10);

        }
        GUI.SetColor (Color.white);
        GUILayout.EndHorizontal ();
        return null;
    }

    public string NodeToRemove () {
        var nodeToRemove = removeNode;
        removeNode = null;
        return nodeToRemove;
    }
}