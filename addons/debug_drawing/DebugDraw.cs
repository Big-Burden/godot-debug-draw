using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Burden.DebugDrawing;
using Godot;
using GC = Godot.Collections;


//TODO:
/*
 Add editor dock for toggling debug layers, and add in in-game access aswell
 also include toggling disabling depth draw
 
 check that everything is being destroyed on exit tree or free of this node.
 
 add support for un-keyed text
 
 Figure out a better way of keying text/format
 
 look into line drawing of x points
 */


[Flags]
public enum DebugLayers
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


public partial class DebugDraw : Node
{
	public const int MaxPoolSize = 512;
	public const int StartingPoolSize = 64;

	private static DebugMeshDrawer _meshDrawer;
	private static DebugCanvasDrawer _canvasDrawer;


	public static bool DrawingEnabled = true;


	public static int EnabledLayers { get; private set; } = (int)DebugLayers.All;
	public static Action OnDrawSettingsUpdated;

	public static bool DoDepthTest { get; private set; } = true;

	private DebugDock _dock;


	public DebugDraw()
	{
		Name = "DebugDraw";
		_meshDrawer = new DebugMeshDrawer(this);
		_canvasDrawer = new DebugCanvasDrawer(this);
	}

	public override void _Ready()
	{
		base._Ready();
		GD.Print(EnabledLayers);
		if (!Engine.IsEditorHint())
		{
			_dock = GD.Load<PackedScene>("res://addons/debug_drawing/control/debug_dock.tscn")
				.Instantiate<DebugDock>();
			AddChild(_dock);
			_dock.Owner = this;
		}
	}


	public override void _ExitTree()
	{
		base._ExitTree();
		_canvasDrawer.Free();
		_canvasDrawer = null;
		_meshDrawer = null;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (DrawingEnabled)
		{
			_meshDrawer.Update();
			_canvasDrawer.Update();
		}
	}

	public static void SetDrawingDepthTestEnabled(bool enabled)
	{
		if (enabled != DoDepthTest)
		{
			DoDepthTest = enabled;
			_meshDrawer.SetDepthTestEnabled(DoDepthTest);
			OnDrawSettingsUpdated?.Invoke();
		}
	}

	public static void SetEnabledLayers(int layers)
	{
		EnabledLayers = layers;
		OnDrawSettingsUpdated?.Invoke();
	}

	public static void SetLayerEnabled(int layer, bool enabled)
	{
		if (enabled)
		{
			EnabledLayers |= layer;
		}
		else
		{
			EnabledLayers &= ~layer;
		}

		OnDrawSettingsUpdated?.Invoke();
	}

	#region Drawing

	//Cube
	[Conditional("DEBUG")]
	public static void DrawCube(Transform3D xform, Vector3 size, float duration = 0.0f,
		Color? color = null, bool drawSolid = false, DebugLayers layers = DebugLayers.Layer1)
	{
		xform = xform.ScaledLocal(size);
		_meshDrawer?.DrawCube(xform, duration, color, drawSolid, layers);
	}

