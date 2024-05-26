namespace ReadWriteArc
{
    public class Character : ICharacter
    {
        public string Id { get; }

        public bool IsGround { get; protected set; }
        public bool IsWall { get; protected set; }
        public bool IsHurt { get; protected set; }
        public bool IsSmashHurt { get; protected set; }
        public string CurrentAnim { get; protected set; }

        public Character(string id) =>
            Id = id;

        public virtual void Write(ICharacterRead characterRead)
        {
            IsGround = characterRead.IsGround;
            IsWall = characterRead.IsWall;
            IsHurt = characterRead.IsHurt;
            IsSmashHurt = characterRead.IsSmashHurt;
            CurrentAnim = characterRead.CurrentAnim;
        }
    }
}