using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public enum State { X, N, S, Super, Shell }

public class Cell : MonoBehaviour
{
    public State state;
    public int idx;
    public int x;
    public int y;
    public int shellCount;
    
    private Image image;
    private Button button;

    public Transform cellAnim;
    public Transform removeCellAnim;

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
        SetState( BoardManager.instance.stateQueue.Peek( ) );
        BoardManager.instance.ClickCell( x, y );

        if( state == State.Shell )
        {
            shellCount = 3;
            GameObject countText = Instantiate( CellManager.instance.shellCountText, transform );
            countText.GetComponent<TMP_Text>( ).text = "3";
            countText.transform.SetParent( transform, false );
        }
    }

    public void LockClick( )
    {
        StartCoroutine( StopClick( ) );
    }

    private IEnumerator StopClick( )
    {
        button.enabled = false;
        yield return new WaitForSeconds( 1f );
        if( state == State.X )
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

    public void RemoveCell( Cell start, Cell end )
    {
        GameObject copiedCell = Instantiate( this.gameObject, removeCellAnim );
        if (start.state == State.Shell)
        {
            Destroy( start.transform.GetChild( 0 ).gameObject );
            start.shellCount = 0;
        }
        
        RectTransform copyRect = copiedCell.GetComponent<RectTransform>();
        copyRect.anchorMin = new Vector2(0.5f, 0.5f);
        copyRect.anchorMax = new Vector2(0.5f, 0.5f);
        copyRect.pivot = new Vector2(0.5f, 0.5f);

        Transform copiedTransform = copiedCell.transform;
        copiedTransform.position = transform.position;
        Vector3 prevPosition = copiedTransform.position;

        start.state = State.X;
        start.SetColor( State.X );

        copiedTransform.DOScale( 0f, 1f );
        copiedTransform.DOMove( end.transform.position, 1f ).OnComplete( ( ) => {
            Destroy( copiedCell );
        } );
    }

    public void MoveCell( Cell start, Cell end )
    {
        GameObject copiedCell = Instantiate( this.gameObject, cellAnim );
        if( start.state == State.Shell )
        {
            Destroy( start.transform.GetChild( 0 ).gameObject );
            end.shellCount = start.shellCount;
            start.shellCount = 0;
        }
            

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
            copiedTransform.GetChild( 0 ).SetParent( end.transform );
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
            case State.Super :
                image.sprite = CellManager.instance.superImage;
                button.enabled = false;
                break;
            case State.Shell :
                image.sprite = CellManager.instance.shellImage;
                button.enabled = false;
                break;
            default :
                break;
        }
    }
}