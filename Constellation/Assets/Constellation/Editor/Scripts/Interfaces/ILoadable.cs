namespace ConstellationEditor
{
    public interface ILoadable
    {
        void Open(ConstellationScriptInfos _path);
        void Save();
        void New();
        void Export();

    }
}