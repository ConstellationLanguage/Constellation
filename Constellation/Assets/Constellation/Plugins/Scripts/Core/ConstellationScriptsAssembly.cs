using System.Collections.Generic;
using UnityEngine;
using Constellation;

namespace Constellation.Unity3D
{
    [CreateAssetMenuAttribute(fileName = "New Script Assembly", menuName = "Constellation Script Assembly", order = 4)]
    public class ConstellationScriptsAssembly : ScriptableObject
    {
        public List<ConstellationBehaviourScript> ConstellationScripts;
        public List<ConstellationTutorialScript> ConstellationTutorials;

        public void SetScriptAssembly()
        {
            foreach(var constellationScript in ConstellationScripts)
            {
                constellationScript.ScriptAssembly = this;
            }
        }

        public ConstellationScriptData [] GetAllScriptData()
        {
            var scriptDatas = new List<ConstellationScriptData>();
            foreach (var constellationScript in ConstellationScripts)
            {
                scriptDatas.Add(constellationScript.script);
            }

            return scriptDatas.ToArray();
        }
    }
}
