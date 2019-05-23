using Constellation;
using UnityEditor;
using UnityEngine;

// makes sure that the static constructor is always called in the editor.
[InitializeOnLoad]
public class DragConstellation : Editor {
    private static bool isDraggable;
    static DragConstellation () {
        // Adds a callback for when the hierarchy window processes GUI events
        // for every GameObject in the heirarchy.
        EditorApplication.projectWindowItemOnGUI += OnGUI;
        isDraggable = true;
    }

    static void OnGUI (string s, Rect r) {
        // happens when an acceptable item is released over the GUI window
        if (Event.current.type == EventType.MouseUp) {
            isDraggable = true;
        }

        if (EditorWindow.mouseOverWindow == null)
            return;

        //No very cool to have a try catch for this but I cannot to otherwise
        try {
            #pragma warning disable 0618
            if (EditorWindow.mouseOverWindow.title == "Inspector") {
            #pragma warning restore 0618
                // get all the drag and drop information ready for processing.
                if (isDraggable) {
                    DragAndDrop.AcceptDrag ();
                    // used to emulate selection of new objects.
                    // run through each object that was dragged in.
                    foreach (var objectRef in DragAndDrop.objectReferences) {
                        // if the object is the particular asset type...
                        if (objectRef is ConstellationScript) {
                            // we create a new GameObject using the asset's name.
                            var gameObject = Selection.gameObjects[0];
                            // we attach component X, associated with asset X.
                            var constellations = gameObject.GetComponents (typeof (ConstellationBehaviour));
                            foreach (ConstellationBehaviour constellation in constellations) {
                                if (constellation.GetConstellationData () == null) {
                                    constellation.SetConstellationScript (objectRef as ConstellationScript);
                                    isDraggable = false;
                                    return;
                                }
                            }
                            var componentX = gameObject.AddComponent<ConstellationBehaviour> ();
                            // we place asset X within component X.
                            componentX.SetConstellationScript (objectRef as ConstellationScript);
                            isDraggable = false;
                            // add to the list of selected objects.
                        }
                    }
                }
            } else {
                isDraggable = true;
            }
        } catch {

        }
    }
}