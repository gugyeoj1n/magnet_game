using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public int score = 0;
    public static GameManager instance;

    void Awake( )
    {
        instance = this;
    }

    public void AddScore( int value )
    {
        score += value;
        scoreText.text = score.ToString( );
    }
}