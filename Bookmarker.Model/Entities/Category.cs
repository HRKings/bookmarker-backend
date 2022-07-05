namespace Bookmarker.Model.Entities
{
    public partial class Category
    {
        public Category()
        {
            Bookmarks = new HashSet<Bookmark>();
            InverseParent = new HashSet<Category>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public Guid? ParentId { get; set; }
        public string? Icon { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Category? Parent { get; set; }
        public virtual ICollection<Bookmark> Bookmarks { get; set; }
        public virtual ICollection<Category> InverseParent { get; set; }
    }
}
