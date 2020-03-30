using UnityEngine;
namespace ConstellationEditor {
    public static class StartPanel {
        public static void Draw(ILoadable loadable) {
            if(GUILayout.Button("New")) {
                loadable.New();
            }

            if(GUILayout.Button("Load")) {
                loadable.Open(null);
            }

            if (GUILayout.Button("Wiki")) {
                Application.OpenURL("https://github.com/ConstellationLanguage/Constellation/wiki");
            }
        }
    }
}