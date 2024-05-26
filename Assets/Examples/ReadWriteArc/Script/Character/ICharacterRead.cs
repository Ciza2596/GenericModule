namespace ReadWriteArc
{
    public interface ICharacterRead
    {
        string Id { get; }

        bool IsGround { get; }
        bool IsWall { get; }

        bool IsHurt { get; }
        bool IsSmashHurt { get; }

        string CurrentAnim { get; }
        
        
    }
}