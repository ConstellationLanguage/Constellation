namespace Constellation {
    public class ConstellationScriptDataDoesNotExist : ConstellationError, IConstellationError {
        private const string whatWentWrong = "ERROR: There was something wrong with the constellation editor. This error is preventing any data loss. \n\n" +
            "WHAT SHOULD I DO ? : Please report it at " + BugReport.BUG_REPORT_URL;
        private const string errorTitle = "Constellation Data not found";
        private const int id = 900;
        public ConstellationScriptDataDoesNotExist () {
        }

        public string GetErrorTitle () {
            return errorTitle;
        }

        public string GetErrorMessage () {
            return whatWentWrong;
        }

        public string GetFormatedError()
        {
            return errorTitle + " (" + id + ") " + "\n\n" + whatWentWrong;
        }

        public bool IsIgnorable () {
            return false;
        }

        public bool IsCloseEditor() {
            return false;
        }

        public bool IsReportable() {
            return false;
        }

        public int GetID () {
            return id;
        }

        public override IConstellationError GetError () {
            return this;
        }
    }
}