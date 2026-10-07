using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CizaAddressablesModule.Editor
{
    public class AddressablesAssetManagerWindow : EditorWindow
    {
        private AddressablesAssetManager _addressablesAssetManager = new AddressablesAssetManager();

        private const string ADDRESSABLES_ASSET_MANAGER_EDITOR = "AddressablesAssetManagerEditor.";

        protected readonly string _tabIndexKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(TabIndex)}";

        protected virtual int TabIndex
        {
            get => EditorPrefs.GetInt(_tabIndexKey, 0);
            set => EditorPrefs.SetInt(_tabIndexKey, value);
        }
        

        private readonly string _configNameKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(ConfigName)}";

        protected virtual string ConfigName
        {
            get => EditorPrefs.GetString(_configNameKey, "AddressablesAssetConfig.txt");
            set => EditorPrefs.SetString(_configNameKey, value);
        }

        private readonly string _exportPathKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(ExportPath)}";

        protected virtual string ExportPath
        {
            get => EditorPrefs.GetString(_exportPathKey, Application.dataPath);

            set => EditorPrefs.SetString(_exportPathKey, value);
        }

        private readonly string _importTextGuidKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}{nameof(ImportTextGuid)}";

        protected virtual string ImportTextGuid
        {
            get => EditorPrefs.GetString(_importTextGuidKey, string.Empty);
            set => EditorPrefs.SetString(_importTextGuidKey, value);
        }

        private TextAsset _importText;

        protected virtual TextAsset ImportText
        {
            get
            {
                _importText ??= GetObject<TextAsset>(ImportTextGuid);
                return _importText;
            }
            set
            {
                _importText = value;
                var guid = _importText is null ? string.Empty : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_importText));
                ImportTextGuid = guid;
            }
        }
        
        private readonly string _addAddressableDataListKey = $"{ADDRESSABLES_ASSET_MANAGER_EDITOR}AddAddressableDataList";
        
        protected virtual string[] USSPaths => new[] { "AddressablesAssetManager" };
        
        protected virtual string[] RootClasses => new[] { "addressablesassetmanager" };

        //private method
        [MenuItem("Tools/Ciza/AddressablesAssetManager")]
        private static void ShowWindow() => GetWindow<AddressablesAssetManagerWindow>("AddressablesAssetManager");

        protected void CreateGUI()
        {
            var root = rootVisualElement;
            
            foreach (var sheet in StyleSheetUtils.GetStyleSheets(USSPaths))
                root.styleSheets.Add(sheet);
            foreach (var c in RootClasses)
                root.AddToClassList(c);
            
            var tabView = new TabView();

            var exportTab = CreateTab("Export");
            SetupExport(exportTab.contentContainer);
            tabView.Add(exportTab);
            
            var importTab = CreateTab("Import");
            SetupImport(importTab.contentContainer);
            tabView.Add(importTab);
            
            var addTab = CreateTab("Add");
            var scrollView = new ScrollView() { horizontalScrollerVisibility = ScrollerVisibility.Hidden };
            addTab.contentContainer.Add(scrollView);
            SetupAdd(scrollView.contentContainer);
            tabView.Add(addTab);

            tabView.selectedTabIndex = Math.Clamp(TabIndex, 0, 2);
            tabView.activeTabChanged += (_, _) => TabIndex = tabView.selectedTabIndex;
            
            root.Add(tabView);
        }
        
        protected virtual Tab CreateTab(string tabName)
        {
            var tab = new Tab(tabName);
            tab.contentContainer.AddToClassList(AlignLabel.UNITY_INSPECTOR_CLASS);
            tab.contentContainer.AddToClassList(AlignLabel.UNITY_INSPECTOR_ELEMENT_CLASS);
            tab.contentContainer.SetPadding(20, 20, 10, 10);
            return tab;
        }

        protected virtual TextField CreateTextField(string label, string value, Action<string> onValueChanged)
        {
            var textField = new TextField(label);
            textField.AddToClassList(AlignLabel.UNITY_ALIGN_FIELD_CLASS);
            textField.labelElement.style.paddingLeft = 0;
            textField.SetValueWithoutNotify(value);
            textField.RegisterValueChangedCallback(@event => onValueChanged?.Invoke(@event.newValue));
            return textField;
        }

        protected virtual Button CreateButton(string text, Action onClick)
        {
            var button = new Button(onClick) {text = text};
            button.AddToClassList(AlignLabel.UNITY_ALIGN_FIELD_CLASS);
            return button;
        }
        
        protected virtual void SetupExport(VisualElement container)
        {
            var configNameField = CreateTextField("Config Name", ConfigName, value => ConfigName = value);
            container.Add(configNameField);

            var exportPathField = CreateTextField("Export Path", ExportPath, value => ExportPath = value);
            var exportPathButton = CreateButton("Select", () =>
            {
                var selectedPath = EditorUtility.OpenFolderPanel("Folder Path", ExportPath, "");
                if (string.IsNullOrWhiteSpace(selectedPath))
                    return;
                ExportPath = selectedPath;
                exportPathField.SetValueWithoutNotify(ExportPath);
            });
            exportPathField.Add(exportPathButton);
            container.Add(exportPathField);
            
            container.Add(new SmallSpaceVE());
            var exportButton = CreateButton("Export", Export);
            container.Add(exportButton);
        }

        protected virtual void SetupImport(VisualElement container)
        {
            var configField = new ObjectField("Config") { objectType = typeof(TextAsset), allowSceneObjects = false };
            configField.AddToClassList(AlignLabel.UNITY_ALIGN_FIELD_CLASS);
            configField.labelElement.style.paddingLeft = 0;
            configField.SetValueWithoutNotify(ImportText);
            configField.RegisterValueChangedCallback(@event => ImportText = @event.newValue as TextAsset);
            container.Add(configField);
            
            container.Add(new SmallSpaceVE());
            var importButton = CreateButton("Import", Import);
            container.Add(importButton);
        }

        protected virtual void SetupAdd(VisualElement container)
        {
            var addAddressableListVE = new AddAddressableListVE(_addAddressableDataListKey, _addressablesAssetManager);
            addAddressableListVE.Initialize();
            addAddressableListVE.style.marginRight = 5;
            container.Add(addAddressableListVE);
        }
        
        protected virtual  void Export()
        {
            var content = _addressablesAssetManager.Export();
            CreateAndWriteFile(content);

            AssetDatabase.Refresh();
        }

        protected virtual  void Import()
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

        protected virtual void CreateAndWriteFile(string content = null)
        {
            var fullPath = GetFullPath();
            var fileStream = new FileStream(fullPath, FileMode.Create);

            if (content != null)
                WriteFile(fileStream, content);

            fileStream.Close();
        }

        protected virtual void WriteFile(FileStream fileStream, string content)
        {
            var charArray = content.ToCharArray();
            var byteArray = new byte[Encoding.UTF8.GetMaxByteCount(charArray.Length)];

            var encoder = Encoding.UTF8.GetEncoder();
            encoder.Convert(charArray, 0, charArray.Length, byteArray, 0, byteArray.Length, true, out var charsUsed, out var bytesUsed, out var completed);

            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Write(byteArray, 0, bytesUsed);
        }

        protected virtual string GetFullPath()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(ConfigName), "[AddressablesAssetManagerEditor::GetFullPath] FileName is null.");

            var exportPath = GetAssetPathWithDataPath(ExportPath);
            return Path.Combine(exportPath, ConfigName);
        }

        private T GetObject<T>(string guid) where T : Object
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            return obj;
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