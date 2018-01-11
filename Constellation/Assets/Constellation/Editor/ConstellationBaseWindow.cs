namespace ConstellationEditor
{
    public class ConstellationBaseWindow : ExtendedEditorWindow, ILoadable
    {
        protected ConstellationEditorDataService scriptDataService;
        static protected bool canDrawUI = false;

        public void Awake()
        {
            Setup();
            canDrawUI = false;
        }

        protected virtual void Setup()
        { 
        }

        public void New()
        {
            scriptDataService = new ConstellationEditorDataService();
            scriptDataService.New();
            Setup();
        }

        public void Recover()
        {
            scriptDataService = new ConstellationEditorDataService();
            var scriptData = scriptDataService.Recover(scriptDataService.OpenEditorData().LastOpenedConstellationPath[0]);
            if (scriptData != null)
                Setup();
        }

        public void Open(string _path = "")
        {
            scriptDataService = new ConstellationEditorDataService();
            scriptDataService.OpenConstellation(_path);
            Setup();
        }


        public void Save()
        {
            scriptDataService.Save();
        }

        protected bool IsConstellationSelected()
        {
            if (scriptDataService != null)
            {
                if (scriptDataService.GetCurrentScript() != null)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }
    }
}
