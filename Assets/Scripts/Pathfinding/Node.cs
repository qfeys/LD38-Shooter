using UnityEngine;
using System.Collections;
using System.Linq;

public class Node {

	public bool BadNode;    

	//Grid coordinates
	public int X;
	public int Y;

	//world position
	public Vector2 Position;

	//our 8 connection points
	public NodeConnection Top;
	public NodeConnection Left;
	public NodeConnection Bottom;
	public NodeConnection Right;
	public NodeConnection TopLeft;
	public NodeConnection TopRight;
	public NodeConnection BottomLeft;
	public NodeConnection BottomRight;

    public static bool drawGrid = true;
	GameObject DebugNode;
	
	public Node(int x, int y, Vector2 position, Grid grid)
	{
		Initialize(x, y, position, grid);
	}
	
	public void Initialize(int x, int y, Vector2 position, Grid grid)
	{
		X = x; Y = y;
		
		Position = position;

        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(position));
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);
        if (hits.Any(h => h.rigidbody != null))
        {
            BadNode = true;
        }

        //Draw Node on screen for debugging purposes
        if (drawGrid)
        {
            DebugNode = GameObject.Instantiate(Resources.Load("Node")) as GameObject;
            DebugNode.transform.position = Position;
            DebugNode.GetComponent<DebugNode>().X = X;
            DebugNode.GetComponent<DebugNode>().Y = Y;
        }
    }

	public void SetColor(Color color)
	{
        if(drawGrid)
		    DebugNode.transform.GetComponent<SpriteRenderer> ().color = color;
	}

	//Cull nodes if they don't have enough valid connection points (3)
	public void CheckConnectionsPass1(Grid grid)
	{
		if (!BadNode) {

			int clearCount = 0;

			if (Top != null && Top.Valid)
				clearCount++;
			if (Bottom != null && Bottom.Valid)
				clearCount++;
			if (Left != null && Left.Valid)
				clearCount++;
			if (Right != null && Right.Valid)
				clearCount++;
			if (TopLeft != null && TopLeft.Valid)
				clearCount++;
			if (TopRight != null && TopRight.Valid)
				clearCount++;
			if (BottomLeft != null && BottomLeft.Valid)
				clearCount++;
			if (BottomRight != null && BottomRight.Valid)
				clearCount++;

			//If not at least 3 valid connection points - disable node
			if (clearCount < 3) {
				BadNode = true;
				DisableConnections ();
			}
		}		
		
		if (!BadNode)
			SetColor (Color.yellow);
		else
			SetColor (Color.red);
	}

	//Remove connections that connect to bad nodes
	public void CheckConnectionsPass2()
	{
		if (Top != null && Top.Node != null && Top.Node.BadNode)
			Top.Valid = false;
		if (Bottom != null && Bottom.Node != null && Bottom.Node.BadNode)
			Bottom.Valid = false;
		if (Left != null && Left.Node != null && Left.Node.BadNode)
			Left.Valid = false;
		if (Right != null && Right.Node != null && Right.Node.BadNode)
			Right.Valid = false;
		if (TopLeft != null && TopLeft.Node != null && TopLeft.Node.BadNode)
			TopLeft.Valid = false;
		if (TopRight != null && TopRight.Node != null && TopRight.Node.BadNode)
			TopRight.Valid = false;
		if (BottomLeft != null && BottomLeft.Node != null && BottomLeft.Node.BadNode)
			BottomLeft.Valid = false;
		if (BottomRight != null && BottomRight.Node != null && BottomRight.Node.BadNode)
			BottomRight.Valid = false;
	}

	//Disable all connections going from this this
	public void DisableConnections()
	{
		if (Top != null) {
			Top.Valid = false;
		}
		if (Bottom != null) {
			Bottom.Valid = false;
		}
		if (Left != null) {
			Left.Valid = false;
		}
		if (Right != null) {
			Right.Valid = false;
		}
		if (BottomLeft != null) {
			BottomLeft.Valid = false;
		}
		if (BottomRight != null) {
			BottomRight.Valid = false;
		}
		if (TopRight != null) {
			TopRight.Valid = false;
		}
		if (TopLeft != null) {
			TopLeft.Valid = false;
		}
	}

	//debug draw for connection lines
	public void DrawConnections()
	{
		if (Top != null) Top.DrawLine ();
		if (Bottom != null)Bottom.DrawLine ();
		if (Left != null)Left.DrawLine ();
		if (Right != null)Right.DrawLine ();
		if (BottomLeft != null)BottomLeft.DrawLine ();
		if (BottomRight != null)BottomRight.DrawLine ();
		if (TopRight != null)TopRight.DrawLine ();
		if (TopLeft != null)TopLeft.DrawLine ();
	}


	//Raycast in all 8 directions to determine valid routes
	public void InitializeConnections(Grid grid)
	{
		bool valid = true;
		float diagonalDistance = Mathf.Sqrt (Mathf.Pow (Grid.UnitSize/2f, 2) + Mathf.Pow (Grid.UnitSize/2f, 2));

		if (X > 1)
		{	
			//Left
			valid = true;
			if (grid.Nodes[X-2,Y].BadNode)
			{
				valid = false;
			}
			Left = new NodeConnection(this, grid.Nodes[X - 2, Y], valid);

			//TopLeft
			if (Y > 0)
			{
				valid = true;
				if (grid.Nodes[X - 1, Y - 1].BadNode)
				{
					valid = false;
				}
				TopLeft = new NodeConnection(this, grid.Nodes[X - 1, Y - 1], valid);
			}

			//BottomLeft
			if (Y < grid.Height - 1)
			{
				valid = true;
				if (grid.Nodes[X - 1, Y + 1].BadNode)
				{
					valid = false;
				}			
				BottomLeft = new NodeConnection(this,grid.Nodes[X - 1, Y + 1], valid);
			}
		}


		if (X < grid.Width - 2)
		{
            // Right
			valid = true;
			if (grid.Nodes[X + 2, Y].BadNode)
			{
				valid = false;
			}
			Right = new NodeConnection(this,grid.Nodes[X + 2, Y], valid);

			//TopRight
			if (Y > 0)
			{
				valid = true;
				if (grid.Nodes[X + 1, Y - 1].BadNode)
				{
					valid = false;
				}
				TopRight = new NodeConnection(this,grid.Nodes[X + 1, Y - 1], valid);
			}
			
			//BottomRight
			if (Y < grid.Height - 1)
			{
				valid = true;
				if (grid.Nodes[X + 1, Y + 1].BadNode)
				{
					valid = false;
				}
				BottomRight = new NodeConnection(this,grid.Nodes[X + 1, Y + 1], valid);
			}

		}
	
		if (Y - 1 > 0)
		{
            // Top
			valid = true;
			if (grid.Nodes[X, Y - 2].BadNode)
			{
				valid = false;
			}			
			Top = new NodeConnection(this,grid.Nodes[X, Y - 2], valid);
		}


		if (Y < grid.Height - 2)
		{
            // Bottom
			valid = true;
			if (grid.Nodes[X, Y + 2].BadNode)
			{
				valid = false;
			}						
			Bottom = new NodeConnection(this,grid.Nodes[X, Y + 2], valid);
		}
	}


}

