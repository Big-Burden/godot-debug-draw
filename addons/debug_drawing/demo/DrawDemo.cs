using Godot;
using GC = Godot.Collections;


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

	[Export]
	private CollisionShape3D _shapeMotionStart;

	[Export]
	private Node3D _shapeMotionEnd;

	[Export]
	private Node3D _shapeMotionBody;
	
	[Export]
	private Node3D _shapeCollisionBody;

	[Export]
	private CollisionShape3D _shapeCollision;
	
	private float i;

	private PhysicsRayQueryParameters3D _rayQuery1;
	private PhysicsRayQueryParameters3D _rayQuery2;
	private PhysicsRayQueryParameters3D _rayQuery3;
	private PhysicsShapeQueryParameters3D _shapeQueryMotion;
	private PhysicsShapeQueryParameters3D _shapeQueryCollision;


	public override void _Ready()
	{
		base._Ready();
		_rayQuery1 = PhysicsRayQueryParameters3D.Create(_rayStart.GlobalPosition,
			_rayEnd.GlobalPosition);
		_rayQuery2 = PhysicsRayQueryParameters3D.Create(
			_rayStart.GlobalPosition + Vector3.Up * 0.5f,
			_rayEnd.GlobalPosition + Vector3.Up * 0.5f);
		_rayQuery3 = PhysicsRayQueryParameters3D.Create(
			_rayStart.GlobalPosition + Vector3.Up * -0.75f,
			_rayEnd.GlobalPosition + Vector3.Up * -0.75f);
		_shapeQueryMotion = new PhysicsShapeQueryParameters3D()
		{
			CollideWithAreas = false,
			CollideWithBodies = true,
			CollisionMask = 4294967295,
			Shape = _shapeMotionStart.Shape,
			Exclude = null,
			Margin = 0.04f,
			Motion = _shapeMotionEnd.GlobalPosition - _shapeMotionStart.GlobalPosition,
			Transform = _shapeMotionStart.Transform
		};
		
		_shapeQueryCollision = new PhysicsShapeQueryParameters3D()
		{
			CollideWithAreas = false,
			CollideWithBodies = true,
			CollisionMask = 4294967295,
			Shape = _shapeCollision.Shape,
			Exclude = null,
			Margin = 0.04f,
			Transform = _shapeCollision.Transform
		};
	}


	public override void _Process(double delta)
	{
		base._Process(delta);
		i += (float)delta;

		_rayCollisionBody.RotateX((float)delta);
		_shapeMotionBody.RotateY((float)delta);
		_shapeCollisionBody.RotateY((float)delta);
		
		GC.Dictionary result = GetWorld3D().DirectSpaceState.IntersectRay(_rayQuery1);
		DebugDraw.RayIntersect(_rayQuery1, result);

		if (result.Count > 0)
		{
			DebugDraw.Text("hit1", result["position"]);
		}

		result = GetWorld3D().DirectSpaceState.IntersectRay(_rayQuery2);
		DebugDraw.RayIntersect(_rayQuery2.From, _rayQuery2.To, (Vector3)result["position"],
			0.0f, Colors.Blue, Colors.Yellow);

		if (result.Count > 0)
		{
			DebugDraw.Text("hit2", result["position"], 0.0f, Colors.Purple);
		}

		result = GetWorld3D().DirectSpaceState.IntersectRay(_rayQuery3);
		DebugDraw.RayIntersect(_rayQuery3, result);


		float[] motionResult = GetWorld3D().DirectSpaceState.CastMotion(_shapeQueryMotion);
		DebugDraw.ShapeMotion(_shapeQueryMotion, motionResult);

		GC.Array<Vector3> hits = GetWorld3D().DirectSpaceState.CollideShape(_shapeQueryCollision, 32);
		DebugDraw.ShapeCollision(_shapeQueryCollision, hits);

		Color col = Colors.Red;
		col.H = i / Mathf.Tau;

		DebugDraw.Text("hit3", result.Count > 0);
		DebugDraw.TempText("A temp string, I will overflow!", 0.5f, col);


		DebugDraw.TempText3D("Wow, look at all these shapes!", Vector3.Up * 5, 0.0f,
			col);

		DebugDraw.Box(_cube.GlobalTransform, _cube.ShapeParams, 0.0f, Colors.Green);
		DebugDraw.Box(_cube.GlobalPosition + Vector3.Right * 2.0f,
			_cube.Basis.GetRotationQuaternion(), _cube.ShapeParams, 0.0f,
			new Color(Colors.Green, 0.5f), true, DebugLayers.Layer2);

		DebugDraw.Cylinder(_cylinder.GlobalTransform, _cylinder.ShapeParams.X,
			_cylinder.ShapeParams.Y, 0.0f, Colors.Purple);
		DebugDraw.Cylinder(_cylinder.GlobalPosition + Vector3.Right * 2.0f,
			_cylinder.Basis.GetRotationQuaternion(), _cylinder.ShapeParams.X,
			_cylinder.ShapeParams.Y, 0.0f, new Color(Colors.Purple, 0.5f), true,
			DebugLayers.Layer2);

		DebugDraw.Sphere(_sphere.GlobalTransform, _sphere.ShapeParams.X, 0.0f,
			Colors.OrangeRed);
		DebugDraw.Sphere(_sphere.GlobalPosition + Vector3.Right * 2.0f,
			_sphere.Basis.GetRotationQuaternion(), _sphere.ShapeParams.X, 0.0f,
			new Color(Colors.OrangeRed, 0.5f), true, DebugLayers.Layer2);


		DebugDraw.Point(_point.GlobalTransform, _point.ShapeParams.X, 0.0f,
			Colors.Cyan);

		DebugDraw.Quad(_quad.GlobalTransform, _point.ShapeParams.X, 0.0f,
			Colors.Red);

		DebugDraw.Plane(_plane.GlobalTransform, _plane.ShapeParams.X, 0.0f,
			new Color(Colors.Yellow, 0.5f));

		DebugDraw.Circle(_circle.GlobalTransform, _point.ShapeParams.X, 0.0f,
			Colors.Yellow);

		DebugDraw.Axes(_axes.GlobalTransform, _axes.ShapeParams.X);

		DebugDraw.Arrow(_arrow.GlobalTransform.Origin, -_arrow.GlobalTransform.Basis.Z,
			_arrow.ShapeParams.X, 0.0f, Colors.Blue);

		DebugDraw.Line(_line.GlobalPosition,
			_line.GlobalPosition + _line.GlobalTransform.Basis.Z * 1.0f, 0.0f, Colors.HotPink);

		if (i > Mathf.Tau)
		{
			i = 0;
		}
	}
}
