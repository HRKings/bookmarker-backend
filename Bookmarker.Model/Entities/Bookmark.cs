namespace Bookmarker.Model.Entities
{
    public partial class Bookmark
    {
        public Bookmark()
        {
            BookmarkTags = new HashSet<BookmarkTag>();
            DuplicatedLinkDuplicateds = new HashSet<DuplicatedLink>();
            DuplicatedLinkOriginals = new HashSet<DuplicatedLink>();
        }

        public Guid Id { get; set; }
        public string Url { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageAlt { get; set; }
        public string? OgType { get; set; }
        public string? VideoUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsLinkBroken { get; set; }
        public string? PureUrl { get; set; }

        public virtual Category? Category { get; set; }
        public virtual BookmarkIndexStatus BookmarkIndexStatus { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
        public virtual ICollection<BookmarkTag> BookmarkTags { get; set; }
        public virtual ICollection<DuplicatedLink> DuplicatedLinkDuplicateds { get; set; }
        public virtual ICollection<DuplicatedLink> DuplicatedLinkOriginals { get; set; }
    }
}
