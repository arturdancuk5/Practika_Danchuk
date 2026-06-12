namespace Praktuchna_9_Данчук_Артур;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _items.Add(item);
    }

    public void Remove(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _items.Remove(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_items);
    }
}
