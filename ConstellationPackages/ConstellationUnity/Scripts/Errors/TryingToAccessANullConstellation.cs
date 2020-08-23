namespace Constellation.Unity3D
{
    public class TryingToAccessANullCosntellation : ConstellationError, IConstellationError
    {
        private const string whatWentWrong = "ERROR: You are trying to access a constellation on a Constellation behaviour but none where added\n\n" +
            "HINT: You must make sure a ConstellationBehaviour has a Constellation attached to it\n\n" +
            "HOW TO FIX: Attach a Constellation script on this GameObject:";
        private const string errorTitle = "Trying to access a null constellation";
        private const int id = 100;
        private ConstellationComponent constellationBehaviour;
        public TryingToAccessANullCosntellation(ConstellationComponent _constellationBehaviour)
        {
            constellationBehaviour = _constellationBehaviour;
            constellationBehaviour.HasThrownError(this);
        }

        public string GetErrorTitle()
        {
            return errorTitle;
        }

        public string GetErrorMessage()
        {
            return whatWentWrong + constellationBehaviour.name;
        }
        public string GetFormatedError()
        {
            return errorTitle + " (" + id + ") " + "\n\n" + whatWentWrong + constellationBehaviour.gameObject.name;
        }

        public bool IsCloseEditor()
        {
            return false;
        }

        public bool IsIgnorable()
        {
            return true;
        }

        public bool IsReportable()
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