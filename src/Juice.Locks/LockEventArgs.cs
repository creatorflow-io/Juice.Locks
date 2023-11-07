namespace Juice.Locks
{
    public class LockEventArgs
    {
        public string Key { get; init; }
        public string Issuer { get; init; }
    }
}
