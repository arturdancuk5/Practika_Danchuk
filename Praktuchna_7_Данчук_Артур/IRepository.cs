namespace Praktuchna_7_Данчук_Артур;

public interface IRepository<T> where T : class
{
    void Add(T item);

    void Remove(T item);

    List<T> GetAll();
}
