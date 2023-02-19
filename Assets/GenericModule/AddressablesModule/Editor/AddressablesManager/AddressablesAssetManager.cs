using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AddressablesModule.Editor
{
    public class AddressablesAssetManager
    {
        //private method
        private const string END_TAG = "\n";
        private const string SPACE_TAG = " ";

        private const string GROUP_TAG = "@Group: ";
        private const string GROUP_END_TAG = END_TAG;

        private const string ASSET_INFO_TAG = "#";
        private const string ASSET_INFO_AND_TAG = ";";

        private const string ADDRESS_TAG = "Address: ";
        private const string INSTANCE_ID_TAG = "InstanceId: ";
        private const string LABELS_TAG = "Labels: ";

        private const string ASSET_PATH_TAG = "(AssetPath: ";
        private const string ASSET_PATH_END_TAG = ")";

        private const string LABELS_AND_TAG = ",";


        //public method
        public string Export()
        {
            string content = string.Empty;

            if (TryGetAllGroupNames(out var groupNames))
            {
                foreach (var groupName in groupNames)
                {
                    content += GetGroupNameWithTag(groupName);

                    if (TryGetGroupAssets(groupName, out var groupDatas))
                        foreach (var groupData in groupDatas)
                            content += GetAssetInfoWithTag(groupData);

                    content += GROUP_END_TAG;
                }
            }

            return content;
        }

        public void Import(string content,
            BundledAssetGroupSchema.BundlePackingMode bundlePackingMode)
        {
            ClearAddressables();
            ClearLabels();

            var assetInfoDatas = CreateAssetInfoDatasByContent(content, out var groupNames);

            CreateAddressablesGroups(groupNames, bundlePackingMode);


            foreach (var assetInfoData in assetInfoDatas)
            {
                var groupName = assetInfoData.GroupName;
                var address = assetInfoData.Address;
                var instanceId = assetInfoData.InstanceId;
                var labels = assetInfoData.Labels;

                AddEntryToAddressables(groupName, address, instanceId, labels);
            }
        }

        public void Add(string groupName, BundledAssetGroupSchema.BundlePackingMode bundlePackingMode,
            string assetFolderPath, string labelsString, string addressPrefix, string addressSuffix)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (string.IsNullOrWhiteSpace(groupName))
                groupName = settings.DefaultGroup.name;

            else
                CreateAddressablesGroup(groupName, bundlePackingMode);

            var labels = string.IsNullOrWhiteSpace(labelsString) ? null : GetLabelsFromLabelsString(labelsString);

            var guids = AssetDatabase.FindAssets("", new string[] { assetFolderPath });
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                if (obj is DefaultAsset)
                    continue;

                var address = addressPrefix + obj.name + addressSuffix;
                var instanceId = obj.GetInstanceID();
                AddEntryToAddressables(groupName, address, instanceId, labels);
            }
        }


        //private method
        private bool TryGetAllGroupNames(out string[] groupNames)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            groupNames = null;

            if (!settings)
            {
                Debug.LogError("[AddressablesAssetManager::GetAllGroupName] Settings is null.");
                return false;
            }

            var groups = settings.groups;
            var groupsCount = groups.Count - 2;

            if (groupsCount <= 0)
            {
                Debug.Log("[AddressablesAssetManager::GetAllGroupName] Group is only Built In Data.");
                return false;
            }

            groupNames = new string[groupsCount];

            for (int i = 0; i < groupsCount; i++)
            {
                var groupName = groups[i + 2].Name;
                groupNames[i] = groupName;
            }

            return true;
        }

        private bool TryGetGroupAssets(string groupName, out AssetInfoData[] groupDatas)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            groupDatas = null;

            if (!settings)
            {
                Debug.LogError("[AddressablesAssetManager::TryGetGroupAssets] Settings is null.");
                return false;
            }

            var groups = settings.groups;
            var group = groups.Find(group => group.Name == groupName);
            if (group is null)
            {
                Debug.LogError($"[AddressablesAssetManager::TryGetGroupAssets] Not find group: {groupName}.");
                return false;
            }

            var entries = group.entries.ToArray();
            var entriesLength = entries.Length;
            groupDatas = new AssetInfoData[entriesLength];

            for (int i = 0; i < entriesLength; i++)
            {
                var entry = entries[i];
                var address = entry.address;
                var assetPath = entry.AssetPath;
                var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                var instanceId = obj.GetInstanceID();
                var labels = entry.labels.ToArray();
                var labelsString = GetLabelsStringFromLabels(labels);
                groupDatas[i] = new AssetInfoData(groupName, address, instanceId, labelsString, assetPath);
            }

            return true;
        }

        private void ClearAddressables()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (!settings)
            {
                Debug.LogError("[AddressablesAssetManager::ClearAddressables] Settings is null.");
                return;
            }

            var groups = settings.groups.ToArray();
            for (int i = 2; i < groups.Length; i++)
            {
                var group = groups[i];
                settings.RemoveGroup(group);
            }
        }

        private void ClearLabels()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (!settings)
            {
                Debug.LogError("[AddressablesAssetManager::ClearLabels] Settings is null.");
                return;
            }

            var labels = settings.GetLabels().ToArray();
            if (labels.Length <= 0)
                return;

            foreach (var label in labels)
                settings.RemoveLabel(label);
        }

        private void CreateAddressablesGroups(string[] groupNames,
            BundledAssetGroupSchema.BundlePackingMode bundlePackingMode)
        {
            for (int i = 0; i < groupNames.Length; i++)
            {
                var groupName = groupNames[i];
                CreateAddressablesGroup(groupName, bundlePackingMode);
            }
        }

        private void CreateAddressablesGroup(string groupName,
            BundledAssetGroupSchema.BundlePackingMode bundlePackingMode)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (!settings)
            {
                Debug.LogError("[AddressablesAssetManager::CreateAddressablesGroup] Settings is null.");
                return;
            }

            var group = settings.FindGroup(groupName);
            if (!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema),
                    typeof(BundledAssetGroupSchema));

            var bundledAssetGroupSchema = group.GetSchema<BundledAssetGroupSchema>();
            bundledAssetGroupSchema.BundleMode = bundlePackingMode;
        }

        private void AddEntryToAddressables(string groupName, string address, int instanceId, string[] labels = null)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (!settings)
            {
                Debug.LogError("[AddressablesAssetManager::AddToAddressables] Settings is null.");
                return;
            }

            var group = settings.FindGroup(groupName);
            if (!group)
            {
                Debug.LogError($"[AddressablesAssetManager::AddToAddressables] Not Find Group: {groupName}.");
                return;
            }

            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(instanceId, out string guid, out long localId);

            var entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.SetAddress(address);

            entry.labels.Clear();

            if (labels != null)
                foreach (var label in labels)
                    entry.SetLabel(label, true, true);

            var entriesAdded = new List<AddressableAssetEntry> { entry };

            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
        }

        private AssetInfoData[] CreateAssetInfoDatasByContent(string content, out string[] groupNames)
        {
            content = content.Replace(END_TAG, null);
            var groupContents = content.Split(GROUP_TAG);

            var originGroupNames = new List<string>();

            var assetInfoDatas = Array.Empty<AssetInfoData>();
            foreach (var groupContent in groupContents)
            {
                if (string.IsNullOrWhiteSpace(groupContent))
                    continue;

                var newAssetInfoDatas = CreateAssetInfoDatasByGroupContent(groupContent, out var groupName);
                originGroupNames.Add(groupName);

                assetInfoDatas = assetInfoDatas.Concat(newAssetInfoDatas).ToArray();
            }

            groupNames = originGroupNames.ToArray();
            return assetInfoDatas;
        }

        private AssetInfoData[] CreateAssetInfoDatasByGroupContent(string groupContent, out string groupName)
        {
            var assetInfoContents = groupContent.Split(ASSET_INFO_TAG);
            groupName = assetInfoContents[0];

            var length = assetInfoContents.Length - 1;
            var assetInfoDatas = new AssetInfoData[length];
            for (int i = 0; i < length; i++)
            {
                var assetInfoContent = assetInfoContents[i + 1];
                assetInfoDatas[i] = CreateAssetInfoDataByAssetInfoContent(groupName, assetInfoContent);
            }

            return assetInfoDatas;
        }

        private AssetInfoData CreateAssetInfoDataByAssetInfoContent(string groupName, string assetInfoContent)
        {
            var infoContents = assetInfoContent.Split(ASSET_INFO_AND_TAG);

            var address = infoContents[0].Replace(ADDRESS_TAG, null);
            var instanceIdString = infoContents[1].Replace(INSTANCE_ID_TAG, null);
            var instanceId = int.Parse(instanceIdString);
            var labelsString = infoContents[2].Replace(LABELS_TAG, null);
            var labels = GetLabelsFromLabelsString(labelsString);
            return new AssetInfoData(groupName, address, instanceId, labels);
        }

        private string GetGroupNameWithTag(string groupName) => GROUP_TAG + groupName + END_TAG;

        private string GetAssetInfoWithTag(AssetInfoData assetInfoData)
        {
            var content = ASSET_INFO_TAG + END_TAG;
            content += ADDRESS_TAG + assetInfoData.Address + ASSET_INFO_AND_TAG + END_TAG;
            content += INSTANCE_ID_TAG + assetInfoData.InstanceId + ASSET_INFO_AND_TAG + END_TAG;
            content += LABELS_TAG + assetInfoData.LabelsString + ASSET_INFO_AND_TAG + END_TAG;

            content += ASSET_PATH_TAG + assetInfoData.AssetPath + ASSET_PATH_END_TAG + END_TAG;
            content += END_TAG;
            return content;
        }

        private string GetLabelsStringFromLabels(string[] labels)
        {
            var labelsString = string.Empty;
            var labelsLength = labels.Length;
            for (int i = 0; i < labelsLength; i++)
            {
                labelsString += labels[i];

                if (i < (labelsLength - 1))
                    labelsString += LABELS_AND_TAG + SPACE_TAG;
            }

            return labelsString;
        }

        private string[] GetLabelsFromLabelsString(string labelsString)
        {
            labelsString = labelsString.Replace(" ", null);
            var labels = labelsString.Split(LABELS_AND_TAG).ToList();
            return labels.ToArray();
        }
    }
}