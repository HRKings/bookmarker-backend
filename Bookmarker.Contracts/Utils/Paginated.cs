namespace Bookmarker.Contracts.Utils;

public class Paginated<T>
{
    public IEnumerable<T> Content { get; set; }
    public int Page { get; set; }
    public int ItemsPerPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public bool HasNext { get; set; }
}