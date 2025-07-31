using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaAddressablesModule.Editor
{
    public class AddressablesAssetManagerEditor : EditorWindow
    {
        //private variable
        private readonly string[] _toolbarTexts = { "Export", "Import", "Add" };
        private int _toolbarIndex = 0;

        private AddressablesAssetManager _addressablesAssetManager = new AddressablesAssetManager();

        private const string ADDRESSABLES_ASSET_MANAGER_EDITOR = "AddressablesAssetManagerEditor.";

        private readonly string _configNameKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(ConfigName)}";

        private string ConfigName
        {
            get
            {
                var value = PlayerPrefs.GetString(_configNameKey);
                return string.IsNullOrWhiteSpace(value) ? "AddressablesAssetConfig.txt" : value;
            }

            set
            {
                PlayerPrefs.SetString(_configNameKey, value);
                PlayerPrefs.Save();
            }
        }

        private readonly string _exportPathKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(ExportPath)}";

        private string ExportPath
        {
            get
            {
                var value = PlayerPrefs.GetString(_exportPathKey);
                return string.IsNullOrWhiteSpace(value) ? Application.dataPath : value;
            }

            set
            {
                PlayerPrefs.SetString(_exportPathKey, value);
                PlayerPrefs.Save();
            }
        }

        private readonly string _importTextGuidKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(ImportTextGuid)}";

        private string ImportTextGuid
        {
            get => PlayerPrefs.GetString(_importTextGuidKey);
            set
            {
                PlayerPrefs.SetString(_importTextGuidKey, value);
                PlayerPrefs.Save();
            }
        }

        private TextAsset _importText;

        private TextAsset ImportText
        {
            get
            {
                if (_importText is null)
                    _importText = GetObject<TextAsset>(ImportTextGuid);

                return _importText;
            }
            set
            {
                _importText = value;
                var guid = _importText is null ? string.Empty : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_importText));
                ImportTextGuid = guid;
            }
        }

        private readonly string _assetFolderPathKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(AssetPath)}";

        private string AssetPath
        {
            get => PlayerPrefs.GetString(_assetFolderPathKey);

            set
            {
                PlayerPrefs.SetString(_assetFolderPathKey, value);
                PlayerPrefs.Save();
            }
        }

        private readonly string _groupNameKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(GroupName)}";

        private string GroupName
        {
            get => PlayerPrefs.GetString(_groupNameKey);

            set
            {
                PlayerPrefs.SetString(_groupNameKey, value);
                PlayerPrefs.Save();
            }
        }

        private string _bundleModeKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(BundleMode)}";

        private BundledAssetGroupSchema.BundlePackingMode BundleMode
        {
            get => (BundledAssetGroupSchema.BundlePackingMode)PlayerPrefs.GetInt(_bundleModeKey);

            set
            {
                PlayerPrefs.SetInt(_bundleModeKey, (int)value);
                PlayerPrefs.Save();
            }
        }

        private readonly string _labelsStringKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(LabelsString)}";

        private string LabelsString
        {
            get => PlayerPrefs.GetString(_labelsStringKey);

            set
            {
                PlayerPrefs.SetString(_labelsStringKey, value);
                PlayerPrefs.Save();
            }
        }

        private readonly string _addressPrefixKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(AddressPrefix)}";

        private string AddressPrefix
        {
            get => PlayerPrefs.GetString(_addressPrefixKey);

            set
            {
                PlayerPrefs.SetString(_addressPrefixKey, value);
                PlayerPrefs.Save();
            }
        }

        private readonly string _addressSuffixKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(AddressSuffix)}";

        private string AddressSuffix
        {
            get => PlayerPrefs.GetString(_addressSuffixKey);

            set
            {
                PlayerPrefs.SetString(_addressSuffixKey, value);
                PlayerPrefs.Save();
            }
        }

        //private method
        [MenuItem("Tools/Ciza/AddressablesAssetManager")]
        private static void ShowWindow() => GetWindow<AddressablesAssetManagerEditor>("AddressablesAssetManager");

        private void OnGUI()
        {
            ToolbarArea();

            switch (_toolbarIndex)
            {
                case 0:
                    ExportArea();
                    break;
                case 1:
                    ImportArea();
                    break;
                case 2:
                    AddArea();
                    break;
            }
        }

        private void ToolbarArea()
        {
            GUILayout.BeginHorizontal();
            _toolbarIndex = GUILayout.Toolbar(_toolbarIndex, _toolbarTexts);
            GUILayout.EndHorizontal();
        }

        private void ExportArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(0.5f);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            ConfigName = EditorGUILayout.TextField("Config Name", ConfigName);
            ExportPath = GetFolderPathAndOpenWindow("Export Path", ExportPath);
            EditorGUILayout.Space();

            if (GUILayout.Button("Export"))
                Export();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(0.5f);
            EditorGUILayout.EndHorizontal();
        }

        private void ImportArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(0.5f);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            ImportText = EditorGUILayout.ObjectField("Config", ImportText, typeof(TextAsset), false) as TextAsset;
            EditorGUILayout.Space();

            if (GUILayout.Button("Import"))
                Import();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(0.5f);
            EditorGUILayout.EndHorizontal();
        }

        private void AddArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(0.5f);

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();
            AssetPath = GetAssetFolderPathAndOpenWindow("Asset Folder Path", AssetPath);
            EditorGUILayout.Space();

            GroupName = EditorGUILayout.TextField("Group Name", GroupName);
            BundleMode = (BundledAssetGroupSchema.BundlePackingMode)EditorGUILayout.EnumPopup("Bundle Mode", BundleMode);
            LabelsString = EditorGUILayout.TextField("Labels", LabelsString);
            EditorGUILayout.Space();

            AddressPrefix = EditorGUILayout.TextField("Address Prefix", AddressPrefix);
            AddressSuffix = EditorGUILayout.TextField("Address Suffix", AddressSuffix);
            EditorGUILayout.Space();

            if (GUILayout.Button("Add"))
                Add();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(0.5f);
            EditorGUILayout.EndHorizontal();
        }

        private void Export()
        {
            var content = _addressablesAssetManager.Export();
            CreateAndWriteFile(content);

            AssetDatabase.Refresh();
        }

        private void Import()
        {
            if (ImportText is null)
            {
                Debug.LogError("[AddressablesAssetManagerEditor::Import] ImportText is null.");
                return;
            }

            var content = ImportText.text;
            _addressablesAssetManager.Import(content);
            AssetDatabase.Refresh();
        }

        private void Add() =>
            _addressablesAssetManager.Add(GroupName, (int)BundleMode, AssetPath, LabelsString, AddressPrefix, AddressSuffix);

        private void CreateAndWriteFile(string content = null)
        {
            var fullPath = GetFullPath();
            var fileStream = new FileStream(fullPath, FileMode.Create);

            if (content != null)
                WriteFile(fileStream, content);

            fileStream.Close();
        }

        private void WriteFile(FileStream fileStream, string content)
        {
            var charArray = content.ToCharArray();
            var byteArray = new byte[Encoding.UTF8.GetMaxByteCount(charArray.Length)];

            var encoder = Encoding.UTF8.GetEncoder();
            encoder.Convert(charArray, 0, charArray.Length, byteArray, 0, byteArray.Length, true, out var charsUsed, out var bytesUsed, out var completed);

            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Write(byteArray, 0, bytesUsed);
        }

        private string GetFullPath()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(ConfigName), "[AddressablesAssetManagerEditor::GetFullPath] FileName is null.");

            var exportPath = GetAssetPathWithDataPath(ExportPath);
            return Path.Combine(exportPath, ConfigName);
        }

        private string GetFolderPathAndOpenWindow(string label, string originPath)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var path = EditorGUILayout.TextField(label, originPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                {
                    path = EditorUtility.OpenFolderPanel("Folder Path", originPath, "");
                    path = GetAssetPathWithoutDataPath(path);
                }

                return string.IsNullOrWhiteSpace(path) ? originPath : path;
            }
        }

        private string GetAssetFolderPathAndOpenWindow(string label, string originPath)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var path = EditorGUILayout.TextField(label, originPath);
                if (GUILayout.Button("Folder", EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    path = EditorUtility.OpenFolderPanel("Folder Path", originPath, "");
                    path = GetAssetPathWithoutDataPath(path);
                }
                
                if (GUILayout.Button("File", EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    path = EditorUtility.OpenFilePanel("File Path", originPath, "");
                    path = GetAssetPathWithoutDataPath(path);
                }

                return string.IsNullOrWhiteSpace(path) ? originPath : path;
            }
        }

        private T GetObject<T>(string guid) where T : Object
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            return obj;
        }

        private string GetAssetPathWithoutDataPath(string path)
        {
            var dataPath = Application.dataPath;
            dataPath = dataPath.Replace("Assets", "");

            if (path.Contains(dataPath))
                path = path.Replace(dataPath, "");

            return path;
        }

        private string GetAssetPathWithDataPath(string path)
        {
            var dataPath = Application.dataPath;
            dataPath = dataPath.Replace("Assets", "");

            if (!path.Contains(dataPath))
                path = dataPath + path;

            return path;
        }
    }
}