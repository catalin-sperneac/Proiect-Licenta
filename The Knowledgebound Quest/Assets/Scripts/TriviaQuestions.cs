using UnityEngine;
using System;

namespace MyGame.Trivia
{
    [Serializable]
    public class QuestionBase
    {
        public string questionText;
        public string correctAnswer;
        public int correctCount;

        public bool AutoAnswer()
        {
            return UnityEngine.Random.Range(0, 100) < correctCount * 20;
        }

        public void ResetProgress()
        {
            correctCount = 0;
        }

        public virtual bool CheckAnswer(string input, out int status)
        {
            status = (input == correctAnswer) ? 1 : 2;
            if (status == 1) correctCount++;
            return status == 1;
        }
    }

    [Serializable]
    public class MultipleChoiceQuestion : QuestionBase
    {
        public string[] options;

        public override bool CheckAnswer(string input, out int status)
        {
            if (input == correctAnswer)
            {
                status = 1;
                correctCount++;
            }
            else
            {
                status = 2;
            }
            return input == correctAnswer;
        }
    }

    [Serializable]
    public class InputQuestion : QuestionBase
    {
        public float firstIntervalPercentage = 0.03f;
        public float secondIntervalPercentage = 0.1f;

        public override bool CheckAnswer(string input, out int status)
        {
            int selectedNumber = int.Parse(input);
            int correctNumber = int.Parse(correctAnswer);
            bool firstInterval = false;
            bool secondInterval = false;
            firstInterval = (selectedNumber >= (int)(correctNumber - (correctNumber * firstIntervalPercentage)) && selectedNumber <= (int)(correctNumber + (correctNumber * firstIntervalPercentage)));
            secondInterval = (selectedNumber >= (int)(correctNumber - (correctNumber * secondIntervalPercentage)) && selectedNumber <= (int)(correctNumber + (correctNumber * secondIntervalPercentage)));
            if (firstInterval && secondInterval)
            {
                status = 1;
                correctCount++;
            }
            else if (!firstInterval && secondInterval)
            {
                status = 0;
            }
            else
            {
                status = 2;
            }
            return status == 1;
        }
    }
}

