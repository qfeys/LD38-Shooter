﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Direction
{
	Right,
	Left,
	Top,
	Bottom,
	BottomLeft,
	BottomRight,
	TopLeft,
	TopRight,  
}

public class Grid : MonoBehaviour {
		
	public Vector2 Offset;

    public float _width;
    public float _height;
    public bool drawGrid = true;

	public int Width { get; private set; }
	public int Height { get; private set; }

    public Node[,] Nodes;
	
	public int Left { get { return 0; } }
	public int Right { get { return Width; } }
	public int Bottom { get { return 0; } }
	public int Top { get { return Height; } }

	public const float UnitSize = 0.3f;

	private LineRenderer LineRenderer;
	GameObject Player;

	void Awake () 
	{	
		LineRenderer = transform.GetComponent<LineRenderer>();
        Node.drawGrid = drawGrid;
        Enemy.grid = this;

        //Get grid dimensions

        Width = (int)(_width / UnitSize * 2);
        Height = (int)(_height / UnitSize * 2);

        Nodes = new Node[Width, Height];

		//Initialize the grid nodes - 1 grid unit between each node
		//We render the grid in a diamond pattern
		for (int x = 0; x < Width/2; x++)
		{
			for (int y = 0; y < Height; y++)
			{
                float ptx = x * UnitSize;
                float pty = (-(y / 2) + (1 / 2f)) * UnitSize;
				int offsetx = 0;

				if (y % 2 == 0)
				{
                    ptx = (x + (1 / 2f)) * UnitSize;
					offsetx = 1;
				}	
				else
				{
                    pty = -(y / 2) * UnitSize;
				}

                Vector2 pos = new Vector2(ptx + Offset.x, pty + Offset.y);
				Node node = new Node(x*2 + offsetx, y, pos, this);
				Nodes[x*2 + offsetx, y] = node;
			}
		}
		
		//Create connections between each node
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				if (Nodes[x,y] == null) continue;				
				Nodes[x, y].InitializeConnections(this);
			}
		}		

		//Pass 1, we removed the bad nodes, based on valid connections
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				if (Nodes[x,y] == null) 
					continue;				

				Nodes[x, y].CheckConnectionsPass1 (this);
			}
		}		

		//Pass 2, remove bad connections based on bad nodes
		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
			{
				if (Nodes[x,y] == null) 
					continue;				

				Nodes[x, y].CheckConnectionsPass2 ();
                if (drawGrid)
                    Nodes[x, y].DrawConnections();	//debug
			}
		}		
	}		
	

	public Point WorldToGrid(Vector2 worldPosition)
	{
        Vector2 gridPosition = new Vector2(((worldPosition.x - Offset.x) * 2f) / UnitSize, (-((worldPosition.y - Offset.y) * 2f) + 1) / UnitSize);

		//adjust to our nearest integer
		float rx = gridPosition.x % 1;
		if (rx < 0.5f)
			gridPosition.x = gridPosition.x - rx;
		else
			gridPosition.x = gridPosition.x + (1 - rx);
		
		float ry = gridPosition.y % 1;
		if (ry < 0.5f)
			gridPosition.y = gridPosition.y - ry;
		else
			gridPosition.y = gridPosition.y + (1 - ry);
				
		int x = (int)gridPosition.x;
		int y = (int)gridPosition.y;

        //if (x < 0 || y < 0 || x >= Width || y >= Height)
        //    throw new IndexOutOfRangeException("Pos not in grid");

        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x >= Width) x = Width - 1;
        if (y >= Height) y = Height - 1;

        Node node = Nodes [x, y];
		//We calculated a spot between nodes'
		//Find nearest neighbor
		if((node == null) ||  (x % 2 == 0 && y % 2 == 0) || (gridPosition.y % 2 == 1 && gridPosition.x % 2 == 1))
		{   
			float mag = 100;

            try
            {
                if (x < Width - 1 && Nodes[x + 1, y] != null && !Nodes[x + 1, y].BadNode)
                {
                    float mag1 = (Nodes[x + 1, y].Position - worldPosition).magnitude;
                    if (mag1 < mag)
                    {
                        mag = mag1;
                        node = Nodes[x + 1, y];
                    }
                }
                if (y < Height - 1 && Nodes[x, y + 1] != null && !Nodes[x, y + 1].BadNode)
                {
                    float mag1 = (Nodes[x, y + 1].Position - worldPosition).magnitude;
                    if (mag1 < mag)
                    {
                        mag = mag1;
                        node = Nodes[x, y + 1];
                    }
                }
                if (x > 0 && Nodes[x - 1, y] != null && !Nodes[x - 1, y].BadNode)
                {
                    float mag1 = (Nodes[x - 1, y].Position - worldPosition).magnitude;
                    if (mag1 < mag)
                    {
                        mag = mag1;
                        node = Nodes[x - 1, y];
                    }
                }
                if (y > 0 && Nodes[x, y - 1] != null && !Nodes[x, y - 1].BadNode)
                {
                    float mag1 = (Nodes[x, y - 1].Position - worldPosition).magnitude;
                    if (mag1 < mag)
                    {
                        mag = mag1;
                        node = Nodes[x, y - 1];
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Worldpos : " + worldPosition + ", X: " + x + " , Y: " + y);
                Debug.Log("Node: " + Nodes[x, y] + "Nodes[x-1,y]" + Nodes[x - 1, y]);
                throw e;
            }
		}
        if (node != null)
            return new Point(node.X, node.Y);
        else
            return new Point(0, 0);
	}

	public Vector2 GridToWorld(Point gridPosition)
	{
		Vector2 world = new Vector2(gridPosition.X / 2f * UnitSize + Offset.x, -(gridPosition.Y / 2f - 0.5f)* UnitSize + Offset.y);

		return world;
	}
	
	public bool ConnectionIsValid(Point point1, Point point2)
	{
		//comparing same point, return false
		if (point1.X == point2.X && point1.Y == point2.Y)
			return false;
		
		if (Nodes [point1.X, point1.Y] == null)
			return false;
		
		//determine direction from point1 to point2
		Direction direction = Direction.Bottom;

		if (point1.X == point2.X)
		{
			if (point1.Y < point2.Y)
				direction = Direction.Bottom;
			else if (point1.Y > point2.Y)
				direction = Direction.Top;
		}
		else if (point1.Y == point2.Y)
		{
			if (point1.X < point2.X)
				direction = Direction.Right;
			else if (point1.X > point2.X)
				direction = Direction.Left;
		}
		else if (point1.X < point2.X)
		{
			if (point1.Y > point2.Y)
				direction = Direction.TopRight;
			else if (point1.Y < point2.Y)
				direction = Direction.BottomRight;
		}
		else if (point1.X > point2.X)
		{
			if (point1.Y > point2.Y)
				direction = Direction.TopLeft;
			else if (point1.Y < point2.Y)
				direction = Direction.BottomLeft;
		}

		//check connection
		switch (direction)
		{
			case Direction.Bottom:
			if (Nodes[point1.X, point1.Y].Bottom != null)
				return Nodes[point1.X, point1.Y].Bottom.Valid;
			else
				return false;

			case Direction.Top:
			if (Nodes[point1.X, point1.Y].Top != null)
				return Nodes[point1.X, point1.Y].Top.Valid;
			else
				return false;
		
			case Direction.Right:
			if (Nodes[point1.X, point1.Y].Right != null)
				return Nodes[point1.X, point1.Y].Right.Valid;
			else
				return false;

			case Direction.Left:
			if (Nodes[point1.X, point1.Y].Left != null)
				return Nodes[point1.X, point1.Y].Left.Valid;
			else
				return false;
		
			case Direction.BottomLeft:
			if (Nodes[point1.X, point1.Y].BottomLeft != null)
				return Nodes[point1.X, point1.Y].BottomLeft.Valid;
			else
				return false;

			case Direction.BottomRight:
			if (Nodes[point1.X, point1.Y].BottomRight != null)
				return Nodes[point1.X, point1.Y].BottomRight.Valid;
			else
				return false;
		
			case Direction.TopLeft:
			if (Nodes[point1.X, point1.Y].TopLeft != null)
				return Nodes[point1.X, point1.Y].TopLeft.Valid;
			else
				return false;
		
			case Direction.TopRight:
			if (Nodes[point1.X, point1.Y].TopRight != null)
				return Nodes[point1.X, point1.Y].TopRight.Valid;
			else
				return false;
		
			default:
				return false;
		}		
	}


	void Update()
	{
		//Pathfinding demo
		if(drawGrid && false)
        {
            if (Player == null)
            {
                Player = GameObject.Find("Player");
                Player.GetComponent<Enemy>().GO();
            }
            //Convert mouse click point to grid coordinates
            Point gridPos = WorldToGrid(Enemy.goal.transform.position);

			if (gridPos != null) {						
				
				if (gridPos.X > 0 && gridPos.Y > 0 && gridPos.X < Width && gridPos.Y < Height) {

					//Convert player point to grid coordinates
					Point playerPos = WorldToGrid (Player.transform.position);					
					Nodes[playerPos.X, playerPos.Y].SetColor(Color.blue);

					//Find path from player to clicked position
					BreadCrumb bc = PathFinder.FindPath (this, playerPos, gridPos);

					int count = 0;		
					LineRenderer lr = Player.GetComponent<LineRenderer> ();
					lr.SetVertexCount(100);  //Need a higher number than 2, or crashes out
					lr.SetWidth(0.1f, 0.1f);
					lr.SetColors(Color.yellow, Color.yellow);

					//Draw out our path
					while (bc != null) {					
						lr.SetPosition(count, GridToWorld(bc.position));
						bc = bc.next;
						count += 1;
					}
					lr.SetVertexCount(count);					
				}				
			}
		}
	}

}



