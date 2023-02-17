using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Assertions;

namespace AddressablesModule.Editor
{
    public class AddressablesAssetManagerEditor : EditorWindow
    {
        //private variable
        private readonly string[] _toolbarTexts = { "Export", "Import", "Add" };
        private int _toolbarIndex = 0;

        private AddressablesAssetManager _addressablesAssetManager = new AddressablesAssetManager();

        private string _fileName = "AddressablesAssetConfig.txt";
        private string _exportPath = Application.dataPath;

        private TextAsset _importText;

        private BundledAssetGroupSchema.BundlePackingMode _bundleMode =
            BundledAssetGroupSchema.BundlePackingMode.PackSeparately;

        private string _assetFolderPath;
        private string _groupName;
        private string _labelsString;
        private string _addressPrefix;
        private string _addressSuffix;


        //private method
        [MenuItem("Tools/CizaModule/AddressablesAssetManager")]
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
            EditorGUILayout.Space();
            _fileName = EditorGUILayout.TextField("Config Name", _fileName);
            _exportPath = GetFolderPathAndOpenWindow("Export Path", _exportPath);
            EditorGUILayout.Space();

            if (GUILayout.Button("Export"))
                Export();
        }

        private void ImportArea()
        {
            EditorGUILayout.Space();
            _importText =
                EditorGUILayout.ObjectField("Config", _importText, typeof(TextAsset)) as TextAsset;
            _bundleMode =
                (BundledAssetGroupSchema.BundlePackingMode)EditorGUILayout.EnumFlagsField("Bundle Mode", _bundleMode);
            EditorGUILayout.Space();

            if (GUILayout.Button("Import"))
                Import();
        }

        private void AddArea()
        {
            EditorGUILayout.Space();
            _assetFolderPath = GetFolderAssetPathAndOpenWindow("Asset Folder Path", _assetFolderPath);
            EditorGUILayout.Space();
            
            _groupName = EditorGUILayout.TextField("Group Name", _groupName);
            _bundleMode =
                (BundledAssetGroupSchema.BundlePackingMode)EditorGUILayout.EnumFlagsField("Bundle Mode", _bundleMode);
            _labelsString = EditorGUILayout.TextField("Labels", _labelsString);
            EditorGUILayout.Space();
            
            _addressPrefix = EditorGUILayout.TextField("Address Prefix", _addressPrefix);
            _addressSuffix = EditorGUILayout.TextField("Address Suffix", _addressSuffix);
            EditorGUILayout.Space();

            if (GUILayout.Button("Add"))
                Add();
        }


        private void Export()
        {
            var content = _addressablesAssetManager.Export();
            CreateAndWriteFile(content);

            AssetDatabase.Refresh();
        }

        private void Import()
        {
            if (_importText is null)
            {
                Debug.LogError("[AddressablesAssetManagerEditor::Import] ImportText is null.");
                return;
            }

            var content = _importText.text;
            _addressablesAssetManager.Import(content, _bundleMode);
            Export();
        }

        private void Add() =>
            _addressablesAssetManager.Add(_groupName, _bundleMode, _assetFolderPath, _labelsString, _addressPrefix,
                _addressSuffix);


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
            var byteArray = new byte[charArray.Length];

            Encoder ec = Encoding.UTF8.GetEncoder();
            ec.GetBytes(charArray, 0, charArray.Length, byteArray, 0, true);

            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Write(byteArray, 0, byteArray.Length);
        }


        private string GetFullPath()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(_fileName),
                "[AddressablesAssetManagerEditor::GetFullPath] FileName is null.");
            return Path.Combine(_exportPath, _fileName);
        }

        private string GetFolderPathAndOpenWindow(string label, string originPath)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var path = EditorGUILayout.TextField(label, originPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                    path = EditorUtility.OpenFolderPanel("Folder Path", "Assets", "");

                return string.IsNullOrWhiteSpace(path) ? originPath : path;
            }
        }

        private string GetFolderAssetPathAndOpenWindow(string label, string originPath)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var path = EditorGUILayout.TextField(label, originPath);
                if (GUILayout.Button("Select", EditorStyles.miniButton, GUILayout.Width(65)))
                {
                    path = EditorUtility.OpenFolderPanel("Folder Path", "Assets", "");
                    var dataPath = Application.dataPath;
                    dataPath = dataPath.Replace("Assets", "");
                    path = path.Replace(dataPath, "");
                }


                return string.IsNullOrWhiteSpace(path) ? originPath : path;
            }
        }
    }
}