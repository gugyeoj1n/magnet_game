using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public enum State { X, N, S }

public class Cell : MonoBehaviour
{
    public State state;
    public int idx;
    public int x;
    public int y;
    
    private Image image;
    private Button button;

    public Transform cellAnim;

    private AudioSource audio;
    public AudioClip putSound;

    void Awake( )
    {
        image = GetComponent<Image>( );
        button = GetComponent<Button>( );
        audio = GetComponent<AudioSource>( );

        button.onClick.AddListener( ChangeCell );
    }

    private void ChangeCell( )
    {
        audio.clip = putSound;
        audio.Play( );
        SetState( BoardManager.instance.nextState );
        BoardManager.instance.ClickCell( x, y );
    }

    public void LockClick( )
    {
        StartCoroutine( StopClick( ) );
    }

    private IEnumerator StopClick( )
    {
        button.enabled = false;
        yield return new WaitForSeconds( 1f );
        button.enabled = true;
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

    public void MoveCell( Cell start, Cell end )
    {
        GameObject copiedCell = Instantiate( this.gameObject, cellAnim );

        RectTransform copyRect = copiedCell.GetComponent<RectTransform>();
        copyRect.anchorMin = new Vector2(0.5f, 0.5f);
        copyRect.anchorMax = new Vector2(0.5f, 0.5f);
        copyRect.pivot = new Vector2(0.5f, 0.5f);

        Transform copiedTransform = copiedCell.transform;
        copiedTransform.position = transform.position;
        Vector3 prevPosition = copiedTransform.position;

        end.state = start.state;
        start.state = State.X;
        start.SetColor( State.X );

        copiedTransform.DOMove( end.transform.position, 1f ).OnComplete( ( ) => {
            end.SetColor( end.state );
            Destroy( copiedCell );
        } );
    }


    private void SetColor( State target )
    {
        switch ( state )
        {
            case State.X :
                image.sprite = CellManager.instance.xImage;
                button.enabled = true;
                break;
            case State.N :
                image.sprite = CellManager.instance.nImage;
                button.enabled = false;
                break;
            case State.S :
                image.sprite = CellManager.instance.sImage;
                button.enabled = false;
                break;
            default :
                break;
        }
    }
}