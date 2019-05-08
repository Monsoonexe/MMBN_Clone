﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelArray : MonoBehaviour {
    private static int boardRowsCount = 3;
    private static int boardColumnsCount = 6;
    
    //game and other things should reference panels with a Cartesian Plane in mind.  Panel 0,0 is on the bottom row on the farthest left side.
    //internally, the list is more natural: the panels belong to a row. access a panel through the row.
    
    [SerializeField] private Panel[] panelRow2 = new Panel[6]; //these are set in the inspector
    [SerializeField] private Panel[] panelRow1 = new Panel[6]; //these are set in the inspector
    [SerializeField] private Panel[] panelRow0 = new Panel[6]; //these are set in the inspector

    private Panel[,] boardArray;//a list of individual panels kept in (x, y) format

    void Awake()
    {
        InitializeBoardArrayWithPanels();// set panel references into a list that can be accessed via cartesian points (x, y)
    }

	// Use this for initialization
	void Start () {
        

	}//end Start()
	
	// Update is called once per frame
	void Update () {
		
	}//end Update()

    public void InitializeBoardArrayWithPanels(){
        //fills boardArray with Panels that can be referenced via Cartesian Points (x,y)
        boardArray = new Panel[boardColumnsCount, boardRowsCount]; // 6 X 3
        for (int column = 0; column < boardColumnsCount; ++column)// x,y coordinates
        {
            boardArray[column, 2] = panelRow2[column];//top row
            boardArray[column, 1] = panelRow1[column];//middle row
            boardArray[column, 0] = panelRow0[column];//bottom row
        }//end for
    }//end InitializeBoardArrayWithPanels()

    public Panel GetPanel(int x, int y){
        //find which Panel is at coordinates (x, y)
        if (x <= boardColumnsCount && x >= 0 && y <= boardRowsCount && y >= 0)//if x and y are valid points on a 6,3 sized plane
        {        
            return boardArray[x, y];
        }
        else
        {
            Debug.Log("ERROR: Request Panel is out of range!");
            return null;
        }
    }//end GetPanel

    public Panel GetOccupantsPanel(GameObject panelOccupant)
    {
        Panel occupiedPanel = null;//this is the panel that will be returned if it is occupied by this GO
        for (int column = 0; column < boardColumnsCount; ++column)//iterate through all Panels in PanelArray
        {
            for (int row = 0; row < boardRowsCount; ++row)
            {
                if (boardArray[column, row].GetOccupant() == panelOccupant)//if panel being searched for matches one
                {
                    if(occupiedPanel == null)
                    {
                        occupiedPanel = boardArray[column, row];
                    }
                    else
                    {
                        Debug.Log("ERROR: This GO occupies multiple panels! " + panelOccupant.name);
                    }
                }//end if 
            }//end for rows
        }//end for columns

        if(occupiedPanel == null)
        {
            Debug.Log("ERROR: panel not found -- this object does not belong to a panel!: " + panelOccupant.name);
        }

        return occupiedPanel; //did not find a panel
    }//which panel is this guy on?

    public void GetPanelCoordinates(Panel panel, ref int x, ref int y)
        //if you have a panel, but need to find its coordinates
    {
        for (int column = 0; column < boardColumnsCount; ++column)//iterate through all Panels in PanelArray
        {
            for (int row = 0; row < boardRowsCount; ++row)
            {
                if (boardArray[column, row] == panel)//if panel being searched for matches one
                {
                    x = column;
                    y = row;
                    return;
                }//end if 
            }//end for rows
        }//end for columns
        //Debug.Log("ERROR: Panel could not be found! Fuck all! Panel name: " + panel.name);

    }//end GetPanelCoordinates()

    public static int GetBoardColumnsCount()
    {
        return boardColumnsCount;
    }//how many columns is this array?

    public static int GetBoardRowsCount()
    {
        return boardRowsCount;
    }//how many rows in this array?

    public Panel[] GetRowOfPanels(int rowNum)
    {
        Panel[] rowOfPanels = new Panel[boardColumnsCount];
        for(int iPanel = 0; iPanel < boardColumnsCount; ++iPanel)
        {
            rowOfPanels[iPanel] = boardArray[iPanel, rowNum];
        }
        return rowOfPanels;
    }//gimme this whole row

    public Panel[] GetColumnOfPanels(int colNum)
    {
        Panel[] colOfPanels = new Panel[boardRowsCount];
        for (int iPanel = 0; iPanel < boardRowsCount; ++iPanel)
        {
            colOfPanels[iPanel] = boardArray[colNum, iPanel];
        }
        return colOfPanels;
    }//gimme this whole column

}
