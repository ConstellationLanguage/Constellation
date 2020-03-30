using UnityEngine;
namespace ConstellationEditor
{
    [System.Serializable]
    public class ConstellationsTabPanel
    {

        const int panelHeight = 35;

        private ConstellationScriptInfos removeNode;

        public ConstellationsTabPanel()
        {
        }

        public ConstellationScriptInfos Draw(ConstellationScriptInfos[] scriptsInfos)
        {
            GUI.color = Color.white;
            GUILayout.BeginHorizontal();

            foreach (var scriptInfos in scriptsInfos)
            {
                var constellationPath = scriptInfos.ScriptPath.Split('/');
                var name = constellationPath[constellationPath.Length - 1].Split('.')[0];
                if (scriptInfos.IsIstance == true)
                {
                    GUI.color = Color.yellow;
                    constellationPath = scriptInfos.InstancePath.Split('/');
                    name = constellationPath[constellationPath.Length - 1].Split('.')[0];
                }

                if (GUILayout.Button(name, "MiniToolbarButton", GUILayout.MaxWidth(125), GUILayout.MinWidth(125)))
                {
                    return scriptInfos;
                }

                if (GUILayout.Button("X", "MiniToolbarButton", GUILayout.MaxWidth(20), GUILayout.MinWidth(20)))
                {
                    removeNode = scriptInfos;
                }
                GUI.color = Color.grey;
                GUILayout.Space(10);
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            return null;
        }

        public int GetHeight()
        {
            return panelHeight;
        }

        public ConstellationScriptInfos ConstellationToRemove()
        {
            var nodeToRemove = removeNode;
            removeNode = null;
            return nodeToRemove;
        }
    }
}