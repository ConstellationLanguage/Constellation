namespace Constellation
{
    public interface ICustomNode
    {
        void UpdateNode(ConstellationScriptData constellation, IConstellationFileParser constellationFileParser);
        void SetupNodeIO(IConstellationFileParser constellationFileParser);
        string GetDisplayName();
        void InitializeConstellation(ConstellationScriptData[] constellationScripts, IConstellationFileParser constellationFileParser, bool isPrivateScope);
    }
}
