using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        var seq = DOTween.Sequence( );
        if( target == State.X )
        {
            seq.Append( transform.DOScale( 0f, 0.5f ).OnComplete( ( ) => {
                SetColor( target );
                transform.localScale = Vector3.one;
            } ) );
        }
        else
        {
            SetColor( target );
            seq.Append( transform.DOScale ( 0.7f, 0.5f ) );
            seq.Append( transform.DOScale ( 1f, 0.5f ) );
        }
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