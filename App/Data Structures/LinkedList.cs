public class LinkedListNode<T>
{
    public T Value { get; set; }
    public LinkedListNode<T> Next { get; set; }
    public LinkedListNode<T> Previous { get; set; }

    public LinkedListNode(T value)
    {
        Value = value;
    }
}

public class LinkedList<T>
{
    public int Count { get; private set; }
    public bool IsEmpty => First == null;
    public LinkedListNode<T> First { get; private set; }
    public LinkedListNode<T> Last { get; private set; }

    public void Append(T value)
    {
        var node = new LinkedListNode<T>(value);

        if (Last != null)
        {
            node.Previous = Last;
            Last.Next = node;
        }
        else
        {
            First = node;
        }

        Last = node;
        Count++;
    }

    public LinkedListNode<T> NodeAt(int index)
    {
        if (index < 0)
        {
            return null;
        }

        var node = First;
        while (node != null && index-- > 0)
        {
            node = node.Next;
        }

        return node;
    }

    public void Clear()
    {
        First = null;
        Last = null;
        Count = 0;
    }

    public T Remove(LinkedListNode<T> node)
    {
        if (node.Previous != null)
        {
            node.Previous.Next = node.Next;
        }
        else
        {
            First = node.Next;
        }

        if (node.Next != null)
        {
            node.Next.Previous = node.Previous;
        }
        else
        {
            Last = node.Previous;
        }

        Count--;

        node.Previous = null;
        node.Next = null;

        return node.Value;
    }
}
