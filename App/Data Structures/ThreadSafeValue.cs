using System;
using System.Threading;

public class ThreadSafeValue<T>
{
    private T _value;
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    public T Value
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return _value;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
        set
        {
            _lock.EnterWriteLock();
            try
            {
                _value = value;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }

    public ThreadSafeValue(T value = default(T))
    {
        _value = value;
    }
}
