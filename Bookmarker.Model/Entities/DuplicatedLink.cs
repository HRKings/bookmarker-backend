namespace Bookmarker.Model.Entities
{
    public partial class DuplicatedLink
    {
        public Guid OriginalId { get; set; }
        public Guid DuplicatedId { get; set; }
        public bool? UserIgnore { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Bookmark Duplicated { get; set; } = null!;
        public virtual Bookmark Original { get; set; } = null!;
    }
}
