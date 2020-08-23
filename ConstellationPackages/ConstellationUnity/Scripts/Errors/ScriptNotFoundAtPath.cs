namespace Constellation
{
    public class ScriptNotFoundAtPath : ConstellationError, IConstellationError
    {
        private const string whatWentWrong = "ERROR: Script not found\n\n" +
            "HINT : This error usally happens if you moved/deleted a script opened in the constellation editor. \n\n" +
            "HOW TO FIX? : Just click recover and everything should be fine.";
        private const string errorTitle = "Script Not Found At Path";
        private const int id = 901;
        private string path;
        public ScriptNotFoundAtPath(string _path)
        {
            path = _path;
        }

        public string GetErrorTitle()
        {
            return errorTitle;
        }

        public string GetErrorMessage()
        {
            return whatWentWrong;
        }

        public string GetFormatedError()
        {
            return errorTitle + " (" + id + ") " + "\n\n" + whatWentWrong + "\n\n path: " + path;
        }

        public bool IsReportable()
        {
            return false;
        }

        public bool IsCloseEditor()
        {
            return false;
        }

        public bool IsIgnorable()
        {
            return false;
        }

        public int GetID()
        {
            return id;
        }

        public override IConstellationError GetError()
        {
            return this;
        }
    }
}