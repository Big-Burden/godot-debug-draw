using System;
using System.Collections.Generic;
using System.Linq;
using Burden.DebugDrawing;
using Godot;
using GC = Godot.Collections;

#if TOOLS

[Flags]
public enum DebugLayers : uint
{
	None = 0,
	Layer1 = 1 << 1,
	Layer2 = 1 << 2,
	Layer3 = 1 << 3,
	Layer4 = 1 << 4,
	Layer5 = 1 << 5,
	Layer6 = 1 << 6,
	Layer7 = 1 << 7,
	Layer8 = 1 << 8,
	All = Layer1 | Layer2 | Layer3 | Layer4 | Layer5 | Layer6 | Layer7 | Layer8
}

#endif //TOOLS

public partial class DebugDraw : Node
{
	#if TOOLS
	private static DebugMeshDrawer _meshDrawer;
	private static DebugCanvasDrawer _canvasDrawer;
	private DebugDock _dock;
	private CanvasLayer _drawCanvas;
	
	private static uint _enabledLayers = (uint)DebugLayers.All;
	public static Action OnDrawSettingsUpdated;
	
	private static bool _doDepthTest;
	private static int _maxPoolSize = 1024;
	private static int _startingPoolSize = 256;
	
	public DebugDraw()
	{
		Name = "DebugDraw";

		_maxPoolSize = (int)ProjectSettings.GetSetting(
			DebugDrawingPlugin.MaxPoolSizeOption, _maxPoolSize);
		_startingPoolSize = (int)ProjectSettings.GetSetting(
			DebugDrawingPlugin.StartingPoolSizeOption, _startingPoolSize);
	}


	public override void _Ready()
	{
		
		base._Ready();

		_drawCanvas = new CanvasLayer();
		AddChild(_drawCanvas);
		_drawCanvas.Owner = this;
		_drawCanvas.Layer = 100;

		_meshDrawer = new DebugMeshDrawer(this);
		_canvasDrawer = new DebugCanvasDrawer(_drawCanvas);

		if (!Engine.IsEditorHint())
		{
			_dock = GD.Load<PackedScene>("res://addons/debug_drawing/control/debug_dock.tscn")
				.Instantiate<DebugDock>();
			_drawCanvas.AddChild(_dock);
			_dock.Owner = _drawCanvas;
		}
	}


