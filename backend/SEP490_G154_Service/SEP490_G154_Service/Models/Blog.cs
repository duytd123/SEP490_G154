using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class Blog
{
    public long BlogId { get; set; }

    public long AuthorId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? CoverImage { get; set; }

    public int? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Author { get; set; } = null!;
}
