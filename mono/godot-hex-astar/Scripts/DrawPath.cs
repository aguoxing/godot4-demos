using Godot;
using System;
using System.Collections.Generic;

public partial class DrawPath : Node2D
{
	private List<Vector2> _pathList = new List<Vector2>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void UpdatePath(List<Vector2> pathList)
	{
		_pathList.Clear();
		_pathList = pathList;
		QueueRedraw();
	}

	public override void _Draw()
	{
		for (int i = 0; i < _pathList.Count; i++)
		{
			Vector2 pos = new Vector2(_pathList[i].X, _pathList[i].Y);
			if (i != _pathList.Count - 1)
			{
				Vector2 nextPos = new Vector2(_pathList[i + 1].X, _pathList[i + 1].Y);
				DrawLine(pos, nextPos, Colors.Red, 2.0f);
			}
		}
	}
}
