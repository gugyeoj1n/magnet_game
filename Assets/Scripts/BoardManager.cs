using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    public static BoardManager instance;

    void Awake( )
    {
        instance = this;
    }

    void Start( )
    {
        if (!PlayerPrefs.HasKey("isFirstRun"))
        {
            // 게임이 처음 실행되는 경우
            PlayerPrefs.SetInt("isFirstRun", 1); // 키 저장
            PlayerPrefs.Save(); // 변경 사항 저장

            // 초기화 작업 수행 (예: 튜토리얼 시작, 기본 설정 등)
            Debug.Log("게임이 처음 실행되었습니다.");
        }
        else
        {
            // 게임이 이미 실행된 경우
            Debug.Log("게임이 이미 실행되었습니다.");
        }

        InitBoard( );
        InitQueue( );
    }

    private void InitQueue( )
    {
        stateQueue = new Queue<State>( );
        for( int i = 0; i < 4; i++ )
        {
            State randomState = ( State ) Random.Range( 1, 4 );
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
        } else {
            target.sprite = CellManager.instance.superImage;
        }
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
            Debug.Log( allPoints[ index ].x + " / " + allPoints[ index ].y );
            board[ allPoints[ index ].x, allPoints[ index ].y ].SetState( ( State ) Random.Range( 1, 3 ) );
            allPoints.RemoveAt( index );
        }
    }

    private void ChangeNextCell( )
    {
        stateQueue.Dequeue( );
        State randomState = ( State ) Random.Range( 1, 4 );
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
            newRandomCell.SetState( ( State ) Random.Range( 1, 3 ) );
        UpdateBoard( x, y );

        StartCoroutine( CheckOver( ) );
    }

    private void UpdateBoard( int x, int y )
    {
        State centerState = board[ x, y ].state;
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
                    target.SetState( State.X );
                    GameManager.instance.AddScore( 1 );
                    continue;
                }
            }

            if( target.state != centerState && target.state != State.X )
            {
                target.SetState( State.X );
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