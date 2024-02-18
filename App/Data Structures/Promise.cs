using System;
using System.Collections.Generic;
using System.Threading;

public class Promise<T>
{
    private enum State
    {
        Pending,
        Resolved
    }

    private State state = State.Pending;
    private T resolvedValue;
    private readonly object locker = new object();
    private readonly List<Action<T>> callbacks = new List<Action<T>>();

    public Promise(Action<Action<T>> executor)
    {
        executor(Resolve);
    }

    public void Then(Action<T> onResolved)
    {
        lock (locker)
        {
            if (state == State.Resolved)
            {
                ThreadPool.QueueUserWorkItem(_ => onResolved(resolvedValue));
            }
            else
            {
                callbacks.Add(onResolved);
            }
        }
    }

    private void Resolve(T value)
    {
        lock (locker)
        {
            if (state != State.Pending) return;

            state = State.Resolved;
            resolvedValue = value;

            foreach (var callback in callbacks)
            {
                ThreadPool.QueueUserWorkItem(_ => callback(resolvedValue));
            }

            callbacks.Clear();
        }
    }
}
