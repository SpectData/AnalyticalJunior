[System.Serializable]
public class ReviewItem
{
    public string questionText;
    public string studentAnswer;
    public string correctAnswer;
    public string explanation;

    public ReviewItem(string questionText, string studentAnswer,
        string correctAnswer, string explanation)
    {
        this.questionText = questionText;
        this.studentAnswer = studentAnswer;
        this.correctAnswer = correctAnswer;
        this.explanation = explanation;
    }
}
