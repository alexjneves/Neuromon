namespace Neuromon
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            var battleSimulator = new BattleSimulator();
            var renderer = new Renderer(battleSimulator);

            battleSimulator.Run();
        }
    }
}
