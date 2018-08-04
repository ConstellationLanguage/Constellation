namespace Constellation
{
    public class UnknowError : ConstellationError, IConstellationError
    {
        private const string errorMessage = "Constellation Error: An unexpected error happened. You can report this error on https://github.com/ConstellationLanguage/Constellation/ \n"
            + "Quick tips to help us \n"
            + "1. Include the current unity version you are using \n"
            + "2. The steps to reproduce \n"
            + "3. The error displayed in the unity console \n"
            + "4. Any other infos might be usefull";

        private const string errorTitle = "Unknow error from: ";
        private string errorSource;
        private const int id = 000;
        public int GetID()
        {
            return id;
        }
        public UnknowError(string _errorSource)
        {
            errorSource = _errorSource;
        }

        public string GetErrorTitle()
        {
            return errorTitle + errorSource;
        }

        public bool IsReportable()
        {
            return true;
        }

        public bool IsCloseEditor()
        {
            return true;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }

        public bool IsIgnorable()
        {
            return false;
        }

        public string GetFormatedError()
        {
            return errorSource + errorMessage;
        }

        public override IConstellationError GetError()
        {
            return this;
        }
    }
}