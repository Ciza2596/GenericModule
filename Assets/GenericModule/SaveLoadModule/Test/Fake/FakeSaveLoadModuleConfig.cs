using System.Text;
using SaveLoadModule;
using UnityEngine;

public class FakeSaveLoadModuleConfig : ISaveLoadModuleConfig
{
    //public variable
    public string ApplicationDataPath => Application.persistentDataPath;

    public string DefaultFilePath => "DefaultFile.slmf";

    public int BufferSize => 2048;
    public Encoding Encoding => Encoding.UTF8;
}