	[Conditional("DEBUG")]
	public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 size,
		float duration = 0.0f, Color? color = null, bool drawSolid = false,
		DebugLayers layers = DebugLayers.Layer1)
	{
		Transform3D xform = new Transform3D(new Basis(rotation), position).ScaledLocal(size);
		_meshDrawer?.DrawCube(xform, duration, color, drawSolid, layers);
	}

	//Cylinder
	[Conditional("DEBUG")]
	public static void DrawCylinder(Transform3D xform, float height = 1.0f, float radius = 1.0f,
		float duration = 0.0f, Color? color = null, bool drawSolid = false,
		DebugLayers layers = DebugLayers.Layer1)
	{
		xform = xform.ScaledLocal(new Vector3(radius, height, radius));
		_meshDrawer?.DrawCylinder(xform, duration, color, drawSolid, layers);
	}

	[Conditional("DEBUG")]
	public static void DrawCylinder(Vector3 position, Quaternion rotation, float height = 1.0f,
		float radius = 1.0f, float duration = 0.0f, Color? color = null, bool drawSolid = false,
		DebugLayers layers = DebugLayers.Layer1)
	{
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(new Vector3(radius, height,
				radius));
		_meshDrawer?.DrawCylinder(xform, duration, color, drawSolid, layers);
	}

	//Sphere
	[Conditional("DEBUG")]
	public static void DrawSphere(Transform3D xform, float radius = 1.0f, float duration = 0.0f,
		Color? color = null, bool drawSolid = false, DebugLayers layers = DebugLayers.Layer1)
	{
		xform = xform.ScaledLocal(Vector3.One * radius);
		_meshDrawer?.DrawSphere(xform, duration, color, drawSolid, layers);
	}

	[Conditional("DEBUG")]
	public static void DrawSphere(Vector3 position, Quaternion rotation, float radius = 1.0f,
		float duration = 0.0f, Color? color = null, bool drawSolid = false,
		DebugLayers layers = DebugLayers.Layer1)
	{
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(Vector3.One * radius);
		_meshDrawer?.DrawSphere(xform, duration, color, drawSolid, layers);
	}


	//Point
	[Conditional("DEBUG")]
	public static void DrawPoint(Transform3D xform, float size = 1.0f, float duration = 0.0f,
		Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		xform = xform.ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPoint(xform, duration, color, layers);
	}

	[Conditional("DEBUG")]
	public static void DrawPoint(Vector3 position, Quaternion rotation, float size = 1.0f,
		float duration = 0.0f, Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPoint(xform, duration, color, layers);
	}

	[Conditional("DEBUG")]
	public static void DrawPoint(Vector3 position, float size = 1.0f, float duration = 0.0f,
		Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPoint(xform, duration, color, layers);
	}

	//Quad
	[Conditional("DEBUG")]
	public static void DrawQuad(Vector3 position, float size = 1.0f, float duration = 0.0f,
		Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawQuad(xform, duration, color, layers);
	}

	[Conditional("DEBUG")]
	public static void DrawQuad(Transform3D xform, float size = 1.0f, float duration = 0.0f,
		Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		xform = xform.ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawQuad(xform, duration, color, layers);
	}

	//Plane
	[Conditional("DEBUG")]
	public static void DrawPlane(Transform3D xform, float size = 1.0f,
		float duration = 0.0f, Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		xform = xform.ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPlane(xform, duration, color, layers);
	}

	public static void DrawPlane(Vector3 position, Vector3 normal, float size = 1.0f,
		float duration = 0.0f, Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		var xform = new Transform3D(Basis.Identity, position);
		xform = xform.LookingAt(position + normal, Vector3.Up).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPlane(xform, duration, color, layers);
	}

	public static void DrawPlane(Plane plane, float size = 1.0f, float duration = 0.0f,
		Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		var xform = new Transform3D(Basis.Identity, plane.GetCenter());
		xform = xform.LookingAt(xform.Origin + plane.Normal, Vector3.Up)
			.ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawPlane(xform, duration, color, layers);
	}


	//Circle
	[Conditional("DEBUG")]
	public static void DrawCircle(Vector3 position, float radius = 1.0f, float duration = 0.0f,
		Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		Transform3D xform =
			new Transform3D(Basis.Identity, position).ScaledLocal(Vector3.One * radius);
		_meshDrawer?.DrawCircle(xform, duration, color, layers);
	}

	[Conditional("DEBUG")]
	public static void DrawCircle(Transform3D xform, float radius = 1.0f, float duration = 0.0f,
		Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		xform = xform.ScaledLocal(Vector3.One * radius);
		_meshDrawer?.DrawCircle(xform, duration, color, layers);
	}

	//Axes
	[Conditional("DEBUG")]
	public static void DrawAxes(Transform3D xform, float size = 25.0f, float duration = 0.0f,
		DebugLayers layers = DebugLayers.Layer1)
	{
		xform = xform.ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawAxes(xform, duration, layers);
	}

	[Conditional("DEBUG")]
	public static void DrawAxes(Vector3 position, Quaternion rotation, float size = 1.0f,
		float duration = 0.0f, DebugLayers layers = DebugLayers.Layer1)
	{
		Transform3D xform =
			new Transform3D(new Basis(rotation), position).ScaledLocal(Vector3.One * size);
		_meshDrawer?.DrawAxes(xform, duration, layers);
	}

	//Lines
	[Conditional("DEBUG")]
	public static void DrawLine(Vector3 from, Vector3 to, float duration = 0.0f,
		Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		_meshDrawer?.DrawLine(from, to, duration, color, layers);
	}

	[Conditional("DEBUG")]
	public static void DrawLines(Vector3[] points, float duration = 0.0f, Color? color = null,
		DebugLayers layers = DebugLayers.Layer1)
	{
		_meshDrawer?.DrawLines(points, duration, color, layers);
	}

	//Text
	[Conditional("DEBUG")]
	public static void DrawText(string key, object text, float duration = 0.0f, Color? color = null,
		DebugLayers layers = DebugLayers.Layer1)
	{
		_canvasDrawer?.DrawText(key, text.ToString(), duration, color, layers);
	}

	[Conditional("DEBUG")]
	public static void DrawText3D(string key, object text, Vector3 location, float duration = 0.0f,
		Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		_canvasDrawer?.DrawText3D(key, text.ToString(), location, duration, color, layers);
	}

	//Ray
	[Conditional("DEBUG")]
	public static void DrawRay(PhysicsRayQueryParameters3D query,
		GC.Dictionary result, float duration = 0.0f, Color? rayColor = null,
		Color? hitColor = null, DebugLayers layers = DebugLayers.Layer1)
	{
		bool hit = result.Count > 0;
		Vector3 hitLoc = Vector3.Zero;
		if (hit)
		{
			hitLoc = (Vector3)result["position"];
		}

		_meshDrawer?.DrawRay(query.From, query.To, hitLoc, hit, duration, rayColor, hitColor,
			layers);
	}

	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 from, Vector3 to, Vector3 hit, float duration = 0.0f,
		Color? rayColor = null, Color? hitColor = null, DebugLayers layers = DebugLayers.Layer1)
	{
		_meshDrawer?.DrawRay(from, to, hit, true, duration, rayColor, hitColor, layers);
	}

	//Arrow
	public static void DrawArrow(Vector3 position, Vector3 direction, float size = 1.0f,
		float duration = 0.0f, Color? color = null, DebugLayers layers = DebugLayers.Layer1)
	{
		var xform = new Transform3D(Basis.Identity, position);
		xform = xform.LookingAt(position + direction, Vector3.Up).ScaledLocal(Vector3.One * size);
		_meshDrawer.DrawArrow(xform, duration, color, layers);
	}

	#endregion
}


