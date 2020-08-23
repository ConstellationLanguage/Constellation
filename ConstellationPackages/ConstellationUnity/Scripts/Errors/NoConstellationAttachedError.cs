namespace Constellation.Unity3D
{
    public class NoConstellationAttached : ConstellationError, IConstellationError
    {
        private const string whatWentWrong = "ERROR: You must make sure a ConstellationBehaviour has a Constellation attached to it \n\n" +
            "HINT: If you are trying to instantiate the behaviour at runtime disable the gameobject and enable it once the constellation is set on it \n\n" +
            "HOW TO FIX: Attach a Constellation script on this GameObject:";
        private const string errorTitle = "No constellation attached on constellation behaviour";
        private ConstellationComponent constellationComponent;
        private const int id = 101;

        public NoConstellationAttached(ConstellationComponent _constellationComponent)
        {
            constellationComponent = _constellationComponent;
            constellationComponent.HasThrownError(this);
        }

        public string GetErrorTitle()
        {
            return errorTitle;
        }

        public string GetErrorMessage()
        {
            return whatWentWrong + constellationComponent.gameObject.name;
        }

        public string GetFormatedError()
        {
            return errorTitle + " (" + id + ") " + "\n\n" + whatWentWrong + constellationComponent.gameObject.name;
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
            return true;
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