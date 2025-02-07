using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

public class BoardManager : MonoBehaviour
{
    private enum Direction { Up, Down, Left, Right }

    public int boardWidth;
    public int boardHeight;
    public List<Cell> cells;
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
        for( int i = 0; i < cells.Count; i++ )
            cells[ i ].idx = i;

        int totalCells = cells.Count;
        int[ ] randomIndices = Enumerable.Range( 0, totalCells ).OrderBy( n => Random.value ).Take( 10 ).ToArray( );

        HashSet<int> usedRows = new HashSet<int>( );
        HashSet<int> usedCols = new HashSet<int>( );

        foreach( int index in randomIndices )
        {
            int row = index / boardWidth;
            int col = index % boardWidth;

            if( !usedRows.Contains( row ) && !usedCols.Contains( col ) )
            {
                State randomState = ( State ) Random.Range( 1, 3 );
                cells[ index ].SetState( randomState );

                usedRows.Add( row );
                usedCols.Add( col );
            }
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

    public void ClickCell( int target )
    {   
        Cell newRandomCell = GetRandomEmptyCell( );
        newRandomCell.SetState( ( State ) Random.Range( 1, 3 ) );
        ChangeNextCell( );
        UpdateBoard( target );
    }

    private void UpdateBoard( int center )
    {
        State centerState = cells[ center ].state;
        foreach( KeyValuePair<int, Direction> value in GetAdjacentIndex( center ) )
        {
            if( cells[ value.Key ].state != centerState && cells[ value.Key ].state != State.X )
            {
                cells[ value.Key ].SetState( State.X );
                GameManager.instance.AddScore( 1 );
            }
            else if( cells[ value.Key ].state == centerState && cells[ value.Key ].state != State.X )
            {
                MoveCell( value.Key, value.Value );
            }
        }
    }

    private void MoveCell(int target, Direction direction)
    {
        int row = target / boardWidth;
        int col = target % boardHeight;
        int newTarget = target;

        switch (direction)
        {
            case Direction.Up:
                newTarget = target - boardWidth;
                break;
            case Direction.Down:
                newTarget = target + boardWidth;
                break;
            case Direction.Left:
                if ( col == 0 ) return;
                newTarget = target - 1;
                break;
            case Direction.Right:
                if( col == boardWidth - 1 ) return;
                newTarget = target + 1;
                break;
            default:
                return;
        }

        if ( newTarget >= 0 && newTarget < cells.Count && cells[ newTarget ].state == State.X )
        {
            cells[ target ].MoveCell( cells[ target ], cells[ newTarget ] );
            // cells[ newTarget ].SetState( cells[ target ].state );
            // cells[ target ].SetState( State.X );
            UpdateBoard( newTarget );
        }
    }


    private Dictionary<int, Direction> GetAdjacentIndex( int target )
    {
        Dictionary<int, Direction> adjacentIndices = new Dictionary<int, Direction>( );
        int row = target / boardWidth;
        int col = target % boardHeight;

        if( row > 0 ) adjacentIndices.Add( target - boardWidth, Direction.Up );
        if( row < boardWidth - 1 ) adjacentIndices.Add( target + boardWidth, Direction.Down );
        if( col > 0 ) adjacentIndices.Add( target - 1, Direction.Left );
        if( col < boardHeight - 1 ) adjacentIndices.Add( target + 1, Direction.Right );

        return adjacentIndices;
    }

    private Cell GetRandomEmptyCell( )
    {
        List<int> emptyCells = new List<int>( );
        foreach( Cell cell in cells )
            if( cell.state == State.X )
                emptyCells.Add( cell.idx );

        int idx = emptyCells[ Random.Range( 0, emptyCells.Count ) ];
        return cells[ idx ];
    }
}
