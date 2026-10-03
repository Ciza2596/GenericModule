using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAddressablesModule.Editor
{
    public class AddAddressableListVE : BSimpleListVE<AddAddressableData, AddAddressableVE>
    {
        // VARIABLE: -----------------------------------------------------------------------------

        protected readonly AddressablesAssetManager _addressablesAssetManager;
        
        protected override AddAddressableData CreateDefaultItem() => 
             new AddAddressableData();
        protected override AddAddressableVE CreateItemVE() => 
            new AddAddressableVE(this);
        protected override int GetItemIndex(AddAddressableVE item) =>
            item.Index;
        protected override void RefreshItem(AddAddressableVE item, int index, AddAddressableData value) =>
            item.Refresh(index, value);
        protected override void RefreshItemSelectedStyle(AddAddressableVE item) =>
            item.RefreshSelectedStyle();
        protected override string GetItemSearchText(AddAddressableData item) =>
            item == null ? string.Empty : $"{item.GroupName} {item.Labels}";
        protected override AddAddressableData NormalizeItem(AddAddressableData item) =>
            item ?? new AddAddressableData();
        protected override AddAddressableData Clone(AddAddressableData item) =>
            item == null ? new AddAddressableData() : JsonUtility.FromJson<AddAddressableData>(JsonUtility.ToJson(item));
        
        protected virtual string[] GetFilteredAssetPaths(FileFilter[] fileFilters, string[] paths)
        {
            paths = paths.Where(AssetDatabase.IsValidFolder).ToArray();

            var results = new List<string>();
            if (fileFilters is not { Length: > 0 })
                results.AddRange(AssetDatabase.FindAssets(string.Empty, paths).Select(AssetDatabase.GUIDToAssetPath).Where(Path.HasExtension));
            else
            {
                foreach (var filter in fileFilters)
                {
                    var filterString = $"{filter.NamePrefix} {filter.NameSuffix} ";
                    var assetPaths = AssetDatabase.FindAssets(filterString.Trim(), paths).Select(AssetDatabase.GUIDToAssetPath);

                    var filteredPaths = assetPaths.Where(path => CheckIsFilePathMatched(path, filter));
                    results.AddRange(filteredPaths);
                }
            }

            return results.Distinct().ToArray();

            bool CheckIsFilePathMatched(string path, FileFilter filter)
            {
                if (!path.CheckHasValue() || !path.Contains('/') || !path.Contains('.'))
                    return false;

                var fileName = Path.GetFileNameWithoutExtension(path);
                var extension = Path.GetExtension(path);
                var isPrefixMatched = !filter.NamePrefix.CheckHasValue() || fileName.StartsWith(filter.NamePrefix);
                var isSuffixMatched = !filter.NameSuffix.CheckHasValue() || fileName.EndsWith(filter.NameSuffix);
                var isExtensionMatched = filter.Extension == "." || extension.Equals(filter.Extension);
                return isPrefixMatched && isSuffixMatched && isExtensionMatched;
            }
        }
        
        // CONSTRUCTOR: ------------------------------------------------------------------------

        [Preserve]
        public AddAddressableListVE(string addAddressableDatasKey, AddressablesAssetManager addressablesAssetManager) : base(addAddressableDatasKey ?? "CizaAddressablesModule.AddressablesAssetManager.AddAddressableDatas") =>
            _addressablesAssetManager = addressablesAssetManager;

        public virtual void AddAddressable(AddAddressableData addAddressableData)
        {
            var assetPaths = GetFilteredAssetPaths(addAddressableData.FileFilters, addAddressableData.Paths);
            var groupName = addAddressableData.GroupName;
            var bundleModeIndex = (int)addAddressableData.BundleMode;
            var labels = addAddressableData.Labels;
            var addressTrimPrefix = addAddressableData.AddressTrimPrefix;
            var addressTrimSuffix = addAddressableData.AddressTrimSuffix;
            var addressAddPrefix = addAddressableData.AddressAddPrefix;
            var addressAddSuffix = addAddressableData.AddressAddSuffix;
            
            foreach (var assetPath in assetPaths) 
                _addressablesAssetManager.Add(groupName, bundleModeIndex, assetPath, labels, addressTrimPrefix, addressTrimSuffix, addressAddPrefix, addressAddSuffix);
        }
        
        
    }
}
