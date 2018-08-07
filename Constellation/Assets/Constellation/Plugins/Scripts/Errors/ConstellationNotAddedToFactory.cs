namespace Constellation {
    public class ConstellationNotAddedToFactory : ConstellationError, IConstellationError {
        private const string whatWentWrong = "ERROR: Node not found in factory \n\n" +
            "WHAT SHOULD I DO ? \n\n If this is a stock node please report it at: " + BugReport.BUG_REPORT_URL +
            "\n\n If this is a node you created look at your factory and check if the node exist.";
        private const string errorTitle = "Node not found in factory";
        private const int id = 902;
        public ConstellationNotAddedToFactory () {
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
            return true;
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