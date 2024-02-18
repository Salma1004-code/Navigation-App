public class BoundedStack<T>
{
    private int bound;
    private List<T> elements = new List<T>();

    public BoundedStack(int bound)
    {
        if (bound <= 0)
        {
            throw new ArgumentException("The BoundedStack must have a bound greater than zero!");
        }

        this.bound = bound;
    }

    public void Push(T value)
    {
        elements.Add(value);

        if (elements.Count > bound)
        {
            elements.RemoveAt(0);
        }
    }

    public void Push(IEnumerable<T> array)
    {
        elements.AddRange(array);

        while (elements.Count > bound)
        {
            elements.RemoveAt(0);
        }
    }

    public T Pop()
    {
        if (elements.Count == 0)
        {
            return default(T); // returns null for reference types and zero for value types
        }

        T lastElement = elements[elements.Count - 1];
        elements.RemoveAt(elements.Count - 1);
        return lastElement;
    }

    public List<T> Remove(Predicate<T> predicate)
    {
        List<T> removed = elements.FindAll(predicate);
        elements.RemoveAll(predicate);
        return removed;
    }

    public void Clear()
    {
        elements.Clear();
    }

    public T Peek()
    {
        if (elements.Count == 0)
        {
            return default(T); // returns null for reference types and zero for value types
        }

        return elements[elements.Count - 1];
    }

    public bool IsEmpty
    {
        get { return elements.Count == 0; }
    }

    public int Count
    {
        get { return elements.Count; }
    }
}
