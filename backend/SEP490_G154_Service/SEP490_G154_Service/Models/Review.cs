using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class Review
{
    public long ReviewId { get; set; }

    public string EntityType { get; set; } = null!;

    public long EntityId { get; set; }

    public long UserId { get; set; }

    public long? ParentReviewId { get; set; }

    public int? Rating { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Review> InverseParentReview { get; set; } = new List<Review>();

    public virtual Review? ParentReview { get; set; }

    public virtual User User { get; set; } = null!;
}
