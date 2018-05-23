using UnityEngine;
<<<<<<< HEAD
namespace ConstellationEditor {
    public static class StartPanel {
        public static void Draw(ILoadable loadable) {
                if(GUILayout.Button("New")) {
                    loadable.New();
                }

                if(GUILayout.Button("Load")) {
                    loadable.Open("");
                }

                if(GUILayout.Button("Wiki")) {
                    Application.OpenURL(ConstellationLinks.Wiki);
                }
        }
=======
public static class StartPanel
{
    public static void Draw(ILoadable loadable)
    {
            if(GUILayout.Button("New")) {
                loadable.New();
            }
            if(GUILayout.Button("Load")) {
                loadable.Open("");
            }
            if(GUILayout.Button("Wiki")) {
                Application.OpenURL("https://github.com/ConstellationLanguage/Constellation/wiki");
            }
>>>>>>> e459bba71f871cae96fbb70339d360abf83f682e
    }
}