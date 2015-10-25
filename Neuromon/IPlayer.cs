namespace Neuromon
{
    internal interface IPlayer
    {
        string Name { get; }
        Neuromon Neuromon { get; }

        Turn ChooseTurn();
    }
}