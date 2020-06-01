namespace Constellation
{
    [System.Serializable]
    public class OutputData
    {
        public string Guid;
        public bool IsBright;
        public string Type;
        public string Description;

        public OutputData(string _guid, bool _isWarm, string _type, string _description)
        {
            Guid = _guid;
            IsBright = _isWarm;
            Type = _type;
            Description = _description;
        }
    }
}