using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public TMP_Text versionText;

    void Start()
    {
        versionText.text = Application.version;
    }

    public void Play( )
    {
        SceneManager.LoadScene( "Play" );
    }
}