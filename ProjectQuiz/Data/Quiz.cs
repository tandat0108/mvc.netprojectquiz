using System;
using System.Collections.Generic;

namespace ProjectQuiz.Data;

public partial class Quiz
{
    public int QuizId { get; set; }

    public string Title { get; set; } = null!;

    public int? ProjectId { get; set; }

    public string QuestionType { get; set; } = null!;

    public string QuestionText { get; set; } = null!;

    public int IsCorrect { get; set; }

    public string Answers { get; set; } = null!;

    public virtual Project? Project { get; set; }

    public virtual ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
}
