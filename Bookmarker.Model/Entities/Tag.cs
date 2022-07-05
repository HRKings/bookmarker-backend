namespace Bookmarker.Model.Entities
{
    public partial class Tag
    {
        public Tag()
        {
            BookmarkTags = new HashSet<BookmarkTag>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Bookmark IdNavigation { get; set; } = null!;
        public virtual ICollection<BookmarkTag> BookmarkTags { get; set; }
    }
}
