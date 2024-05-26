namespace ReadWriteArc
{
    public interface ICharacter : ICharacterRead
    {
        void Write(ICharacterRead characterRead);
    }
}