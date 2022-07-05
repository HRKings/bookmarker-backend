namespace Bookmarker.Model.Entities
{
    public partial class BookmarkIndexStatus
    {
        public Guid Id { get; set; }
        public bool HasBasicIndex { get; set; }
        public bool? HasArticleIndex { get; set; }
        public bool? HasContentIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Bookmark IdNavigation { get; set; } = null!;
    }
}
