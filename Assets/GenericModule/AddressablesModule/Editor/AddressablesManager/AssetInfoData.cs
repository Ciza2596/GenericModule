namespace AddressablesModule.Editor
{
    public class AssetInfoData
    {
        //public variable
        public string GroupName { get; }
        public string Address { get; }
        public string AssetPath { get; }
        public string LabelsString { get; }

        public string[] Labels { get; }


        //constructor
        public AssetInfoData(string groupName, string address, string assetPath, string[] labels, string splitTag) : this(
            groupName, address, assetPath)
        {
            var labelsLength = labels.Length;
            for (int i = 0; i < labelsLength; i++)
            {
                LabelsString += labels[i];

                if (i < (labelsLength - 1))
                    LabelsString += splitTag;
            }
        }

        public AssetInfoData(string groupName, string address, string assetPath, string labelsString,
            string splitTag) : this(
            groupName, address, assetPath) =>
            Labels = labelsString.Split(splitTag);


        private AssetInfoData(string groupName, string address, string assetPath)
        {
            GroupName = groupName;
            Address = address;
            AssetPath = assetPath;
        }
    }
}