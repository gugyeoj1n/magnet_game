using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider volumeSlider;
    private AudioSource[] sources;

    void Start( )
    {
        sources = FindObjectsOfType<AudioSource>( );

        if( PlayerPrefs.GetInt( "first" ) == 0 )
        {
            PlayerPrefs.SetInt( "first", 1 );
            PlayerPrefs.SetFloat( "volume", 1f );
        }

        float value = PlayerPrefs.GetFloat( "volume" );
        SetVolume( value );
        volumeSlider.value = value;
        volumeSlider.onValueChanged.AddListener( SetVolume );
    }

    public void SetVolume( float value )
    {
        PlayerPrefs.SetFloat( "volume" , value );

        foreach( AudioSource source in sources )
            source.volume = value;
    }
}