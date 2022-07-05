using Bookmarker.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookmarker.Model
{
    public partial class BookmarkerContext : DbContext
    {
        public BookmarkerContext()
        {
        }

        public BookmarkerContext(DbContextOptions<BookmarkerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bookmark> Bookmarks { get; set; } = null!;
        public virtual DbSet<BookmarkIndexStatus> BookmarkIndexStatuses { get; set; } = null!;
        public virtual DbSet<BookmarkTag> BookmarkTags { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<DuplicatedLink> DuplicatedLinks { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=bookmarker;User Id=postgres;Password=password;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bookmark>(entity =>
            {
                entity.ToTable("bookmark");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasDefaultValueSql("'54453511-6970-4fb7-b3a4-51714ce5fd35'::uuid");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.ImageAlt).HasColumnName("image_alt");

                entity.Property(e => e.ImageUrl).HasColumnName("image_url");

                entity.Property(e => e.IsLinkBroken)
                    .HasColumnName("is_link_broken")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.OgType).HasColumnName("og_type");

                entity.Property(e => e.PureUrl).HasColumnName("pure_url");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Url).HasColumnName("url");

                entity.Property(e => e.VideoUrl).HasColumnName("video_url");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Bookmarks)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("bookmark_category_id_fk");
            });

            modelBuilder.Entity<BookmarkIndexStatus>(entity =>
            {
                entity.ToTable("bookmark_index_status");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.HasArticleIndex)
                    .HasColumnName("has_article_index")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.HasBasicIndex).HasColumnName("has_basic_index");

                entity.Property(e => e.HasContentIndex)
                    .HasColumnName("has_content_index")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("now()");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.BookmarkIndexStatus)
                    .HasForeignKey<BookmarkIndexStatus>(d => d.Id)
                    .HasConstraintName("bookmark_index_status_id_fkey");
            });

            modelBuilder.Entity<BookmarkTag>(entity =>
            {
                entity.HasKey(e => new { e.BookmarkId, e.TagId })
                    .HasName("bookmark_tag_pkey");

                entity.ToTable("bookmark_tag");

                entity.Property(e => e.BookmarkId).HasColumnName("bookmark_id");

                entity.Property(e => e.TagId).HasColumnName("tag_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("now()");

                entity.HasOne(d => d.Bookmark)
                    .WithMany(p => p.BookmarkTags)
                    .HasForeignKey(d => d.BookmarkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bookmark_tag_bookmark_id_fkey");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.BookmarkTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bookmark_tag_tag_id_fkey");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.HasIndex(e => e.Title, "category_title_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Icon)
                    .HasColumnName("icon")
                    .HasDefaultValueSql("'bi:bookmark-star'::text");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("now()");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("category_parent_id_fkey");
            });

            modelBuilder.Entity<DuplicatedLink>(entity =>
            {
                entity.HasKey(e => new { e.OriginalId, e.DuplicatedId })
                    .HasName("duplicated_links_pkey");

                entity.ToTable("duplicated_links");

                entity.Property(e => e.OriginalId).HasColumnName("original_id");

                entity.Property(e => e.DuplicatedId).HasColumnName("duplicated_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.UserIgnore)
                    .HasColumnName("user_ignore")
                    .HasDefaultValueSql("false");

                entity.HasOne(d => d.Duplicated)
                    .WithMany(p => p.DuplicatedLinkDuplicateds)
                    .HasForeignKey(d => d.DuplicatedId)
                    .HasConstraintName("duplicated_links_duplicated_id_fkey");

                entity.HasOne(d => d.Original)
                    .WithMany(p => p.DuplicatedLinkOriginals)
                    .HasForeignKey(d => d.OriginalId)
                    .HasConstraintName("duplicated_links_original_id_fkey");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tag");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Title).HasColumnName("title");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("now()");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Tag)
                    .HasForeignKey<Tag>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tag_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
