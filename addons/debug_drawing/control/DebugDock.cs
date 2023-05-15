#if TOOLS
using Godot;

namespace Burden.DebugDrawing
{
	[Tool]
	public partial class DebugDock : Control
	{
		public override void _Ready()
		{
			
		}
		
		private DebugDrawingPlugin _plugin;
		
		public void SetPlugin(DebugDrawingPlugin plugin)
		{
			_plugin = plugin;
			GetNode<Button>("HBoxContainer/TestDrawBtn")
				.Connect("pressed", new Callable(this, nameof(TestDraw)));
		}

		private void TestDraw()
		{
			DebugDraw.DrawCube(Transform3D.Identity, Vector3.One, 5.0f, Colors.Blue);
			GD.Print("build");
		}
	}
}
#endif
