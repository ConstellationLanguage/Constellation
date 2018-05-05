namespace Constellation {
    public class UnknowError : ConstellationError, IConstellationError {
        private const string errorMessage = "Constellation Error: Something went wrong but we cannot figure out what it is. Please report this error on https://github.com/ConstellationLanguage/Constellation/issues";
        private const string errorTitle = "Unknow Error from: ";
        private string errorSource;
        private const int id = 000;
        public int GetID () {
            return id;
        }
        public UnknowError (string _errorSource) {
            errorSource = _errorSource;
        }

        public string GetErrorTitle () {
            return errorTitle + errorSource;
        }

        public string GetErrorMessage () {
            return errorMessage;
        }

        public bool IsIgnorable () {
            return false;
        }

        public string GetFormatedError () {
            return errorSource + errorMessage;
        }

        public override IConstellationError GetError () {
            return this;
        }
    }
}