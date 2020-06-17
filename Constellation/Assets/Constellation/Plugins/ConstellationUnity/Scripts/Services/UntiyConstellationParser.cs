using UnityEngine;
using Constellation;

public class UntiyConstellationParser : IConstellationFileParser
{
    public ConstellationScriptData ParseConstellationScript(string JSON)
    {
        return JsonUtility.FromJson<ConstellationScriptData>(JSON);
    }

    public string ParseConstellationScript(ConstellationScriptData Data)
    {
        return JsonUtility.ToJson(Data);
    }
}
