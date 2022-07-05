using Index = Meilisearch.Index;

namespace Bookmarker.Search;

public class IndexWrapper<T>
{
    public Index Index { get; init; }

    public IndexWrapper(Index index)
    {
        Index = index;
    }
}