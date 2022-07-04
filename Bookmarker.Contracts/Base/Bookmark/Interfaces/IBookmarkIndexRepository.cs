using Bookmarker.Contracts.Base.Bookmark.Index;

namespace Bookmarker.Contracts.Base.Bookmark.Interfaces;

public interface IBookmarkIndexRepository
{
    public Task<IEnumerable<BookmarkToIndex>?> GetBookmarksToBeFirstIndexed();
    public Task<IEnumerable<string>?> GetBookmarksIdToBeFirstIndexed();
    public Task<int> SetIndexStatus(string id, bool hasBasicIndex, bool hasArticleIndex, bool hasContentIndex);

}