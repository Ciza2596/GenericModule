namespace CizaFadeCreditModule
{
    public static class RowKindsExtension
    {
        public static bool CheckIsEmpty(this RowKinds rowKind) =>
            rowKind == RowKinds.Empty;

        public static bool CheckIsText(this RowKinds rowKind) =>
            rowKind == RowKinds.Text;

        public static bool CheckIsSprite(this RowKinds rowKind) =>
            rowKind == RowKinds.Sprite;
    }
}