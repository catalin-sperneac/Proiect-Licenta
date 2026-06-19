using System.Collections.Generic;

namespace TriviaConfiguration.Models;

public class MultipleChoiceQuestion : TriviaQuestionBase
{
    public List<string> Answers { get; set; } = new();
}