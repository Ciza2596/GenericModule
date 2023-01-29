using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using Path = System.IO.Path;

public class StreamWriterSample : MonoBehaviour
{
    [SerializeField] private string _folderPath = "player/Assets/Examples/SaveLoadModule/Example1/SaveFile";
    [SerializeField] private string _fileName = "Player.txt";

    private StreamWriter _streamWriter;
    
    [Button]
    private void CreateFile()
    {
        var savePath = GetApplicationSavePath();
        _streamWriter = new StreamWriter(savePath,true);
        if (!File.Exists(savePath))
        {
            _streamWriter.WriteLine("Date\t\t| ROM \t\t| Left Hand | Right Hand |\n");
        }
        else
        {
            string date = "Login date: " + System.DateTime.Now + "\n";
            _streamWriter.WriteLine(date);
        }
    }

    private string GetApplicationSavePath() => GetSavePath(Application.persistentDataPath, _folderPath, _fileName);

    private string GetSavePath(string persistentDataPath, string fileName, string fileSuffix) =>
            Path.Combine(persistentDataPath, fileName, fileSuffix);
}