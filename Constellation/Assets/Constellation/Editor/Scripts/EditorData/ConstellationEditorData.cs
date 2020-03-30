using UnityEngine;
using System.Collections.Generic;
using Constellation.Unity3D;

namespace ConstellationEditor {
	public class ConstellationEditorData : ScriptableObject {
		public List<ConstellationScriptInfos> LastOpenedConstellationPath;
		public float SliderX;
		public float SliderY;
		public ClipBoard clipBoard;
		public ConstellationExampleData ExampleData;
        public ConstellationScriptsAssembly ScriptAssembly;
		public bool IsSafeProgramming = false;
    }
}