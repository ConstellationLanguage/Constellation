namespace Constellation
{
    [System.Serializable]
    public class InputData {
        public string Guid;
        public bool IsBright;
        public string Type;
        public string Description;
        public InputData(string _guid, bool _isBright, string _type, string _description)
        {
            Guid = _guid;
            IsBright = _isBright;
            Type = _type;
            Description = _description;
        }
    }
}