using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class HomesStayImage
{
    public long ImageId { get; set; }

    public long HomeStayId { get; set; }

    public string Url { get; set; } = null!;

    public virtual Homestay HomeStay { get; set; } = null!;
}
