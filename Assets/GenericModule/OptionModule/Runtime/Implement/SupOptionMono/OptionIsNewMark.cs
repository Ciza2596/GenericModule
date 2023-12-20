namespace CizaOptionModule.Implement
{
    public class OptionIsNewMark : OptionSup
    {
        public override void Initialize(Option option)
        {
            base.Initialize(option);

            Option.OnIsNew += OnIsNew;
        }

        public override void Release(Option option)
        {
            base.Release(option);
            Option.OnIsNew -= OnIsNew;
        }

        private void OnIsNew(string optionKey, bool isNew) =>
            gameObject.SetActive(isNew);
    }
}