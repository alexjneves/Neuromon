namespace Common.Turn
{
    public sealed class ChangeNeuromon : ITurn
    {
        public Neuromon Neuromon { get; }

        public ChangeNeuromon(Neuromon neuromon)
        {
            Neuromon = neuromon;
        }
    }
}