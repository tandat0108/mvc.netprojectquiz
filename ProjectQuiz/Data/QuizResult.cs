using System;
using System.Collections.Generic;

namespace ProjectQuiz.Data;

public partial class QuizResult
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? QuizId { get; set; }

    public decimal? Score { get; set; }

    public string? Comment { get; set; }

    public virtual Quiz? Quiz { get; set; }

    public virtual User? User { get; set; }
}
