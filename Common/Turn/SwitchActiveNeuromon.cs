namespace Common.Turn
{
    public sealed class SwitchActiveNeuromon : ITurn
    {
        public Neuromon Neuromon { get; }

        public SwitchActiveNeuromon(Neuromon neuromon)
        {
            Neuromon = neuromon;
        }
    }
}