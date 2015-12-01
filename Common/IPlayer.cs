namespace Common
{
    public interface IPlayer
    {
        string Name { get; }
        Neuromon Neuromon { get; }

        Turn ChooseTurn();
    }
}