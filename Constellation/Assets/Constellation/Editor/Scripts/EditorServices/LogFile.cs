using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class LogFile
{
    public static void WriteString(string fileName, string message)
    {

        var folderPath = ConstellationEditor.ConstellationEditor.GetEditorPath() + "EditorData/Logs";
        Directory.CreateDirectory(folderPath);
        string path = folderPath + "/" + fileName + DateTime.UtcNow.Ticks +".txt";
        
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(message);
        writer.Close();
        AssetDatabase.Refresh();
    }
}