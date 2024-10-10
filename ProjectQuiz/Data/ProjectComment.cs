using System;
using System.Collections.Generic;

namespace ProjectQuiz.Data;

public partial class ProjectComment
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public int? UserId { get; set; }

    public string? Comment { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? User { get; set; }
}
