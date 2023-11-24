namespace Juice.Locks
{
    public interface ILock : IDisposable
    {
        public event EventHandler<LockEventArgs>? Released;
        public string Key { get; }
        public string Value { get; }
        public DateTimeOffset ExpiresOn { get; }
        public bool IsExpired { get; }
        public bool IsReleased { get; }
    }

}