namespace Burden.DebugDrawing
{
	internal class DebugMeshDrawer
	{
		private readonly Node _parent;

		private readonly ObjectPool<DebugMeshInstance> _meshPool = new();
		private readonly ObjectPool<DebugLineInstance> _linePool = new();


		private readonly DebugMeshCollection _cubeCollection = new("Cube",
			DebugMeshes.Construct(DebugShape.Cube),
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _cubeSolidCollection = new("CubeSolid",
			new BoxMesh
			{
				Size = Vector3.One
			},
			CreateDefaultMaterial(true));

		private readonly DebugMeshCollection _cylinderCollection = new("Cylinder",
			DebugMeshes.Construct(DebugShape.Cylinder),
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _cylinderSolidCollection = new("CylinderSolid",
			new CylinderMesh
			{
				RadialSegments = 8, TopRadius = 0.5f, BottomRadius = 0.5f, Height = 1.0f
			},
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _sphereCollection = new("Sphere",
			DebugMeshes.Construct(DebugShape.Sphere),
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _sphereSolidCollection = new("SphereSolid",
			new SphereMesh
			{
				RadialSegments = 16, Rings = 16, Height = 1.0f, Radius = 0.5f
			},
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _pointCollection = new("Point",
			DebugMeshes.Construct(DebugShape.Point),
			CreateDefaultMaterial());

		private readonly DebugMeshCollection _quadCollection = new("Quad",
			DebugMeshes.Construct(DebugShape.Quad),
			CreateDefaultMaterial());

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

		private readonly HashSet<DebugLineInstance> _lineInstances = new();
		private readonly ImmediateMesh _linesMesh;
		private readonly MeshInstance3D _linesMeshInstance;

		private readonly DebugMeshCollection[] _collections;


		public DebugMeshDrawer(Node parent)
		{
			_parent = parent;

			//Line mesh
			_linesMesh = new ImmediateMesh();

			_linesMeshInstance = new MeshInstance3D();
			_linesMeshInstance.Mesh = _linesMesh;
			_linesMeshInstance.MaterialOverride = CreateDefaultMaterial();

			_parent.AddChild(_linesMeshInstance);

			_parent.AddChild(_cubeCollection.MultiMeshInstance);
			_parent.AddChild(_cubeSolidCollection.MultiMeshInstance);

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
				_cubeCollection, _cubeSolidCollection,
				_cylinderCollection, _cylinderSolidCollection,
				_sphereCollection, _sphereSolidCollection,
				_pointCollection, _quadCollection, _planeCollection, _circleCollection,
				_axesCollection, _arrowCollection
			};


			var quadMaterial =
				(StandardMaterial3D)_quadCollection.MultiMeshInstance.MaterialOverride;

			quadMaterial.BillboardMode = BaseMaterial3D.BillboardModeEnum.Enabled;
			quadMaterial.BillboardKeepScale = true;

			_quadCollection.MultiMeshInstance.MaterialOverride = quadMaterial;

			DebugMeshCollection.OnInstanceRemoved += inst => _meshPool.Return(inst);
		}


		protected static StandardMaterial3D CreateDefaultMaterial(bool additive = false,
			bool backfaceCulling = true)
		{
			return new StandardMaterial3D
			{
				ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
				BlendMode = additive
					? BaseMaterial3D.BlendModeEnum.Add
					: BaseMaterial3D.BlendModeEnum.Mix,
				VertexColorUseAsAlbedo = true,
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
				CullMode = backfaceCulling
					? BaseMaterial3D.CullModeEnum.Back
					: BaseMaterial3D.CullModeEnum.Disabled
			};
		}


		private DebugMeshInstance GetAMeshInstance(Transform3D xform, float duration, Color? color,
			DebugLayers layers)
		{
			DebugMeshInstance inst = _meshPool.Retrieve();
			if (inst != null)
			{
				inst.Transform = xform;
				inst.SetDuration(duration);
				inst.Color = color ?? Colors.White;
				inst.DrawLayers = layers;
			}

			return inst;
		}


		private DebugLineInstance GetALineInstance(Vector3[] points, float duration,
			Color? color, DebugLayers layers)
		{
			DebugLineInstance inst = _linePool.Retrieve();
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
			_cubeCollection.Update();
			_cubeSolidCollection.Update();

			_cylinderCollection.Update();
			_cylinderSolidCollection.Update();

			_sphereCollection.Update();
			_sphereSolidCollection.Update();

			_pointCollection.Update();
			_quadCollection.Update();
			_planeCollection.Update();
			_circleCollection.Update();
			_axesCollection.Update();
			_arrowCollection.Update();
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
			DebugLineInstance[] expired =
				_lineInstances.Where(instance => instance.IsExpired()).ToArray();

			foreach (DebugLineInstance instance in expired)
			{
				_linePool.Return(instance);
				_lineInstances.Remove(instance);
			}

			_linesMesh.ClearSurfaces();

			if (_lineInstances.Count == 0)
			{
				return;
			}


			_linesMesh.SurfaceBegin(Mesh.PrimitiveType.Lines, CreateDefaultMaterial());

			foreach (DebugLineInstance line in _lineInstances)
			{
				_linesMesh.SurfaceSetColor(line.Color);
				foreach (Vector3 point in line.Points)
				{
					_linesMesh.SurfaceAddVertex(point);
				}

				line.BeenDrawn = true;
			}

			_linesMesh.SurfaceEnd();
		}

		public void DrawCube(Transform3D xform, float duration, Color? color,
			bool drawSolid, DebugLayers layers)
		{
			(drawSolid ? _cubeSolidCollection : _cubeCollection).Add(
				GetAMeshInstance(xform, duration, color, layers));
		}

		public void DrawCylinder(Transform3D xform, float duration, Color? color,
			bool drawSolid, DebugLayers layers)
		{
			(drawSolid ? _cylinderSolidCollection : _cylinderCollection).Add(
				GetAMeshInstance(xform, duration, color, layers));
		}

		public void DrawSphere(Transform3D xform, float duration, Color? color,
			bool drawSolid, DebugLayers layers)
		{
			(drawSolid ? _sphereSolidCollection : _sphereCollection).Add(
				GetAMeshInstance(xform, duration, color, layers));
		}

		public void DrawPoint(Transform3D xform, float duration, Color? color,
			DebugLayers layers)
		{
			_pointCollection.Add(GetAMeshInstance(xform, duration, color ?? Colors.White,
				layers));
		}

		public void DrawQuad(Transform3D xform, float duration, Color? color,
			DebugLayers layers)
		{
			_quadCollection.Add(GetAMeshInstance(xform, duration, color ?? Colors.White,
				layers));
		}

		public void DrawPlane(Transform3D xform, float duration,
			Color? color, DebugLayers layers)
		{
			_planeCollection.Add(GetAMeshInstance(xform, duration,
				color ?? new Color(Colors.White, 0.5f), layers));
			DrawArrow(xform, duration, color ?? Colors.White, layers);
		}

		public void DrawCircle(Transform3D xform, float duration, Color? color, DebugLayers layers)
		{
			_circleCollection.Add(GetAMeshInstance(xform, duration, color ?? Colors.White,
				layers));
		}

		public void DrawAxes(Transform3D xform, float duration, DebugLayers layers)
		{
			_axesCollection.Add(GetAMeshInstance(xform, duration, new Color(1, 1, 1), layers));
		}

		public void DrawLine(Vector3 from, Vector3 to, float duration, Color? color,
			DebugLayers layers)
		{
			DebugLineInstance line = GetALineInstance(new[] {from, to}, duration, color, layers);
			if (line != null)
			{
				_lineInstances.Add(line);
			}
		}

		//Maybe add support for passing in a colour array?
		public void DrawLines(Vector3[] points, float duration, Color? color,
			DebugLayers layers)
		{
			DebugLineInstance line = GetALineInstance(points, duration, color, layers);
			if (line != null)
			{
				_lineInstances.Add(line);
			}
		}

		public void DrawRay(Vector3 from, Vector3 to, Vector3 hitLoc, bool hit, float duration,
			Color? rayColor, Color? hitColor, DebugLayers layers)
		{
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

		public void DrawArrow(Transform3D xform, float duration, Color? color, DebugLayers layers)
		{
			_arrowCollection.Add(GetAMeshInstance(xform, duration, color ?? Colors.White, layers));
		}
	}


	internal partial class DebugCanvasDrawer : GodotObject
	{
		private Font _textFont;
		private int _fontSize = 12;
		private int _screenEdgePadding = 16;
		private int _textEntryExtraPadding = 4;

		private readonly ObjectPool<DebugTextInstance> _textPool = new();

		private readonly Dictionary<string, DebugTextInstance>
			_textEntries = new();


		private readonly ObjectPool<DebugText3DInstance> _text3DPool = new();

		private readonly Dictionary<string, DebugText3DInstance>
			_text3dEntries = new();

		private Node _parent;

		private Node2D Canvas2D;
		private Node2D Canvas3D;

		public DebugCanvasDrawer(Node parent)
		{
			_parent = parent;

			Canvas2D = new Node2D();
			_parent.AddChild(Canvas2D);
			Canvas2D.ZIndex = 100;
			Canvas2D.Connect("draw", new Callable(this, nameof(DrawCanvas2D)));


			Canvas3D = new Node2D();
			_parent.AddChild(Canvas3D);
			Canvas3D.ZIndex = 101;
			Canvas3D.Connect("draw", new Callable(this, nameof(DrawCanvas3D)));

			//https://godotengine.org/qa/7307/getting-default-editor-font-for-draw_string
			var label = new Label();
			_textFont = label.GetThemeDefaultFont();
			label.Free();
		}

		public void DrawText(string key, string text, float duration, Color? color,
			DebugLayers layers)
		{
			string msg = $"{key}|{text}";

			if (_textEntries.ContainsKey(key))
			{
				if (_textEntries[key].Text != msg)
				{
					_textEntries[key].Text = msg;
					Canvas2D.QueueRedraw();
				}

				//DebugTextInstance entry = _texts[key];
				_textEntries[key].SetDuration(duration);
				_textEntries[key].Color = color ?? Colors.Gray;
				_textEntries[key].DrawLayers = layers;
			}
			else
			{
				DebugTextInstance inst = _textPool.Retrieve();
				if (inst != null)
				{
					inst.Text = msg;
					inst.SetDuration(duration);
					inst.Color = color ?? Colors.Gray;
					inst.DrawLayers = layers;
					Canvas2D.QueueRedraw();
					_textEntries.Add(key, inst);
				}
			}
		}

		public void DrawText3D(string key, string text, Vector3 location, float duration,
			Color? color, DebugLayers layers)
		{
			string msg = $"{key}|{text}";

			if (_text3dEntries.ContainsKey(key))
			{
				if (_text3dEntries[key].Text != msg)
				{
					_text3dEntries[key].Text = msg;
					Canvas2D.QueueRedraw();
				}


				//DebugTextInstance entry = _texts[key];
				_text3dEntries[key].SetDuration(duration);
				_text3dEntries[key].Color = color ?? Colors.Gray;
				_text3dEntries[key].DrawLayers = layers;
			}
			else
			{
				DebugText3DInstance inst = _text3DPool.Retrieve();
				if (inst != null)
				{
					inst.Text = msg;
					inst.Location = location;
					inst.SetDuration(duration);
					inst.Color = color ?? Colors.Gray;
					inst.DrawLayers = layers;
					_text3dEntries.Add(key, inst);
				}
			}
		}

		public void Update()
		{
			foreach (KeyValuePair<string, DebugTextInstance> entry in _textEntries)
			{
				if (entry.Value.IsExpired())
				{
					_textPool.Return(entry.Value);
					_textEntries.Remove(entry.Key);
					Canvas2D.QueueRedraw();
				}
			}

			//Always update 3d canvas
			Canvas3D.QueueRedraw();
			foreach (KeyValuePair<string, DebugText3DInstance> entry in _text3dEntries)
			{
				if (entry.Value.IsExpired())
				{
					_text3DPool.Return(entry.Value);
					_textEntries.Remove(entry.Key);
				}
			}
		}

		protected void DrawCanvas2D()
		{
			var pos = new Vector2(_screenEdgePadding, _screenEdgePadding + _fontSize * 1.5f);
			foreach (DebugTextInstance msg in _textEntries.Values)
			{
				Canvas2D.DrawString(_textFont, pos, msg.Text, HorizontalAlignment.Left, -1,
					_fontSize, msg.Color);

				pos.Y += _fontSize * 1.5f;
			}
		}

		protected void DrawCanvas3D()
		{
			Camera3D camera = Canvas3D.GetViewport().GetCamera3D();
			//3D
			foreach (DebugText3DInstance msg in _text3dEntries.Values)
			{
				Vector2 pos = camera.UnprojectPosition(msg.Location);
				Canvas3D.DrawString(_textFont, pos, msg.Text, HorizontalAlignment.Left, -1,
					_fontSize, msg.Color);
			}
		}
	}


	internal class DebugMeshCollection
	{
		public MultiMeshInstance3D MultiMeshInstance { get; }
		private readonly HashSet<DebugMeshInstance> _drawInstances = new();

		public static Action<DebugMeshInstance> OnInstanceRemoved;

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
			DebugMeshInstance[] expired =
				_drawInstances.Where(instance => instance.IsExpired()).ToArray();

			foreach (DebugMeshInstance instance in expired)
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
			foreach (DebugMeshInstance instance in _drawInstances)
			{
				MultiMeshInstance.Multimesh.SetInstanceTransform(i, instance.Transform);
				MultiMeshInstance.Multimesh.SetInstanceColor(i, instance.Color);
				instance.BeenDrawn = true;
				i++;
			}
		}

		public void Add(DebugMeshInstance instance)
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


	//Definitely better to have an object pool than to not,
	//but still have a a source of lots of single use objects somewhere to find.
	internal class ObjectPool<T> where T : IPoolable, new()
	{
		public readonly int MaxSize;
		public int CurrentSize;
		public int FreeObjects;

		private readonly Queue<T> _pool;

		public ObjectPool(int startingSize = DebugDraw.StartingPoolSize,
			int maxSize = DebugDraw.MaxPoolSize)
		{
			_pool = new Queue<T>();
			MaxSize = maxSize;
			ExpandPool(startingSize);
		}

		public T Retrieve()
		{
			if (FreeObjects == 0 && !ExpandPool(1))
			{
				GD.PrintErr(
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


	internal class DebugDrawInstance : IPoolable
	{
		public void SetDuration(float duration)
		{
			ExpirationTime = Time.GetTicksMsec() + (ulong)(duration * 1000.0f);
			BeenDrawn = false;
		}

		public Color Color;
		public bool BeenDrawn;
		protected ulong ExpirationTime;
		public DebugLayers DrawLayers;

		public virtual bool IsExpired()
		{
			return !DebugDraw.DrawingEnabled ||
			       ((DebugLayers)DebugDraw.EnabledLayers & DrawLayers) == 0 ||
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


	internal class DebugMeshInstance : DebugDrawInstance
	{
		public Transform3D Transform;

		public override void Reset()
		{
			base.Reset();
			Transform = default;
		}
	}


	internal class DebugTextInstance : DebugDrawInstance
	{
		public string Text;

		public override void Reset()
		{
			base.Reset();
			Text = default;
		}
	}


	internal class DebugText3DInstance : DebugTextInstance
	{
		public Vector3 Location;

		public override void Reset()
		{
			base.Reset();
			Location = default;
		}
	}


	internal class DebugLineInstance : DebugDrawInstance
	{
		public Vector3[] Points;

		public override void Reset()
		{
			base.Reset();
			Points = null;
		}
	}
}
