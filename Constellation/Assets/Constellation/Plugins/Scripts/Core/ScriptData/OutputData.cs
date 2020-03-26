namespace Constellation
{
    [System.Serializable]
    public class OutputData
    {
        public string Guid;
        public bool IsWarm;
        public string Type;
        public string Description;

        public OutputData(string _guid, bool _isWarm, string _type, string _description)
        {
            Guid = _guid;
            IsWarm = _isWarm;
            Type = _type;
            Description = _description;
        }
    }
}