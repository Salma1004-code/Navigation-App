public class Queue<T>
{
    private LinkedList<T> list = new LinkedList<T>();

    public int Count
    {
        get
        {
            lock (list)
            {
                return list.Count;
            }
        }
    }

    public bool IsEmpty
    {
        get
        {
            lock (list)
            {
                return list.Count == 0;
            }
        }
    }

    public void Enqueue(T value)
    {
        lock (list)
        {
            list.AddLast(value);
        }
    }

    public T Dequeue()
    {
        lock (list)
        {
            if (list.Count == 0)
            {
                return default(T); // returns null for reference types and zero for value types
            }

            T value = list.First.Value;
            list.RemoveFirst();
            return value;
        }
    }

    public void Clear()
    {
        lock (list)
        {
            list.Clear();
        }
    }

    public T Peek()
    {
        lock (list)
        {
            if (list.Count == 0)
            {
                return default(T); // returns null for reference types and zero for value types
            }

            return list.First.Value;
        }
    }
}
