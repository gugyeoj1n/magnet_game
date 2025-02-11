using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public int score = 0;
    public static GameManager instance;

    public GameObject overPanel;
    public TMP_Text overText;
    public TMP_Text overScoreText;
    public Button restartButton;
    public Button homeButton;

    private AudioSource audio;
    public AudioClip overSound;

    void Awake( )
    {
        instance = this;
    }

    void Start( )
    {
        audio = GetComponent<AudioSource>( );
    }

    public void AddScore( int value )
    {
        score += value;
        scoreText.text = score.ToString( );
    }

    public void GameOver( )
    {
        audio.clip = overSound;
        audio.Play( );
        BoardManager.instance.enabled = false;

        overPanel.SetActive( true );
        overScoreText.text = string.Format( "{0}점", score );

        var seq = DOTween.Sequence( );
        seq.AppendInterval( 2f );
        seq.Append( overText.transform.DOMove( overText.transform.position + Vector3.up * 200f, 1f ) );
        seq.Join( overScoreText.DOFade( 1f, 1f ) );
        seq.Join( restartButton.GetComponent<Image>( ).DOFade( 1f, 1f ) );
        seq.Join( restartButton.transform.GetChild( 0 ).GetComponent<TMP_Text>( ).DOFade( 1f, 1f ) );
        seq.Join( homeButton.GetComponent<Image>( ).DOFade( 1f, 1f ) );
        seq.Join( homeButton.transform.GetChild( 0 ).GetComponent<TMP_Text>( ).DOFade( 1f, 1f ) );
    }

    public void RestartGame( )
    {
        SceneManager.LoadScene( "Play" );
    }

    public void LoadHome( )
    {
        SceneManager.LoadScene( "Title" );
    }
}