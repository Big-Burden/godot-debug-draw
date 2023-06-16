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
	private DemoDrawTarget _capsule;

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
		
		Color col = Colors.Red;
		col.H = i / Mathf.Tau;


		//Queries interfere with profiling memory
		if ((DebugDraw.GetEnabledLayers() & 1 << 4) != 0)
		{
			GC.Dictionary result = GetWorld3D().DirectSpaceState.IntersectRay(_rayQuery1);
			DebugDraw.RayIntersect(_rayQuery1, result, 0.0f, null, null, 1 << 4);
		
			if (result.Count > 0)
			{
				DebugDraw.TextKeyed("hit1", result["position"], 0.0f, null, 1 << 4);
			}
		
			result = GetWorld3D().DirectSpaceState.IntersectRay(_rayQuery2);
			if (result.Count > 0)
			{
				DebugDraw.RayIntersect(_rayQuery2.From, _rayQuery2.To, (Vector3)result["position"],
					0.0f, Colors.Yellow, Colors.Blue, 1 << 4);
		
				DebugDraw.TextKeyed("hit2", result["position"], 0.0f, Colors.Purple, 1 << 4);
			}
		
			result = GetWorld3D().DirectSpaceState.IntersectRay(_rayQuery3);
			DebugDraw.RayIntersect(_rayQuery3, result, 0.0f, null, null, 1 << 4);
		
		
			float[] motionResult = GetWorld3D().DirectSpaceState.CastMotion(_shapeQueryMotion);
			DebugDraw.ShapeMotion(_shapeQueryMotion, motionResult, 0.0f, null, null, 1 << 4);
		
			GC.Array<Vector3> hits = GetWorld3D().DirectSpaceState.CollideShape(_shapeQueryCollision, 
				32);
			DebugDraw.ShapeCollision(_shapeQueryCollision, hits, 0.0f, null, null, 1 << 4);
			DebugDraw.TextKeyed("hit3", result.Count > 0, 0.0f, result.Count > 0 ? Colors.Green :
				Colors.Red, 1 << 4);
		}

		DebugDraw.Text("A temp string, I will overflow! " + Engine.GetProcessFrames(), 0.3f, col);
		DebugDraw.Text3D("Wow, look at all these shapes!", Vector3.Up * 4, 0.0f, col);

		DebugDraw.Box(_cube.GlobalTransform, _cube.ShapeParams, Colors.Green, 0.0f, false, 1 << 2);
		DebugDraw.Box(_cube.GlobalPosition + Vector3.Right * 2.0f, 
			_cube.Basis.GetRotationQuaternion(), _cube.ShapeParams,
			new Color(Colors.Green, 0.5f), 0.0f, true, 1 << 3);

		DebugDraw.Cylinder(_cylinder.GlobalTransform, _cylinder.ShapeParams.X, 
			_cylinder.ShapeParams.Y, Colors.Purple, 0.0f, false, 1 << 2);
		DebugDraw.Cylinder(_cylinder.GlobalPosition + Vector3.Right * 2.0f,
			_cylinder.Basis.GetRotationQuaternion(), _cylinder.ShapeParams.X,
			_cylinder.ShapeParams.Y, new Color(Colors.Purple, 0.5f), 0.0f,
			true, 1 << 3);

		DebugDraw.Capsule(_capsule.GlobalTransform, _capsule.ShapeParams.X, _capsule.ShapeParams.Y,
			Colors.Cyan, 0.0f, false, 1 << 2);

		DebugDraw.Capsule(_capsule.GlobalPosition + Vector3.Right * 2.0f,
			_capsule.Basis.GetRotationQuaternion(), _capsule.ShapeParams.X, _capsule.ShapeParams.Y,
			new Color(Colors.Cyan, 0.5f), 0.0f, true, 1 << 3);

		DebugDraw.Sphere(_sphere.GlobalTransform, _sphere.ShapeParams.X,
			Colors.OrangeRed, 0.0f, false, 1<< 2);
		DebugDraw.Sphere(_sphere.GlobalPosition + Vector3.Right * 2.0f,
			_sphere.Basis.GetRotationQuaternion(), _sphere.ShapeParams.X,
			new Color(Colors.OrangeRed, 0.5f), 0.0f, true, 1 << 3);
		
		DebugDraw.Point(_point.GlobalTransform, _point.ShapeParams.X,
			Colors.Cyan, 0.0f, 1 << 2);

		DebugDraw.Quad(_quad.GlobalTransform, _point.ShapeParams.X,
			Colors.Red, 0.0f, 1 << 2);

		DebugDraw.Plane(_plane.GlobalTransform, _plane.ShapeParams.X,
			new Color(Colors.Yellow, 0.5f), 0.0f, 1<< 2);

		DebugDraw.Circle(_circle.GlobalTransform, _point.ShapeParams.X,
			Colors.Yellow, 0.0f, 1 << 2);

		DebugDraw.Axes(_axes.GlobalTransform, _axes.ShapeParams.X, 0.0f, 1 << 2);

		DebugDraw.Arrow(_arrow.GlobalTransform.Origin, -_arrow.GlobalTransform.Basis.Z,
			_arrow.ShapeParams.X, Colors.Blue, 0.0f, 1 << 2);

		DebugDraw.Line(_line.GlobalPosition,
			_line.GlobalPosition + _line.GlobalTransform.Basis.Z * 1.0f, Colors.HotPink, 0.0f, 
			1 << 2);

		Vector3 s = _line.GlobalPosition + Vector3.Up * -2.0f;

		DebugDraw.Lines(new Vector3[]
		{
			s,
			s + -_line.GlobalTransform.Basis.Z,
			s + -_line.GlobalTransform.Basis.Z + Vector3.Up,
			s + -_line.GlobalTransform.Basis.Z + Vector3.Up + Vector3.Right
		}, Colors.LimeGreen, 0.0f, 1 << 2);

		if (i > Mathf.Tau)
		{
			i = 0;
		}
	}
}
