using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource[] sources;

    void Start( )
    {
        sources = FindObjectsOfType<AudioSource>( );
    }

    public void SetVolume( float value )
    {
        foreach( AudioSource source in sources )
            source.volume = value;
    }
}