using System.Collections.Generic;
using UnityEngine;

public static class LocalQuestionList
{
    public static Question[] questions;

    public static Question getQuestion(int index)
    {
        if (index >= questions.Length)
        {
            return null;
        }

        return questions[index];
    }
}
