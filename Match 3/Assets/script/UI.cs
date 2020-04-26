using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    public Text textScore, textMove;
    private int score = 0;
    private int moves = 10;

    public GameObject panelGameOver;
    public Text gTextscore, gTextBestScore;

    private void Awake()
    {
        instance = this;
    }

    public void Score(int value)
    {
        score += value;
        textScore.text = "Score: " + score.ToString();
    }

    public void Moves(int value)
    {
        moves -=value;
        if (moves <= 0)
        {
            GameOver();
        }
        textMove.text = "Moves: " + moves.ToString();
    }

    private void GameOver()
    {
        if (score > PlayerPrefs.GetInt("Score"))
        {
            PlayerPrefs.SetInt("Score", score);
            gTextBestScore.text = "New Best: " + score.ToString();
        }
        else
            gTextBestScore.text = "Best: " + PlayerPrefs.GetInt("Score");
        gTextBestScore.text = "Score: " + score.ToString();
        panelGameOver.SetActive(true);
    }
}
