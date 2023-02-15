using System.Linq;

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
        public AssetInfoData(string groupName, string address, int instanceId, string[] labels,
                             string splitTag,  string assetPath) : this(
            groupName, address, instanceId, assetPath)
        {
            var labelsLength = labels.Length;
            for (int i = 0; i < labelsLength; i++)
            {
                LabelsString += labels[i];

                if (i < (labelsLength - 1))
                    LabelsString += splitTag;
            }
        }

        public AssetInfoData(string groupName, string address, int instanceId, string labelsString,
                             string splitTag) : this(
            groupName, address, instanceId)
        {
            var labelsWithNull= labelsString.Split(splitTag).ToList();
            labelsWithNull.Remove("");
            Labels = labelsWithNull.ToArray();
        }
        

        private AssetInfoData(string groupName, string address, int instanceId, string assetPath = null)
        {
            GroupName = groupName;
            Address = address;
            InstanceId = instanceId;
            AssetPath = assetPath;
        }
    }
}