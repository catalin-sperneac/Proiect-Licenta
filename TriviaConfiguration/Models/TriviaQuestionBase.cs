using System.Collections.Generic;

namespace TriviaConfiguration.Models;

public class TriviaQuestionBase
{
    public string QuestionText { get; set; } = "";
    public string CorrectAnswer { get; set; } = "";
}