using Godot;
using System;

public partial class Minimap : Control
{
	private SubViewport _subViewport;
	private SimpleWorld _simpleWorld;
	
	public override void _Ready()
	{
		_subViewport = GetNode<SubViewport>("SubViewportContainer/SubViewport");
	}

	public void Init()
	{
		if (_simpleWorld == null)
		{
			_simpleWorld = GD.Load<PackedScene>("res://MiniMap/SimpleWorld.tscn").Instantiate<SimpleWorld>();
			_subViewport.AddChild(_simpleWorld);
		}
		else
		{
			_simpleWorld.GeneratorMap();
		}
	}
}
