using System;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAddressablesModule.Editor
{
    [Serializable]
    public class AddAddressableData : IZomeraphyPanel
    {
        // VARIABLE: -----------------------------------------------------------------------------

        [SerializeField]
        protected string _dataId;
        
        [SerializeField]
        protected FileFilter[] _fileFilters = Array.Empty<FileFilter>();
        
        [SerializeField]
        protected string[] _paths = Array.Empty<string>();
        
        [SerializeField]
        protected string _groupName;

        [SerializeField]
        protected BundledAssetGroupSchema.BundlePackingMode _bundleMode;

        [SerializeField]
        protected string _labels;
        
        [SerializeField]
        protected string _addressTrimPrefix;
        [SerializeField]
        protected string _addressTrimSuffix;
        
        [SerializeField]
        protected string _addressAddSuffix;
        [SerializeField]
        protected string _addressAddPrefix;

        // PUBLIC VARIABLE: ---------------------------------------------------------------------
        
        public string DataId => _dataId;

        public FileFilter[] FileFilters => _fileFilters ?? Array.Empty<FileFilter>();
        public string[] Paths => _paths ?? Array.Empty<string>();
        public string GroupName => _groupName;
        public BundledAssetGroupSchema.BundlePackingMode BundleMode => _bundleMode;
        public string Labels => _labels;
        public string AddressTrimPrefix => _addressTrimPrefix;
        public string AddressTrimSuffix => _addressTrimSuffix;
        public string AddressAddSuffix => _addressAddSuffix;
        public string AddressAddPrefix => _addressAddPrefix;

        // CONSTRUCTOR: ------------------------------------------------------------------------

        [Preserve]
        public AddAddressableData () : this(string.Empty){}

        [Preserve]
        public AddAddressableData(string dataId) : this(dataId, Array.Empty<FileFilter>(), Array.Empty<string>(), string.Empty, default, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty){}
        
        [Preserve]
        public AddAddressableData(string dataId, FileFilter[] fileFilters, string[] paths, string groupName, BundledAssetGroupSchema.BundlePackingMode bundleMode, string labels, string addressTrimPrefix, string addressTrimSuffix, string addressAddPrefix, string addressAddSuffix)
        {
            _dataId = dataId;
            _fileFilters = fileFilters == null ? Array.Empty<FileFilter>() : (FileFilter[])fileFilters.Clone();
            _paths = paths == null ? Array.Empty<string>() : (string[])paths.Clone();
            _groupName = groupName ?? string.Empty;
            _bundleMode = bundleMode;
            _labels = labels ?? string.Empty;
            _addressTrimPrefix = addressTrimPrefix ?? string.Empty;
            _addressTrimSuffix = addressTrimSuffix ?? string.Empty;
            _addressAddPrefix = addressAddPrefix ?? string.Empty;
            _addressAddSuffix = addressAddSuffix ?? string.Empty;
        }
    }
}
