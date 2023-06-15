using Godot;
using System;


public partial class DemoDrawTarget : Node3D
{
	[Export]
	public Vector3 ShapeParams;

	[Export]
	public Vector3 DeltaRotation;


	public override void _Process(double delta)
	{
		Rotation += DeltaRotation * (float)delta;
	}
}
