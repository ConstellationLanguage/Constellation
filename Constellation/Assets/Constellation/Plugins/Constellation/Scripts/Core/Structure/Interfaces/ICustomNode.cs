namespace Constellation
{
    public interface ICustomNode
    {
        void UpdateNode(ConstellationScriptData constellation);
        void SetupNodeIO();
        string GetDisplayName();
        void InitializeConstellation(ConstellationScriptData[] constellationScripts);
    }
}
