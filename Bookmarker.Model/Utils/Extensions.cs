namespace Bookmarker.Model.Utils;

public static class Extensions
{
    public static Guid ToGuid(this string? uuid)
        => new (uuid);
}