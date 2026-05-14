using System;
using System.Collections.Generic;

namespace DiplomProject.Models;

public partial class User
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public int RoleId { get; set; }

    public int? GroupId { get; set; }

    public string? ProfileImage { get; set; }

    public virtual Group? Group { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TestResult> TestResults { get; set; } = new List<TestResult>();

    public virtual ICollection<UserTestSession> UserTestSessions { get; set; } = new List<UserTestSession>();
}
