using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Constellation
{
    [CreateAssetMenuAttribute(fileName = "New Script Assembly", menuName = "Constellation Script Assembly", order = 4)]
    public class ConstellationScriptsAssembly : ScriptableObject
    {
        public List<ConstellationScript> constellationScripts;

        public void SetScriptAssembly()
        {
            foreach(var constellationScript in constellationScripts)
            {
                constellationScript.ScriptAssembly = this;
            }
        }
    }
}
