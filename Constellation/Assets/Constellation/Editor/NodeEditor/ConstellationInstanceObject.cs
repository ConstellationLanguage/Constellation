namespace ConstellationEditor {
    [System.Serializable]
    public class ConstellationInstanceObject {
        public ConstellationInstanceObject(string instanceObject, string scriptPath) {
            InstancePath = instanceObject;
            ScriptPath = scriptPath;
        }
        public string InstancePath;
        public string ScriptPath;
    }
}