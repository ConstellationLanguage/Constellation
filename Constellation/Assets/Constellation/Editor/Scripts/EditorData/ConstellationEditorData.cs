using UnityEngine;
using System.Collections.Generic;
using Constellation.Unity3D;

namespace ConstellationEditor {
	public class ConstellationEditorData : ScriptableObject {
		public List<string> LastOpenedConstellationPath;
		public EditorUndoService EditorUndoService;
		public float SliderX;
		public float SliderY;
		public ClipBoard clipBoard;
		public List<ConstellationInstanceObject> CurrentInstancePath;
		public ConstellationExampleData ExampleData;
        public ConstellationScriptsAssembly ScriptAssembly;
		public bool IsSafeProgramming = false;
    }
}