using UnityEngine;

namespace CizaFadeCreditModule
{
    public interface IFadeCreditRowData
    {
        float Time { get; }

        string PrefabAddress { get; }

        int ViewOrder { get; }

        Vector2 Position { get; }
        float Duration { get; }
        Vector2 Size { get; }

        RowKinds RowKind { get; }

        string Text { get; }
        string SpriteAddress { get; }
    }
}