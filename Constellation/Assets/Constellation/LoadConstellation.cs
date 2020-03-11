using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Constellation;

public class LoadConstellation : MonoBehaviour
{
    public string name = "Constellation";
    public ConstellationScriptData constellationScript;
    public string dataAsJson;
    public ConstellationBehaviour ConstellationBehaviour;
    // Start is called before the first frame update
    void Start()
    {
        var folderPath = Application.streamingAssetsPath;
        dataAsJson = File.ReadAllText(folderPath + "/" + name +".const");
        constellationScript = JsonUtility.FromJson<ConstellationScriptData>(dataAsJson);
        ConstellationBehaviour.SetConstellation(constellationScript);
    }
}
