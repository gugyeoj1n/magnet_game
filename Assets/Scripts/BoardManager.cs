using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class BoardManager : MonoBehaviour
{
    private enum Direction { Up, Down, Left, Right }
    private int[] dx = new int[] { 0, 0, 1, -1 };
    private int[] dy = new int[] { 1, -1, 0, 0 };

    public int boardWidth;
    public int boardHeight;
    public List<Cell> cells;
    public Cell[ , ] board;

    public Queue<State> stateQueue;
    public Image[] queuedCellImages;

    [System.Serializable]
    public class CellProb
    {
        public State state;
        public int percentage;
    }

    [SerializeField]
    private List<CellProb> cellProbs;

    public static BoardManager instance;

    void Awake( )
    {
        instance = this;
    }

    void Start( )
    {
        InitBoard( );
        InitQueue( );
    }

    private void InitQueue( )
    {
        stateQueue = new Queue<State>( );
        for( int i = 0; i < 4; i++ )
        {
            State randomState = PickState( );
            stateQueue.Enqueue( randomState );
            SetCellImage( queuedCellImages[ i ], randomState );
        }    
    }

    private void SetCellImage( Image target, State state )
    {
        if( state == State.S )
        {
            target.sprite = CellManager.instance.sImage;
        } else if( state == State.N )
        {
            target.sprite = CellManager.instance.nImage;
        } else if( state == State.Super ) {
            target.sprite = CellManager.instance.superImage;
        } else {
            target.sprite = CellManager.instance.shellImage;
        }
    }

    private State PickState()
    {
        int randomValue = Random.Range( 0, 100 );
        int temp = 0;
        for( int i = 0; i < cellProbs.Count; i++ )
        {
            temp += cellProbs[ i ].percentage;
            if( randomValue < temp )
                return cellProbs[ i ].state;
        }

        return State.X;
    }

    private void InitBoard( )
    {
        board = new Cell[ boardWidth, boardHeight ];

        int cellIdx = 0;
        for( int i = 0; i < boardWidth; i++ )
            for( int j = 0; j < boardHeight; j++)
            {
                board[ i, j ] = cells[ cellIdx ];
                board[ i, j ].idx = cellIdx;
                board[ i, j ].x = i;
                board[ i, j ].y = j;
                cellIdx++;
            }

        List<Vector2Int> allPoints = new List<Vector2Int>( );
        for ( int x = 0; x < boardWidth; x++ )
            for ( int y = 0; y < boardHeight; y++ )
                allPoints.Add( new Vector2Int ( x, y ) );
                
        for ( int i = 0; i < 10; i++ )
        {
            int index = Random.Range( 0, allPoints.Count );
            Cell target = board[ allPoints[ index ].x, allPoints[ index ].y ];
            target.SetState( PickState( ) );
            if( target.state == State.Shell )
            {
                target.shellCount = 3;
                GameObject countText = Instantiate( CellManager.instance.shellCountText, target.transform );
                countText.GetComponent<TMP_Text>( ).text = "3";
                countText.transform.SetParent( target.transform, false );
            }

            allPoints.RemoveAt( index );
        }
    }

    private void ChangeNextCell( )
    {
        stateQueue.Dequeue( );
        State randomState = PickState( );
        stateQueue.Enqueue( randomState );
        
        for( int i = 0; i < 4; i++ )
            SetCellImage( queuedCellImages[ i ], stateQueue.ElementAt( i ) );
    }

    public void ClickCell( int x, int y )
    {   
        foreach( Cell cell in cells )
            cell.LockClick( );

        CheckOver( );
        ChangeNextCell( );
        Cell newRandomCell = GetRandomEmptyCell( );

        if( newRandomCell != null )
        {
            newRandomCell.SetState( PickState( ) );
            if( newRandomCell.state == State.Shell )
            {
                newRandomCell.shellCount = 3;
                GameObject countText = Instantiate( CellManager.instance.shellCountText, newRandomCell.transform );
                countText.GetComponent<TMP_Text>( ).text = "3";
                countText.transform.SetParent( newRandomCell.transform, false );
            }
        }
            
        UpdateBoard( x, y );

        StartCoroutine( CheckOver( ) );
    }

    private void UpdateBoard( int x, int y )
    {
        State centerState = board[ x, y ].state;
        if( centerState == State.Shell )
            return;

        for( int i = 0; i < 4; i++ )
        {
            int newX = x + dx[ i ];
            int newY = y + dy[ i ];

            if( newX < 0 || newX >= boardWidth || newY < 0 || newY >= boardHeight )
                continue;

            Cell target = board[ x + dx[ i ], y + dy[ i ] ];

            if( centerState == State.Super )
            {
                if( target.state != State.X )
                {
                    target.RemoveCell( target, board[ x, y ] );
                    GameManager.instance.AddScore( 1 );
                    continue;
                }
            }

            if( target.state == State.Shell )
            {
                if( target.shellCount == 0 )
                    target.RemoveCell( target, board[ x, y ] );
                else
                    MoveCell( target, dx[ i ], dy[ i ] );
                continue;
            }

            if( target.state != centerState && target.state != State.X )
            {
                target.RemoveCell( target, board[ x, y ] );
                GameManager.instance.AddScore( 1 );
            }
            else if( target.state == centerState && target.state != State.X )
            {
                MoveCell( target, dx[ i ], dy[ i ] );
            }
        }
    }

    private void MoveCell( Cell target, int dx, int dy )
    {
        int newX = target.x + dx;
        int newY = target.y + dy;
        if( newX >= 0 && newX < boardWidth && newY >= 0 && newY < boardHeight && board[ newX, newY ].state == State.X )
        {
            if( target.state == State.Shell )
            {
                target.shellCount--;
                target.transform.GetChild( 0 ).GetComponent<TMP_Text>( ).text = target.shellCount.ToString( );
            }
            target.MoveCell( target, board[ newX, newY ] );
            UpdateBoard( newX, newY );
        }
    }

    private Cell GetRandomEmptyCell( )
    {   
        List<int> emptyCells = new List<int>( );
        foreach( Cell cell in cells )
            if( cell.state == State.X )
                emptyCells.Add( cell.idx );

        if( emptyCells.Count <= 0 )
        {
            return null;
        }
        int idx = emptyCells[ Random.Range( 0, emptyCells.Count ) ];
        return cells[ idx ];
    }

    private IEnumerator CheckOver( )
    {
        yield return new WaitForSeconds( 1f );
        bool isGameOver = cells.All( cell => cell.state != State.X );
        if( isGameOver )
            GameManager.instance.GameOver( );
    }
}