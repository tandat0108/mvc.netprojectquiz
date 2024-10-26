using System;
using System.Collections.Generic;

namespace ProjectQuiz.Data;

public partial class UserQuiz
{
    public int UserQuizId { get; set; }

    public int UserId { get; set; }

    public int ProjectId { get; set; }

    public decimal? TotalScore { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
