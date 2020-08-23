using UnityEditor;
using UnityEngine;

namespace ConstellationUnityEditor {
    public delegate void PlayExample();
    public class PlayBar {
        public PlayBar () {
        }
        
        public void Draw (PlayExample PlayExample) {
            if (GUILayout.Button ("Play example", EditorStyles.toolbarButton, GUILayout.Width (90))) {
                PlayExample();
            }
        }
    }
}