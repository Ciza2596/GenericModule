

namespace AddressablesModule.Editor
{
    public class AssetInfoData
    {
        //public variable
        public string GroupName { get; }
        public string Address { get; }
        public int InstanceId { get; }
        public string LabelsString { get; }
        public string[] Labels { get; }


        public string AssetPath { get; }


        //constructor
        public AssetInfoData(string groupName, string address, int instanceId, string labelsString, string assetPath) : this(
            groupName, address, instanceId, assetPath) =>
            LabelsString = labelsString;

        public AssetInfoData(string groupName, string address, int instanceId, string[] labels) : this(
            groupName, address, instanceId) =>
            Labels = labels;


        private AssetInfoData(string groupName, string address, int instanceId, string assetPath = null)
        {
            GroupName = groupName;
            Address = address;
            InstanceId = instanceId;
            AssetPath = assetPath;
        }
    }
}