namespace Bookmarker.Data;

public static class Queries
{
    public static class Bookmark
    {
        public const string GET_ACTIVE_IDS_URLS = "SELECT id::TEXT, url FROM bookmark WHERE is_link_broken = false";
        public const string GET_ACTIVE_IDS_PURE_URLS = "SELECT id::TEXT, pure_url FROM bookmark WHERE is_link_broken = false";
        public const string INSERT = "INSERT INTO bookmark(url, pure_url, title, description, image_url, image_alt, og_type, video_url) VALUES (@url, @pureUrl, @title, @desc, @image, @alt, @type, @video) RETURNING id::TEXT";

        public const string GET_TOTAL_COUNT = "SELECT COUNT(*) FROM bookmark";

        public const string GET_BY_ID = "SELECT * FROM bookmark b WHERE id = CAST(@id as UUID)";
        public const string GET_ALL_PAGINATED = @"SELECT b.id::TEXT as Id, b.url as Url, b.title as Title, b.description as Description,
        b.image_url as ImageUrl, b.image_alt as ImageAlt, b.og_type as OgType, b.video_url as VideoUrl
    FROM bookmark b LIMIT @limit OFFSET @offset";
        
        public const string GET_DUPLICATED_LINKS =
            "SELECT id::TEXT FROM bookmark WHERE pure_url = @url AND id != CAST(@id as UUID)";
        
        public const string FLAG_BROKEN_LINK = "UPDATE bookmark SET is_link_broken = true WHERE id = CAST(@id as UUID)";
        
        public const string GET_PAGINATED_BY_CATEGORY = @"SELECT b.id::TEXT as Id, b.url as Url, b.title as Title, b.description as Description,
        b.image_url as ImageUrl, b.image_alt as ImageAlt, b.og_type as OgType, b.video_url as VideoUrl
    FROM bookmark b WHERE category_id = CAST(@categoryId as UUID) LIMIT @limit OFFSET @offset";
        public const string GET_COUNT_BY_CATEGORY = "SELECT COUNT(*) FROM bookmark WHERE category_id = CAST(@categoryId as UUID)";

        public const string SET_CATEGORY = @"UPDATE bookmark SET category_id = CAST(@categoryId as UUID) WHERE id = CAST(@id as UUID) RETURNING id::TEXT as Id, url as Url, title as Title, description as Description,
        image_url as ImageUrl, image_alt as ImageAlt, og_type as OgType, video_url as VideoUrl";
    }

    public static class DuplicatedLinks
    {
        public const string FLAG_DUPLICATED_LINK = "INSERT INTO duplicated_links(original_id, duplicated_id) VALUES(CAST(@id as UUID), CAST(@duplicate as UUID)) ON CONFLICT DO NOTHING";
    }

    public static class BookmarkIndex
    {
        public const string GET_TO_BASIC_INDEX = @"SELECT b.id::TEXT as Id, b.url as Url, b.title as Title, b.description as Description,
       b.image_url as ImageUrl, b.image_alt as ImageAlt, b.og_type as OgType, b.video_url as VideoUrl, c.title as CategoryName
    FROM bookmark_index_status INNER JOIN bookmark b ON b.id = bookmark_index_status.id INNER JOIN category c ON b.category_id = c.id WHERE has_basic_index = false";
        public const string GET_ID_TO_BASIC_INDEX = @"SELECT id::TEXT as Id FROM bookmark_index_status WHERE has_basic_index = false";

        public const string SET_INDEX_STATUS = "UPDATE bookmark_index_status SET has_basic_index = @hasBasicIndex, has_article_index = @hasArticleIndex, has_content_index = @hasContentIndex WHERE id = CAST(@id as UUID)";
    }

    public static class Category
    {
        public const string INSERT = "INSERT INTO category(title, icon) VALUES (@title, @icon) RETURNING id::TEXT";
        public const string GET_ALL_PAGINATED = "SELECT c.id::TEXT as Id, c.title as Title, c.parent_id::TEXT as ParentId, c.icon as IconifyIcon FROM category c LIMIT @limit OFFSET @offset";
        public const string GET_BY_ID = "SELECT title, parent_id, icon FROM category b WHERE id = CAST(@id as UUID)";
        
        public const string GET_TOPLEVEL_PAGINATED = "SELECT c.id::TEXT as Id, c.title as Title, c.parent_id::TEXT as ParentId, c.icon as IconifyIcon FROM category c WHERE c.parent_id IS NULL LIMIT @limit OFFSET @offset";

        
        public const string GET_TOTAL_COUNT = "SELECT COUNT(*) FROM category";
        public const string GET_TOPLEVEL_COUNT = "SELECT COUNT(*) FROM category WHERE parent_id IS NULL";

    }
}