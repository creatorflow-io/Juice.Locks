namespace Juice.Locks
{
    public interface ILock : IDisposable
    {
        public event EventHandler<LockEventArgs> Released;
        public string Key { get; }
        public string Value { get; }
    }

}
