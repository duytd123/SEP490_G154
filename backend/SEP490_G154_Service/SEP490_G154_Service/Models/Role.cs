using System;
using System.Collections.Generic;

namespace SEP490_G154_Service.Models;

public partial class Role
{
    public long RoleId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
