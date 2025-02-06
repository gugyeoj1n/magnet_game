using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class BoardManager : MonoBehaviour
{
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

    public void ChangeNextCell( )
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
}
