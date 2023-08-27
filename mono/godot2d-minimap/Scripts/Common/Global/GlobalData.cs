using System;
using System.Collections.Generic;
using Godot;

public class GlobalData
{
    private static readonly Lazy<GlobalData> _instance = new(() => new GlobalData());

    public static GlobalData Instance
    {
        get { return _instance.Value; }
    }

    public Vector2 PlayerPosition;
    public List<Position> IsaacMapData;
    public Vector2 CenterPos;
    public string NoiseMapSeed;
}