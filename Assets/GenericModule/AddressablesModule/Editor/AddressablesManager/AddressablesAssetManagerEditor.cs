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
        private readonly string[] _toolbarTexts = { "Export", "Import" };
        private int _toolbarIndex = 0;

        private AddressablesAssetManager _addressablesAssetManager = new AddressablesAssetManager();

        private string _fileName = "AddressablesAssetConfig.txt";
        private string _exportPath = Application.dataPath;

        private TextAsset _importText;
        private BundledAssetGroupSchema.BundlePackingMode _bundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;


        //private method
        [MenuItem("Tools/CizaModule/AddressablesModuleAsset")]
        private static void ShowWindow() => GetWindow<AddressablesAssetManagerEditor>("AddressablesModuleAssetEditor");

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
            _fileName = EditorGUILayout.TextField("Enter File Name", _fileName);
            _exportPath = GetFolderPathAndOpenWindow("Export Path", _exportPath);
            EditorGUILayout.Space();

            if (GUILayout.Button("Export"))
                Export();
        }

        private void ImportArea()
        {
            EditorGUILayout.Space();
            _importText =
                EditorGUILayout.ObjectField("Addressables Config", _importText, typeof(TextAsset)) as TextAsset;
            _bundleMode = (BundledAssetGroupSchema.BundlePackingMode)EditorGUILayout.EnumFlagsField("Bundle Mode", _bundleMode);
            EditorGUILayout.Space();

            if (GUILayout.Button("Import"))
                Import();
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
        }


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
                    path = EditorUtility.OpenFolderPanel("Folder Path", "", "");

                return string.IsNullOrWhiteSpace(path) ? originPath : path;
            }
        }
    }
}