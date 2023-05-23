using Godot;


public partial class DrawDemo : Node3D
{
	[Export]
	private DemoDrawTarget _cube;

	[Export]
	private DemoDrawTarget _cylinder;

	[Export]
	private DemoDrawTarget _sphere;

	[Export]
	private DemoDrawTarget _point;
	
	[Export]
	private DemoDrawTarget _quad;
	
	[Export]
	private DemoDrawTarget _plane;
	
	[Export]
	private DemoDrawTarget _circle;

	[Export]
	private DemoDrawTarget _axes;
	
	[Export]
	private DemoDrawTarget _arrow;
	
	[Export()]
	private DemoDrawTarget _line;
	
	[Export]
	private Node3D _rayStart;

	[Export]
	private Node3D _rayEnd;

	[Export]
	private Node3D _rayCollisionBody;
	

	private float i;

	private PhysicsRayQueryParameters3D _query1;
	private PhysicsRayQueryParameters3D _query2;
	private PhysicsRayQueryParameters3D _query3;

	public override void _Ready()
	{
		base._Ready();
		_query1 = PhysicsRayQueryParameters3D.Create(_rayStart.GlobalPosition, 
			_rayEnd.GlobalPosition);
		_query2 = PhysicsRayQueryParameters3D.Create(_rayStart.GlobalPosition + Vector3.Up * 0.5f,
			_rayEnd.GlobalPosition + Vector3.Up * 0.5f);
		_query3 = PhysicsRayQueryParameters3D.Create(_rayStart.GlobalPosition + Vector3.Up * -0.75f,
			_rayEnd.GlobalPosition + Vector3.Up * -0.75f);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		i += (float)delta;
		
		_rayCollisionBody.RotateX((float)delta);
		var result = GetWorld3D().DirectSpaceState.IntersectRay(_query1);
		DebugDraw.DrawRay(_query1, result);

		if (result.Count > 0)
		{
			DebugDraw.DrawText("hit1", result["position"]);
		}
		
		result = GetWorld3D().DirectSpaceState.IntersectRay(_query2);
		DebugDraw.DrawRay(_query2.From, _query2.To, (Vector3)result["position"],
			0.0f, Colors.Blue, Colors.Yellow);

		if (result.Count > 0)
		{
			DebugDraw.DrawText("hit2", result["position"], 0.0f, Colors.Purple);
		}
		
		result = GetWorld3D().DirectSpaceState.IntersectRay(_query3);
		DebugDraw.DrawRay(_query3, result);

		Color col = Colors.Red;
		col.H = i/Mathf.Tau;
		
		DebugDraw.DrawText("hit3", result.Count > 0);
		DebugDraw.DrawTempText("Haha a temp string, I will overflow!", 0.5f, col);

		
		DebugDraw.DrawText3D("world",  "Wow, look at all these shapes!", Vector3.Up * 5, 0.0f, 
			col);

		DebugDraw.DrawCube(_cube.GlobalTransform, _cube.ShapeParams, 0.0f, Colors.Green);
		DebugDraw.DrawCube(_cube.GlobalPosition + Vector3.Right * 2.0f,
			_cube.Basis.GetRotationQuaternion(), _cube.ShapeParams, 0.0f, 
			new Color(Colors.Green, 0.5f), true, DebugLayers.Layer2);
		
		DebugDraw.DrawCylinder(_cylinder.GlobalTransform, _cylinder.ShapeParams.X, 
			_cylinder.ShapeParams.Y, 0.0f, Colors.Purple);
		DebugDraw.DrawCylinder(_cylinder.GlobalPosition + Vector3.Right * 2.0f, 
			_cylinder.Basis.GetRotationQuaternion(), _cylinder.ShapeParams.X, 
			_cylinder.ShapeParams.Y, 0.0f, new Color(Colors.Purple, 0.5f), true, DebugLayers.Layer2);
		
		DebugDraw.DrawSphere(_sphere.GlobalTransform, _sphere.ShapeParams.X, 0.0f, 
			Colors.OrangeRed);
		DebugDraw.DrawSphere(_sphere.GlobalPosition + Vector3.Right * 2.0f, 
			_sphere.Basis.GetRotationQuaternion(), _sphere.ShapeParams.X, 0.0f, 
			new Color(Colors.OrangeRed, 0.5f), true, DebugLayers.Layer2);
		
		
		DebugDraw.DrawPoint(_point.GlobalTransform, _point.ShapeParams.X, 0.0f,
			Colors.Cyan);
		
		DebugDraw.DrawQuad(_quad.GlobalTransform, _point.ShapeParams.X, 0.0f,
			Colors.Red);
		
		DebugDraw.DrawPlane(_plane.GlobalTransform, _plane.ShapeParams.X, 0.0f,
			new Color(Colors.Yellow, 0.5f));

		DebugDraw.DrawCircle(_circle.GlobalTransform, _point.ShapeParams.X, 0.0f,
			Colors.Yellow);
		
		DebugDraw.DrawAxes(_axes.GlobalTransform, _axes.ShapeParams.X);
		
		DebugDraw.DrawArrow(_arrow.GlobalTransform.Origin, -_arrow.GlobalTransform.Basis.Z, 
			_arrow.ShapeParams.X, 0.0f, Colors.Blue);

		DebugDraw.DrawLine(_line.GlobalPosition, 
			_line.GlobalPosition + _line.GlobalTransform.Basis.Z * 1.0f, 0.0f, Colors.HotPink);
		
		if (i > Mathf.Tau)
		{
			i = 0;
		}
	}
}