	public override void _ExitTree()
	{
		base._ExitTree();
		_canvasDrawer.Free();
		_canvasDrawer = null;
		_meshDrawer = null;
	}


	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		_meshDrawer.Update();
		_canvasDrawer.Update();
	}
	#endif //TOOLS

	
	public static void SetDrawingDepthTestEnabled(bool enabled)
	{
		#if TOOLS
		if (enabled != _doDepthTest)
		{
			_doDepthTest = enabled;
			_meshDrawer.SetDepthTestEnabled(_doDepthTest);
			OnDrawSettingsUpdated?.Invoke();
		}
		#endif //TOOLS
	}

	
	public static void SetEnabledLayers(uint layers)
	{
		#if TOOLS
		_enabledLayers = layers;
		OnDrawSettingsUpdated?.Invoke();
		#endif //TOOLS
	}

	
	public static void SetLayerEnabled(uint layer, bool enabled)
	{
		#if TOOLS
		if (enabled)
		{
			_enabledLayers |= layer;
		}
		else
		{
			_enabledLayers &= ~layer;
		}

		OnDrawSettingsUpdated?.Invoke();
		#endif //TOOLS
	}
	
	
	public static bool GetDoDepthTest()
	{
		#if TOOLS
		return _doDepthTest;
		#else
		return false;
		#endif //TOOLS
	}
	
	
	public static uint GetEnabledLayers()
	{
		#if TOOLS
		return _enabledLayers;
		#else
		return 0;
		#endif //TOOLS
	}
	
	
	public static int GetMaxPoolSize()
	{
		#if TOOLS
		return _maxPoolSize;
		#else
		return 0;
		#endif //TOOLS
	}
	
	
	public static int GetStartingPoolSize()
	{
		#if TOOLS
		return _startingPoolSize;
		#else
		return 0;
		#endif //TOOLS
	}
	
	
	public static Vector3I[] GetPoolSizes()
	{
		#if TOOLS
		Vector3I[] ret = new Vector3I[4];
		ret[0].X = _meshDrawer.MeshPool.CurrentSize;
		ret[0].Y = _meshDrawer.MeshPool.MaxSize;
		ret[0].Z = _meshDrawer.MeshPool.FreeObjects;

		ret[1].X = _meshDrawer.LinePool.CurrentSize;
		ret[1].Y = _meshDrawer.LinePool.MaxSize;
		ret[1].Z = _meshDrawer.LinePool.FreeObjects;

		ret[2].X = _canvasDrawer.TextPool.CurrentSize;
		ret[2].Y = _canvasDrawer.TextPool.MaxSize;
		ret[2].Z = _canvasDrawer.TextPool.FreeObjects;

		ret[3].X = _canvasDrawer.Text3DPool.CurrentSize;
		ret[3].Y = _canvasDrawer.Text3DPool.MaxSize;
		ret[3].Z = _canvasDrawer.Text3DPool.FreeObjects;

		return ret;
		#else
		return null;
		#endif //TOOLS
	}


	#region Drawing

	//Box
	public static void Box(Transform3D xform, Vector3 size,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(size);

		_meshDrawer?.DrawBox(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}

	
	public static void Box(Vector3 position, Quaternion rotation, Vector3 size,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform = new Transform3D(new Basis(rotation), position).ScaledLocal(size);

		_meshDrawer?.DrawBox(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Box(Vector3 position, Vector3 size,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform = new Transform3D(Basis.Identity, position).ScaledLocal(size);

		_meshDrawer?.DrawBox(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Box(Transform3D xform, BoxShape3D boxShape,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(boxShape.Size);

		_meshDrawer?.DrawBox(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Box(Vector3 position, Quaternion rotation, BoxShape3D boxShape,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(boxShape.Size);

		_meshDrawer?.DrawBox(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Box(Vector3 position, BoxShape3D boxShape, Color? color = null,
		float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform = new Transform3D(Basis.Identity, position).ScaledLocal(boxShape.Size);

		_meshDrawer?.DrawBox(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	//Cylinder
	
	public static void Cylinder(Transform3D xform, float height = 1.0f, float radius = 1.0f,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(new Vector3(radius, height, radius));

		_meshDrawer?.DrawCylinder(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Cylinder(Vector3 position, Quaternion rotation, float height = 1.0f,
		float radius = 1.0f, Color? color = null, float duration = 0.0f, bool drawSolid = false,
		uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(new Vector3(radius, height,
				radius));

		_meshDrawer?.DrawCylinder(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Cylinder(Vector3 position, float height = 1.0f, float radius = 1.0f,
		float duration = 0.0f, Color? color = null, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(new Vector3(radius, height,
				radius));

		_meshDrawer?.DrawCylinder(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Cylinder(Transform3D xform, CylinderShape3D cylinderShape,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(new Vector3(cylinderShape.Radius, cylinderShape.Height,
			cylinderShape.Radius));

		_meshDrawer?.DrawCylinder(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Cylinder(Vector3 position, Quaternion rotation,
		CylinderShape3D cylinderShape, Color? color = null, float duration = 0.0f,
		bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(new Vector3(
				cylinderShape.Radius, cylinderShape.Height, cylinderShape.Radius));

		_meshDrawer?.DrawCylinder(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Cylinder(Vector3 position, CylinderShape3D cylinderShape,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(new Vector3(
				cylinderShape.Radius, cylinderShape.Height, cylinderShape.Radius));

		_meshDrawer?.DrawCylinder(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	//Capsule
	
	public static void Capsule(Transform3D xform, float radius = 1.0f, float height = 1.0f,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		_meshDrawer.DrawCapsule(xform, radius, height, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Capsule(Vector3 position, Quaternion rotation, float radius = 1.0f,
		float height = 1.0f, Color? color = null, float duration = 0.0f, bool drawSolid = false,
		uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform = new(new Basis(rotation), position);

		_meshDrawer.DrawCapsule(xform, radius, height, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Capsule(Vector3 position, float radius = 1.0f, float height = 1.0f,
		float duration = 0.0f, Color? color = null, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform = new(Basis.Identity, position);

		_meshDrawer.DrawCapsule(xform, radius, height, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Capsule(Transform3D xform, CapsuleShape3D capsuleShape, Color? color = null,
		float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(new Vector3(capsuleShape.Radius * 0.5f, capsuleShape.Height,
			capsuleShape.Radius * 0.5f));

		_meshDrawer.DrawCapsule(xform, capsuleShape.Radius, capsuleShape.Height, duration, color,
			drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Capsule(Vector3 position, Quaternion rotation, CapsuleShape3D capsuleShape,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform = new(new Basis(rotation), position);

		_meshDrawer.DrawCapsule(xform, capsuleShape.Radius, capsuleShape.Height, duration, color,
			drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Capsule(Vector3 position, CapsuleShape3D capsuleShape, Color? color = null,
		float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform = new(Basis.Identity, position);

		_meshDrawer.DrawCapsule(xform, capsuleShape.Radius, capsuleShape.Height, duration, color,
			drawSolid, layers);
		#endif //TOOLS
	}


	//Sphere
	
	public static void Sphere(Transform3D xform, float radius = 1.0f, Color? color = null,
		float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(Vector3.One * radius);
		_meshDrawer?.DrawSphere(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Sphere(Vector3 position, Quaternion rotation, float radius = 1.0f,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(Vector3.One * radius);
		_meshDrawer?.DrawSphere(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Sphere(Vector3 position, float radius = 1.0f, Color? color = null,
		float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(Vector3.One * radius);
		_meshDrawer?.DrawSphere(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Sphere(Transform3D xform, SphereShape3D sphereShape, Color? color = null,
		float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(Vector3.One * sphereShape.Radius);
		_meshDrawer?.DrawSphere(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Sphere(Vector3 position, Quaternion rotation, SphereShape3D sphereShape,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(Vector3.One *
				sphereShape.Radius);
		_meshDrawer?.DrawSphere(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	
	public static void Sphere(Vector3 position, SphereShape3D sphereShape, Color? color = null,
		float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(Vector3.One * sphereShape.Radius);
		_meshDrawer?.DrawSphere(xform, duration, color, drawSolid, layers);
		#endif //TOOLS
	}


	//Point
	
	public static void Point(Transform3D xform, float size = 1.0f,
		Color? color = null, float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPoint(xform, duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Point(Vector3 position, Quaternion rotation, float size = 1.0f,
		Color? color = null, float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPoint(xform, duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Point(Vector3 position, float size = 1.0f, Color? color = null,
		float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPoint(xform, duration, color, layers);
		#endif //TOOLS
	}


	//Quad
	
	public static void Quad(Vector3 position, float size = 1.0f, Color? color = null,
		float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawQuad(xform, duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Quad(Transform3D xform, float size = 1.0f, Color? color = null,
		float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawQuad(xform, duration, color, layers);
		#endif //TOOLS
	}


	//Plane
	
	public static void Plane(Transform3D xform, float size = 1.0f, Color? color = null,
		float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPlane(xform, duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Plane(Vector3 position, Vector3 normal, float size = 1.0f,
		Color? color = null, float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		if (normal == Vector3.Zero)
		{
			return;
		}

		Transform3D xform = new(Basis.Identity, position);
		float dot = Mathf.Abs(normal.Dot(Vector3.Up));
		xform = xform.LookingAt(position + normal,
			dot > 0.99f ? Vector3.Right : Vector3.Up).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPlane(xform, duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Plane(Plane plane, float size = 1.0f, Color? color = null,
		float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		if (plane.Normal == Vector3.Zero)
		{
			return;
		}

		Transform3D xform = new(Basis.Identity, plane.GetCenter());
		float dot = Mathf.Abs(plane.Normal.Dot(Vector3.Up));
		xform = xform.LookingAt(xform.Origin + plane.Normal,
			dot > 0.99f ? Vector3.Right : Vector3.Up).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPlane(xform, duration, color, layers);
		#endif //TOOLS
	}


	//Circle
	
	public static void Circle(Transform3D xform, float radius = 1.0f, Color? color = null,
		float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(Vector3.One * radius);
		_meshDrawer?.DrawCircle(xform, duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Circle(Vector3 position, Quaternion rotation, float radius = 1.0f,
		Color? color = null, float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(Vector3.One * radius);
		_meshDrawer?.DrawCircle(xform, duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Circle(Vector3 position, float radius = 1.0f,
		Color? color = null, float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(Vector3.One * radius);
		_meshDrawer?.DrawCircle(xform, duration, color, layers);
		#endif //TOOLS
	}


	//Axes
	
	public static void Axes(Transform3D xform, float size = 25.0f, float duration = 0.0f,
		uint layers = 1 << 1)
	{
		#if TOOLS
		xform = xform.ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawAxes(xform, duration, layers);
		#endif //TOOLS
	}


	
	public static void Axes(Vector3 position, Quaternion rotation, float size = 1.0f,
		float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawAxes(xform, duration, layers);
		#endif //TOOLS
	}


	
	public static void Axes(Vector3 position, float size = 1.0f, float duration = 0.0f,
		uint layers = 1 << 1 << 1)
	{
		#if TOOLS
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawAxes(xform, duration, layers);
		#endif //TOOLS
	}


	//Lines
	
	public static void Line(Vector3 from, Vector3 to, Color? color = null, float duration = 0.0f,
		uint layers = 1 << 1 << 1)
	{
		#if TOOLS
		_meshDrawer?.DrawLine(from, to, duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Lines(Vector3[] points, Color? color = null, float duration = 0.0f,
		uint layers = 1 << 1 << 1)
	{
		#if TOOLS
		_meshDrawer?.DrawLines(points, duration, color, layers);
		#endif //TOOLS
	}


	//Text
	
	public static void TextKeyed(string key, object text, float duration = 0.0f, 
		Color? color = null, uint layers = 1 << 1 << 1)
	{
		#if TOOLS
		_canvasDrawer?.DrawTextKeyed(key, text.ToString(), duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Text(object text, float duration = 0.0f, Color? color = null,
		uint layers = 1 << 1)
	{
		#if TOOLS
		_canvasDrawer?.DrawText(text.ToString(), duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Text3DKeyed(string key, object text, Vector3 location,
		float duration = 0.0f, Color? color = null, uint layers = 1 << 1)
	{
		#if TOOLS
		_canvasDrawer?.DrawText3DKeyed(key, text.ToString(), location, duration, color, layers);
		#endif //TOOLS
	}


	
	public static void Text3D(object text, Vector3 location, float duration = 0.0f, 
		Color? color = null, uint layers = 1 << 1)
	{
		#if TOOLS
		_canvasDrawer?.DrawText3D(text.ToString(), location, duration, color, layers);
		#endif //TOOLS
	}


	//Arrow
	
	public static void Arrow(Vector3 position, Vector3 direction, float size = 1.0f,
		Color? color = null, float duration = 0.0f, uint layers = 1 << 1)
	{
		#if TOOLS
		if (direction == Vector3.Zero)
		{
			return;
		}

		Transform3D xform = new(Basis.Identity, position);
		float dot = Mathf.Abs(direction.Dot(Vector3.Up));
		xform = xform.LookingAt(position + direction, dot > 0.99f ? Vector3.Right : Vector3.Up)
			.ScaledLocal(Vector3.One * size);
		_meshDrawer.DrawArrow(xform, duration, color, layers);
		#endif //TOOLS
	}


	//Ray
	
	public static void RayIntersect(PhysicsRayQueryParameters3D query, GC.Dictionary result,
		float duration = 0.0f, Color? color = null, Color? hitColor = null, uint layers = 1 << 1)
	{
		#if TOOLS
		if (query == null)
		{
			return;
		}

		bool hit = false;
		if (result != null)
		{
			hit = result.Count > 0;
		}

		Vector3 hitLoc = Vector3.Zero;
		if (hit)
		{
			hitLoc = (Vector3)result["position"];
		}

		_meshDrawer?.DrawRay(query.From, query.To, hitLoc, hit, duration, color, hitColor,
			layers);
		#endif //TOOLS
	}


	
	public static void RayIntersect(Vector3 from, Vector3 to, Vector3 hitPos,
		float duration = 0.0f, Color? hitColor = null, Color? color = null, uint layers = 1 << 1)
	{
		#if TOOLS
		_meshDrawer?.DrawRay(from, to, hitPos, true, duration, color, hitColor, layers);
		#endif //TOOLS
	}


	//Shape Queries
	
	public static void ShapeMotion(PhysicsShapeQueryParameters3D query, float[] result,
		float duration = 0.0f, Color? color = null, Color? hitColor = null, uint layers = 1 << 1)
	{
		#if TOOLS
		if (query == null)
		{
			return;
		}

		Transform3D from = query.Transform;
		Transform3D to = new(query.Transform.Basis,
			query.Transform.Origin + query.Motion);


		Transform3D hitPos = Transform3D.Identity;
		bool hit = result.Length > 0;
		if (hit)
		{
			if (result[0] == 1.0f && result[1] == 1.0f)
			{
				hit = false;
			}
		}

		if (hit)
		{
			hitPos = new Transform3D(query.Transform.Basis,
				from.Origin + query.Motion * result[0]);
		}


		if (hit)
		{
			_meshDrawer?.DrawLine(from.Origin, hitPos.Origin, duration, color ?? Colors.Red,
				layers);
			_meshDrawer?.DrawLine(hitPos.Origin, to.Origin, duration, hitColor ?? Colors.Green,
				layers);
		}
		else
		{
			_meshDrawer?.DrawLine(from.Origin, to.Origin, duration, color ?? Colors.Red, layers);
		}

		Shape3D shape = (Shape3D)query.Shape;
		if (shape == null)
		{
			return;
		}

		Shape(from, (Shape3D)query.Shape, color ?? Colors.Red, duration, false, layers);
		Shape(to, (Shape3D)query.Shape, color ?? Colors.Green, duration, false, layers);

		if (hit)
		{
			Shape(hitPos, (Shape3D)query.Shape, hitColor ?? Colors.Green, duration, false, layers);
		}
		#endif //TOOLS
	}


	
	public static void ShapeCollision(PhysicsShapeQueryParameters3D query, GC.Array<Vector3> hits,
		float duration = 0.0f, Color? color = null, Color? hitColor = null, uint layers = 1 << 1)
	{
		#if TOOLS
		if (query == null || hits == null)
		{
			return;
		}

		bool hit = hits.Count > 0;
		if (hit)
		{
			foreach (Vector3 hitPos in hits)
			{
				Point(hitPos, 0.25f, hitColor ?? Colors.Red, duration, layers);
			}
		}

		Transform3D xform = query.Transform;

		Shape3D shape = (Shape3D)query.Shape;
		if (shape == null)
		{
			return;
		}

		Shape(xform, shape, hit ? hitColor ?? Colors.Green : color ?? Colors.Red, duration,
			false, layers);
		#endif //TOOLS
	}


	//Shape
	
	public static void Shape(Transform3D xform, Shape3D shape, Color? color = null,
		float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		switch (shape)
		{
			case BoxShape3D b:
				Box(xform, b, color, duration, drawSolid, layers);
				break;
			case SphereShape3D s:
				Sphere(xform, s, color, duration, drawSolid, layers);
				break;
			case CylinderShape3D c:
				Cylinder(xform, c, color, duration, drawSolid, layers);
				break;
			case WorldBoundaryShape3D wb:
				Plane(wb.Plane, 10.0f, color, duration, layers);
				break;
			default:
				return;
		}
		#endif //TOOLS
	}


	
	public static void Shape(Vector3 position, Quaternion rotation, Shape3D shape,
		Color? color = null, float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		switch (shape)
		{
			case BoxShape3D b:
				Box(position, rotation, b, color, duration, drawSolid, layers);
				break;
			case SphereShape3D s:
				Sphere(position, rotation, s, color, duration, drawSolid, layers);
				break;
			case CylinderShape3D c:
				Cylinder(position, rotation, c, color, duration, drawSolid, layers);
				break;
			case WorldBoundaryShape3D wb:
				Plane(wb.Plane, 10.0f, color, duration, layers);
				break;
			default:
				return;
		}
		#endif //TOOLS
	}


	
	public static void Shape(Vector3 position, Shape3D shape, Color? color = null,
		float duration = 0.0f, bool drawSolid = false, uint layers = 1 << 1)
	{
		#if TOOLS
		switch (shape)
		{
			case BoxShape3D b:
				Box(position, b, color, duration, drawSolid, layers);
				break;
			case SphereShape3D s:
				Sphere(position, s, color, duration, drawSolid, layers);
				break;
			case CylinderShape3D c:
				Cylinder(position, c, color, duration, drawSolid, layers);
				break;
			case WorldBoundaryShape3D wb:
				Plane(position, wb.Plane.Normal, 10.0f, color, duration, layers);
				break;
			default:
				return;
		}
		#endif //TOOLS
	}

	#endregion
}

#if TOOLS

namespace Burden.DebugDrawing
{
	internal class DebugMeshDrawer
	{
		private readonly Node _parent;

		public readonly ObjectPool<DrawLineInstance> LinePool;
		public readonly ObjectPool<DrawMeshInstance> MeshPool;

		private static StandardMaterial3D _defaultMaterialCache;
		private static StandardMaterial3D _additiveMaterialCache;

		private readonly DebugMeshCollection _boxCollection = new("Cube",
			DebugMeshes.Construct(DebugShape.Cube),
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _boxSolidCollection = new("CubeSolid",
			new BoxMesh
			{
				Size = Vector3.One
			},
			CreateDefaultMaterial(true));

		private readonly DebugMeshCollection _cylinderCollection = new("Cylinder",
			DebugMeshes.Construct(DebugShape.Cylinder),
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _cylinderSolidCollection = new("CylinderSolid",
			new CylinderMesh()
			{
				RadialSegments = 16, TopRadius = 0.5f, BottomRadius = 0.5f, Height = 1.0f
			},
			CreateDefaultMaterial(true));

		private readonly DebugMeshCollection _sphereCollection = new("Sphere",
			DebugMeshes.Construct(DebugShape.Sphere),
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _sphereSolidCollection = new("SphereSolid",
			new SphereMesh
			{
				RadialSegments = 16, Rings = 16, Height = 2.0f, Radius = 1.0f
			},
			CreateDefaultMaterial(true));

		private readonly DebugMeshCollection _pointCollection = new("Point",
			DebugMeshes.Construct(DebugShape.Point),
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _quadCollection = new("Quad",
			DebugMeshes.Construct(DebugShape.Quad),
			CreateDefaultMaterial(false, true));

		private readonly DebugMeshCollection _planeCollection = new("Plane",
			DebugMeshes.Construct(DebugShape.Plane, Mesh.PrimitiveType.Triangles),
			CreateDefaultMaterial(true));

		private readonly DebugMeshCollection _circleCollection = new("Circle",
			DebugMeshes.Construct(DebugShape.Circle),
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _axesCollection = new("Axes",
			DebugMeshes.Construct(DebugShape.Axes),
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _arrowCollection = new("Arrow",
			DebugMeshes.Construct(DebugShape.Arrow),
			CreateDefaultMaterial());

		private readonly HashSet<DrawLineInstance> _lineInstances = new();
		private readonly ImmediateMesh _linesMesh;
		private readonly MeshInstance3D _linesMeshInstance;

		private readonly DebugMeshCollection[] _collections;


		public DebugMeshDrawer(Node parent)
		{
			_parent = parent;

			LinePool = new ObjectPool<DrawLineInstance>();
			MeshPool = new ObjectPool<DrawMeshInstance>();

			//Line mesh
			_linesMesh = new ImmediateMesh();

			_linesMeshInstance = new MeshInstance3D();
			_linesMeshInstance.Mesh = _linesMesh;
			_linesMeshInstance.MaterialOverride = CreateDefaultMaterial();

			_parent.AddChild(_linesMeshInstance);

			_parent.AddChild(_boxCollection.MultiMeshInstance);
			_parent.AddChild(_boxSolidCollection.MultiMeshInstance);

			_parent.AddChild(_cylinderCollection.MultiMeshInstance);
			_parent.AddChild(_cylinderSolidCollection.MultiMeshInstance);

			_parent.AddChild(_sphereCollection.MultiMeshInstance);
			_parent.AddChild(_sphereSolidCollection.MultiMeshInstance);

			_parent.AddChild(_pointCollection.MultiMeshInstance);

			_parent.AddChild(_quadCollection.MultiMeshInstance);

			_parent.AddChild(_planeCollection.MultiMeshInstance);

			_parent.AddChild(_circleCollection.MultiMeshInstance);

			_parent.AddChild(_axesCollection.MultiMeshInstance);

			_parent.AddChild(_arrowCollection.MultiMeshInstance);

			_collections = new[]
			{
				_boxCollection, _boxSolidCollection,
				_cylinderCollection, _cylinderSolidCollection,
				_sphereCollection, _sphereSolidCollection,
				_pointCollection, _quadCollection, _planeCollection, _circleCollection,
				_axesCollection, _arrowCollection
			};


			StandardMaterial3D quadMaterial =
				(StandardMaterial3D)_quadCollection.MultiMeshInstance.MaterialOverride;

			quadMaterial.BillboardMode = BaseMaterial3D.BillboardModeEnum.Enabled;
			quadMaterial.BillboardKeepScale = true;

			_quadCollection.MultiMeshInstance.MaterialOverride = quadMaterial;

			DebugMeshCollection.OnInstanceRemoved += inst => MeshPool.Return(inst);
			SetDepthTestEnabled(DebugDraw.GetDoDepthTest());
		}


		protected static StandardMaterial3D CreateDefaultMaterial(bool additive = false,
			bool unique = false)
		{
			if (!unique)
			{
				if (additive && _additiveMaterialCache != null)
				{
					return _additiveMaterialCache;
				}

				if (_defaultMaterialCache != null)
				{
					return _defaultMaterialCache;
				}
			}


			StandardMaterial3D material = new()
			{
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
				BlendMode = additive
					? BaseMaterial3D.BlendModeEnum.Add
					: BaseMaterial3D.BlendModeEnum.Mix,
				VertexColorUseAsAlbedo = true,
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
				CullMode = BaseMaterial3D.CullModeEnum.Back
			};

			if (!unique)
			{
				if (additive)
				{
					_additiveMaterialCache = material;
				}
				else
				{
					_defaultMaterialCache = material;
				}
			}

			return material;
		}


		private DrawMeshInstance GetAMeshInstance(Transform3D xform, float duration, Color? color,
			uint layers)
		{
			DrawMeshInstance inst = MeshPool.Retrieve();
			if (inst != null)
			{
				inst.Transform = xform;
				inst.SetDuration(duration);
				inst.Color = color ?? Colors.White;
				inst.DrawLayers = layers;
			}

			return inst;
		}


		private DrawLineInstance GetALineInstance(Vector3[] points, float duration,
			Color? color, uint layers)
		{
			DrawLineInstance inst = LinePool.Retrieve();
			if (inst != null)
			{
				inst.Points = points;
				inst.SetDuration(duration);
				inst.Color = color ?? Colors.White;
				inst.DrawLayers = layers;
			}

			return inst;
		}


		public void Update()
		{
			foreach (DebugMeshCollection collection in _collections)
			{
				collection.Update();
			}

			DrawLines();
		}


		public void SetDepthTestEnabled(bool doDepthTest)
		{
			foreach (DebugMeshCollection collection in _collections)
			{
				((StandardMaterial3D)collection.MultiMeshInstance.MaterialOverride).NoDepthTest =
					!doDepthTest;
			}

			((StandardMaterial3D)_linesMeshInstance.MaterialOverride).NoDepthTest = !doDepthTest;
		}


		#region Drawing

		protected void DrawLines()
		{
			DrawLineInstance[] expired =
				_lineInstances.Where(instance => instance.IsExpired()).ToArray();

			foreach (DrawLineInstance instance in expired)
			{
				LinePool.Return(instance);
				_lineInstances.Remove(instance);
			}

			_linesMesh.ClearSurfaces();

			if (_lineInstances.Count == 0)
			{
				return;
			}

			_linesMesh.SurfaceBegin(Mesh.PrimitiveType.Lines, CreateDefaultMaterial());

			foreach (DrawLineInstance line in _lineInstances)
			{
				_linesMesh.SurfaceSetColor(line.Color);
				for (int i = 1; i < line.Points.Length; i++)
				{
					_linesMesh.SurfaceAddVertex(line.Points[i - 1]);
					_linesMesh.SurfaceAddVertex(line.Points[i]);
				}

				line.BeenDrawn = true;
			}

			_linesMesh.SurfaceEnd();
		}


		public void DrawBox(Transform3D xform, float duration, Color? color, bool drawSolid,
			uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			(drawSolid ? _boxSolidCollection : _boxCollection).Add(
				GetAMeshInstance(xform, duration, color, layers));
		}


		public void DrawCylinder(Transform3D xform, float duration, Color? color, bool drawSolid,
			uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			(drawSolid ? _cylinderSolidCollection : _cylinderCollection).Add(
				GetAMeshInstance(xform, duration, color, layers));
		}


		public void DrawCapsule(Transform3D xform, float radius, float height, float duration,
			Color? color, bool drawSolid, uint layers)

		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			Transform3D capXForm = xform;
			capXForm = capXForm.ScaledLocal(Vector3.One * radius);

			float d = radius * 2.0f;

			xform = xform.ScaledLocal(new Vector3(d, height - d, d));

			float capOffset = (height - d) / (2 * radius);
			capXForm.Origin = xform.Origin + capXForm.Basis.Y * capOffset;

			(drawSolid ? _sphereSolidCollection : _sphereCollection).Add(
				GetAMeshInstance(capXForm, duration, color, layers));

			capXForm.Origin = xform.Origin + -capXForm.Basis.Y * capOffset;
			(drawSolid ? _sphereSolidCollection : _sphereCollection).Add(
				GetAMeshInstance(capXForm, duration, color, layers));

			(drawSolid ? _cylinderSolidCollection : _cylinderCollection).Add(
				GetAMeshInstance(xform, duration, color, layers));
		}


		public void DrawSphere(Transform3D xform, float duration, Color? color, bool drawSolid,
			uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			(drawSolid ? _sphereSolidCollection : _sphereCollection).Add(
				GetAMeshInstance(xform, duration, color, layers));
		}


		public void DrawPoint(Transform3D xform, float duration, Color? color,
			uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			_pointCollection.Add(GetAMeshInstance(xform, duration, color ?? Colors.White,
				layers));
		}


		public void DrawQuad(Transform3D xform, float duration, Color? color,
			uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			_quadCollection.Add(GetAMeshInstance(xform, duration, color ?? Colors.White, layers));
		}


		public void DrawPlane(Transform3D xform, float duration,
			Color? color, uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			_planeCollection.Add(GetAMeshInstance(xform, duration, 
				color ?? new Color(Colors.White, 0.5f), layers));
			DrawArrow(xform, duration, color ?? Colors.White, layers);
		}


		public void DrawCircle(Transform3D xform, float duration, Color? color, uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			_circleCollection.Add(GetAMeshInstance(xform, duration, color ?? Colors.White, layers));
		}


		public void DrawAxes(Transform3D xform, float duration, uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			_axesCollection.Add(GetAMeshInstance(xform, duration, new Color(1, 1, 1), layers));
		}


		public void DrawLine(Vector3 from, Vector3 to, float duration, Color? color, uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			DrawLineInstance line = GetALineInstance(new[] {from, to}, duration, color, layers);
			if (line != null)
			{
				_lineInstances.Add(line);
			}
		}


		public void DrawLines(Vector3[] points, float duration, Color? color, uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			if (points == null || points.Length <= 1)
			{
				return;
			}

			DrawLineInstance line = GetALineInstance(points, duration, color, layers);
			if (line != null)
			{
				_lineInstances.Add(line);
			}
		}


		public void DrawRay(Vector3 from, Vector3 to, Vector3 hitLoc, bool hit, float duration,
			Color? rayColor, Color? hitColor, uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			if (hit)
			{
				DrawLine(from, hitLoc, duration, rayColor ?? Colors.Red, layers);
				Transform3D xform =
					new Transform3D(Basis.Identity, hitLoc).ScaledLocal(Vector3.One * 0.25f);
				DrawQuad(xform, duration, hitColor ?? Colors.Green, layers);
				DrawLine(hitLoc, to, duration, hitColor ?? Colors.Green, layers);
			}
			else
			{
				DrawLine(from, to, duration, rayColor ?? Colors.Red, layers);
			}
		}

		#endregion


		public void DrawArrow(Transform3D xform, float duration, Color? color, uint layers)
		{
			if ((DebugDraw.GetEnabledLayers() & layers) == 0)
			{
				return;
			}

			_arrowCollection.Add(GetAMeshInstance(xform, duration, color ?? Colors.White, layers));
		}
	}


	internal partial class DebugCanvasDrawer : GodotObject
	{
		private Font _textFont;
		private int _fontSize = 12;
		private int _screenEdgePadding = 16;
		private int _textEntryExtraPadding = 4;

		public readonly ObjectPool<DrawTextInstance> TextPool;

		private readonly Dictionary<string, DrawTextInstance> _keyedTextEntries = new();
		private readonly HashSet<DrawTextInstance> _textEntries = new();

		public readonly ObjectPool<DrawText3DInstance> Text3DPool;

		private readonly Dictionary<string, DrawText3DInstance> _keyedText3dEntries = new();
		private readonly HashSet<DrawText3DInstance> _text3dEntries = new();

		private Node _parent;

		private readonly Node2D _canvas2D;
		private readonly Node2D _canvas3D;


		public DebugCanvasDrawer(Node parent)
		{
			_parent = parent;

			TextPool = new ObjectPool<DrawTextInstance>();
			Text3DPool = new ObjectPool<DrawText3DInstance>();

			_canvas2D = new Node2D();
			_parent.AddChild(_canvas2D);
			_canvas2D.ZIndex = 100;
			_canvas2D.Connect("draw", new Callable(this, nameof(DrawCanvas2D)));


			_canvas3D = new Node2D();
			_parent.AddChild(_canvas3D);
			_canvas3D.ZIndex = 101;
			_canvas3D.Connect("draw", new Callable(this, nameof(DrawCanvas3D)));

			//https://godotengine.org/qa/7307/getting-default-editor-font-for-draw_string
			Label label = new();
			_textFont = label.GetThemeDefaultFont();
			label.Free();
		}


		public void DrawTextKeyed(string key, string text, float duration, Color? color,
			uint layers)
		{
			string msg = $"{key}|{text}";

			if (_keyedTextEntries.ContainsKey(key))
			{
				if (_keyedTextEntries[key].Text != msg)
				{
					_keyedTextEntries[key].Text = msg;
					_canvas2D.QueueRedraw();
				}

				_keyedTextEntries[key].SetDuration(duration);
				_keyedTextEntries[key].Color = color ?? Colors.Gray;
				_keyedTextEntries[key].DrawLayers = layers;
			}
			else
			{
				DrawTextInstance inst = TextPool.Retrieve();
				if (inst != null)
				{
					inst.Text = msg;
					inst.SetDuration(duration);
					inst.Color = color ?? Colors.Gray;
					inst.DrawLayers = layers;
					_keyedTextEntries.Add(key, inst);
					_canvas2D.QueueRedraw();
				}
			}
		}


		public void DrawText(string text, float duration, Color? color, uint layers)
		{
			DrawTextInstance inst = TextPool.Retrieve();
			if (inst != null)
			{
				inst.Text = text;
				inst.SetDuration(duration);
				inst.Color = color ?? Colors.Gray;
				inst.DrawLayers = layers;
				_textEntries.Add(inst);
				_canvas2D.QueueRedraw();
			}
		}


		public void DrawText3DKeyed(string key, string text, Vector3 location, float duration,
			Color? color, uint layers)
		{
			string msg = $"{key}|{text}";

			if (_keyedText3dEntries.ContainsKey(key))
			{
				if (_keyedText3dEntries[key].Text != msg)
				{
					_keyedText3dEntries[key].Text = msg;
				}

				_keyedText3dEntries[key].SetDuration(duration);
				_keyedText3dEntries[key].Color = color ?? Colors.Gray;
				_keyedText3dEntries[key].DrawLayers = layers;
			}
			else
			{
				DrawText3DInstance inst = Text3DPool.Retrieve();
				if (inst != null)
				{
					inst.Text = msg;
					inst.Location = location;
					inst.SetDuration(duration);
					inst.Color = color ?? Colors.Gray;
					inst.DrawLayers = layers;
					_keyedText3dEntries.Add(key, inst);
				}
			}
		}


		public void DrawText3D(string text, Vector3 location, float duration, Color? color,
			uint layers)
		{
			{
				DrawText3DInstance inst = Text3DPool.Retrieve();
				if (inst != null)
				{
					inst.Text = text;
					inst.Location = location;
					inst.SetDuration(duration);
					inst.Color = color ?? Colors.Gray;
					inst.DrawLayers = layers;
					_text3dEntries.Add(inst);
				}
			}
		}


		public void Update()
		{
			foreach (KeyValuePair<string, DrawTextInstance> entry in _keyedTextEntries)
			{
				if (entry.Value.IsExpired())
				{
					TextPool.Return(entry.Value);
					_keyedTextEntries.Remove(entry.Key);
					_canvas2D.QueueRedraw();
				}
			}

			foreach (DrawTextInstance entry in _textEntries)
			{
				if (entry.IsExpired())
				{
					TextPool.Return(entry);
					_textEntries.Remove(entry);
					_canvas2D.QueueRedraw();
				}
			}


			//Always update 3d canvas
			_canvas3D.QueueRedraw();
			foreach (KeyValuePair<string, DrawText3DInstance> entry in _keyedText3dEntries)
			{
				if (entry.Value.IsExpired())
				{
					Text3DPool.Return(entry.Value);
					_keyedText3dEntries.Remove(entry.Key);
				}
			}

			foreach (DrawText3DInstance entry in _text3dEntries)
			{
				if (entry.IsExpired())
				{
					Text3DPool.Return(entry);
					_text3dEntries.Remove(entry);
					_canvas2D.QueueRedraw();
				}
			}
		}


		protected void DrawCanvas2D()
		{
			Vector2 pos = new(_screenEdgePadding, _screenEdgePadding + _fontSize * 1.5f);
			foreach (DrawTextInstance msg in _keyedTextEntries.Values)
			{
				DrawString(msg);
			}

			foreach (DrawTextInstance msg in _textEntries)
			{
				DrawString(msg);
			}

			void DrawString(DrawTextInstance msg)
			{
				if (msg.IsExpired())
				{
					return;
				}

				_canvas2D.DrawString(_textFont, pos, msg.Text, HorizontalAlignment.Left, -1,
					_fontSize, msg.Color);

				pos.Y += _fontSize * 1.5f;
				msg.BeenDrawn = true;
			}
		}


		protected void DrawCanvas3D()
		{
			Camera3D camera = _canvas3D.GetViewport().GetCamera3D();
			foreach (DrawText3DInstance msg in _keyedText3dEntries.Values)
			{
				DrawString3D(msg);
			}

			foreach (DrawText3DInstance msg in _text3dEntries)
			{
				DrawString3D(msg);
			}

			void DrawString3D(DrawText3DInstance msg)
			{
				Vector2 offset = _textFont.GetStringSize(msg.Text, HorizontalAlignment.Left,
					-1f, _fontSize) * 0.5f;
				Vector2 pos = camera.UnprojectPosition(msg.Location) - offset;
				_canvas3D.DrawString(_textFont, pos, msg.Text, HorizontalAlignment.Left, -1,
					_fontSize, msg.Color);
				msg.BeenDrawn = true;
			}
		}
	}


	internal class DebugMeshCollection
	{
		public MultiMeshInstance3D MultiMeshInstance { get; }
		private readonly HashSet<DrawMeshInstance> _drawInstances = new();

		public static Action<DrawMeshInstance> OnInstanceRemoved;


		public DebugMeshCollection(string name, Mesh instanceMesh,
			Material instanceMaterial)
		{
			MultiMeshInstance = new MultiMeshInstance3D
			{
				Name = name,
				CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
				GIMode = GeometryInstance3D.GIModeEnum.Disabled,
				IgnoreOcclusionCulling = true,

				MaterialOverride = instanceMaterial,
				Multimesh = new MultiMesh
				{
					TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
					UseColors = true,
					Mesh = instanceMesh
				}
			};
		}


		public void Update()
		{
			DrawMeshInstance[] expired =
				_drawInstances.Where(instance => instance.IsExpired()).ToArray();

			foreach (DrawMeshInstance instance in expired)
			{
				_drawInstances.Remove(instance);
				OnInstanceRemoved?.Invoke(instance);
			}

			if (MultiMeshInstance.Multimesh.InstanceCount != _drawInstances.Count)
			{
				MultiMeshInstance.Multimesh.InstanceCount = _drawInstances.Count;
			}

			if (_drawInstances.Count == 0)
			{
				return;
			}

			int i = 0;
			foreach (DrawMeshInstance instance in _drawInstances)
			{
				MultiMeshInstance.Multimesh.SetInstanceTransform(i, instance.Transform);
				MultiMeshInstance.Multimesh.SetInstanceColor(i, instance.Color);
				instance.BeenDrawn = true;
				i++;
			}
		}


		public void Add(DrawMeshInstance instance)
		{
			if (instance != null)
			{
				_drawInstances.Add(instance);
			}
		}
	}


	internal interface IPoolable
	{
		void Reset();
	}


	internal class ObjectPool<T> where T : IPoolable, new()
	{
		public readonly int MaxSize;
		public int CurrentSize;
		public int FreeObjects;

		private readonly Queue<T> _pool;


		public ObjectPool()
		{
			_pool = new Queue<T>();
			MaxSize = DebugDraw.GetMaxPoolSize();
			ExpandPool(DebugDraw.GetStartingPoolSize());
		}


		public T Retrieve()
		{
			if (FreeObjects == 0 && !ExpandPool(1))
			{
				GD.PushWarning(
					$"{GetType()} pool has no free objects, consider increasing max size");
				return default;
			}

			FreeObjects--;
			return _pool.Dequeue();
		}


		public bool ExpandPool(int expansion)
		{
			expansion = Mathf.Min(expansion, MaxSize - CurrentSize);
			if (expansion == 0)
			{
				return false;
			}

			for (int i = 0; i < expansion; i++)
			{
				_pool.Enqueue(new T());
			}

			FreeObjects += expansion;
			CurrentSize += expansion;
			return true;
		}


		public void Return(T obj)
		{
			obj.Reset();
			_pool.Enqueue(obj);
			FreeObjects++;
		}
	}


	internal class DrawInstance : IPoolable
	{
		public void SetDuration(float duration)
		{
			ExpirationTime = Time.GetTicksMsec() + (ulong)(duration * 1000.0f);
			BeenDrawn = false;
		}


		public Color Color;
		public bool BeenDrawn;
		protected ulong ExpirationTime;
		public uint DrawLayers;


		public virtual bool IsExpired()
		{
			return (DebugDraw.GetEnabledLayers() & DrawLayers) == 0 ||
			       (Time.GetTicksMsec() > ExpirationTime && BeenDrawn);
		}


		public virtual void Reset()
		{
			BeenDrawn = false;
			Color = default;
			ExpirationTime = 0;
			DrawLayers = 0;
		}
	}


	internal class DrawMeshInstance : DrawInstance
	{
		public Transform3D Transform;


		public override void Reset()
		{
			base.Reset();
			Transform = default;
		}
	}


	internal class DrawTextInstance : DrawInstance
	{
		public string Text;


		public override void Reset()
		{
			base.Reset();
			Text = default;
		}
	}


	internal class DrawText3DInstance : DrawTextInstance
	{
		public Vector3 Location;


		public override void Reset()
		{
			base.Reset();
			Location = default;
		}
	}


	internal class DrawLineInstance : DrawInstance
	{
		public Vector3[] Points;


		public override void Reset()
		{
			base.Reset();
			Points = null;
		}
	}
}
#endif //TOOLS
