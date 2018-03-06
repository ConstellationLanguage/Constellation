using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class PlayBar {
        public PlayBar () { }

        public void Draw () {
            if (GUILayout.Button ("Play example", EditorStyles.toolbarButton, GUILayout.Width (90))) {

            }
        }
    }
}