namespace ConstellationEditor {
    [System.Serializable]
    public class ConstellationScriptInfos {
        public enum ConstellationScriptTag {NoTag, Tutorial, Nestable};
        public ConstellationScriptTag ScriptTag;
        public bool IsIstance;
        public string InstancePath;
        public string ScriptPath;

        public ConstellationScriptInfos(string scriptPath, ConstellationScriptTag constellationScriptTag, bool isInstance, string instanceObject = "") {
            InstancePath = instanceObject;
            ScriptPath = scriptPath;
            ScriptTag = constellationScriptTag;
            IsIstance = isInstance;
        }

    }
}