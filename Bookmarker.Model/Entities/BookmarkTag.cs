namespace Bookmarker.Model.Entities
{
    public partial class BookmarkTag
    {
        public Guid BookmarkId { get; set; }
        public Guid TagId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Bookmark Bookmark { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
