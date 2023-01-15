using System.IO;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using Path = System.IO.Path;

public class FileStreamSample : MonoBehaviour
{
    [SerializeField] private string _fileName = "Player.txt";
    [SerializeField] private string _content = "Hello!";

    private FileStream _fileStream;
    
    [Button]
    private void CreateFile()
    {
        var path = GetApplicationPath();

        if (File.Exists(path))
            File.Delete(path);

        _fileStream = File.Create(path);
        AddText(_fileStream, "Create File Success!");
    }

    [Button]
    private void AddContent() =>
        AddText(_fileStream, _content);
    


    [Button]
    private void LogFilePath()
    {
        var path = GetApplicationPath();
        Debug.Log(path);
    }

    [Button]
    private void DeleteFile()
    {
        var path = GetApplicationPath();

        if (File.Exists(path))
            File.Delete(path);
    }

    private string GetApplicationPath() => GetSavePath(Application.persistentDataPath, string.Empty, _fileName);

    private string GetSavePath(string persistentDataPath, string fileDictionary, string fileName) =>
            Path.Combine(persistentDataPath, fileDictionary, fileName);

    private void AddText(FileStream fileStream, string message)
    {
        var utf8Encoding = new UTF8Encoding(true);
        var bytes = utf8Encoding.GetBytes(message);
        fileStream.Write(bytes,0,bytes.Length);
    }
}