namespace Constellation
{
    public interface IConstellationFileParser
    {
        ConstellationScriptData ParseConstellationScript(string JSON);
        string ParseConstellationScript(ConstellationScriptData data);
    }
}
