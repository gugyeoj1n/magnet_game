using UnityEngine;
using UnityEngine.UI;

public enum State { X, N, S }

public class Cell : MonoBehaviour
{
    public State state;
    public int idx;
    
    public Color XColor = new Color( 0.2358491f, 0.2358491f, 0.2358491f );
    public Color NColor = new Color( 1f, 0.5235849f, 0.5235849f );
    public Color SColor = new Color( 0.5254902f, 0.6301079f, 1f );

    private Image image;
    private Button button;

    void Awake( )
    {
        image = GetComponent<Image>( );
        button = GetComponent<Button>( );

        button.onClick.AddListener( ChangeCell );
    }

    private void ChangeCell( )
    {
        SetState( BoardManager.instance.nextState );
        BoardManager.instance.ClickCell( idx );
    }

    public void SetState( State target )
    {
        state = target;
        SetColor( target );
    }

    private void SetColor( State target )
    {
        switch ( state )
        {
            case State.X :
                image.color = XColor;
                button.enabled = true;
                break;
            case State.N :
                image.color = NColor;
                button.enabled = false;
                break;
            case State.S :
                image.color = SColor;
                button.enabled = false;
                break;
            default :
                break;
        }
    }
}