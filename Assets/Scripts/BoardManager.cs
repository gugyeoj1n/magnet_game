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

    public State nextState;
    public Image nextCellImage;

    public static BoardManager instance;

    void Awake( )
    {
        instance = this;
    }

    void Start( )
    {
        InitBoard( );
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
        if( nextState == State.N )
        {
            nextState = State.S;
            nextCellImage.color = new Color( 0.5254902f, 0.6301079f, 1f );
        } else
        {
            nextState = State.N;
            nextCellImage.color = new Color( 1f, 0.5235849f, 0.5235849f );
        }
    }

    public void ClickCell( int x, int y )
    {   
        Cell newRandomCell = GetRandomEmptyCell( );
        newRandomCell.SetState( ( State ) Random.Range( 1, 3 ) );
        ChangeNextCell( );
        UpdateBoard( x, y );
    }

    private void UpdateBoard( int x, int y )
    {
        State centerState = board[ x, y ].state;
        for( int i = 0; i < 4; i++ )
        {
            int newX = x + dx[ i ];
            int newY = y + dy[ i ];

            if( newX < 0 || newX >= boardWidth || newY < 0 || newY >= boardHeight )
                return;
                
            Cell target = board[ x + dx[ i ], y + dy[ i ] ];
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
            return null;
        int idx = emptyCells[ Random.Range( 0, emptyCells.Count ) ];
        return cells[ idx ];
    }
}