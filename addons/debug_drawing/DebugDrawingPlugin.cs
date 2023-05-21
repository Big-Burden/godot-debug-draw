#if TOOLS
using Godot;
using GC = Godot.Collections;
using System;

namespace Burden.DebugDrawing
{
	[Tool]
	public partial class DebugDrawingPlugin : EditorPlugin
	{
		private DebugDock _dock;

		private static DebugDraw _editorDebugDraw;
		private bool _enabledInEditor;

		private Callable _addEditorDebugCall;

		private const string _EnabledInEditorSettingName = "debug_drawing/editor/enableInEditor";

		public override void _EnterTree()
		{
			base._EnterTree();

			_addEditorDebugCall = new Callable(this, nameof(AddEditorDebugDrawToScene));
			ProjectSettingsChanged += OnProjectSettingsChanged;
			
			AddAutoloadSingleton("DebugDraw", "res://addons/debug_drawing/DebugDraw.cs");

			_dock = GD.Load<PackedScene>("res://addons/debug_drawing/control/DebugDock.tscn")
				.Instantiate<DebugDock>();
			_dock.SetPlugin(this);
			AddControlToDock(DockSlot.RightBl, _dock);
			
			string settingName = "debug_drawing/layers/layer_";

			for (int i = 0; i < 32; i++)
			{
				AddProjectSetting(settingName + (i + 1), Variant.Type.String, "");
			}
			
			AddProjectSetting(_EnabledInEditorSettingName, Variant.Type.Bool, false);

			bool settingEnabledInEditor = 
				(bool)ProjectSettings.GetSetting(_EnabledInEditorSettingName, false);
			if (settingEnabledInEditor)
			{
				_enabledInEditor = true;
				Connect("scene_changed", new Callable(this, nameof(AddEditorDebugDrawToScene)));
				AddEditorDebugDrawToScene(GetTree().EditedSceneRoot);
			}
		}
		
		private void OnProjectSettingsChanged()
		{
			bool settingEnableInEditor = 
				(bool)ProjectSettings.GetSetting(_EnabledInEditorSettingName, false);
			if (settingEnableInEditor && !_enabledInEditor)
			{
				_enabledInEditor = true;
				Connect("scene_changed", _addEditorDebugCall);
				AddEditorDebugDrawToScene(GetTree().EditedSceneRoot);
			}
			else
			{
				if (_enabledInEditor && !settingEnableInEditor)
				{
					_enabledInEditor = false;
					RemoveEditorDebugDraw();
					Disconnect("scene_changed", _addEditorDebugCall);
				}
			}
		}

		public override void _ExitTree()
		{
			base._ExitTree();

			RemoveControlFromDocks(_dock);
			_dock.Free();

			RemoveAutoloadSingleton("DebugDraw");
			_editorDebugDraw?.QueueFree();
			_editorDebugDraw = null;
		}

		private bool AddProjectSetting(string name, Variant.Type type, Variant initialValue)
		{
			if (ProjectSettings.HasSetting(name))
			{
				return false;
			}

			var dict = new GC.Dictionary();
			dict.Add("name", name);
			dict.Add("type", (int)type);

			ProjectSettings.Singleton.Set(name, initialValue);
			ProjectSettings.AddPropertyInfo(dict);
			ProjectSettings.SetInitialValue(name, initialValue);
			return true;
		}


		public void AddEditorDebugDrawToScene(Node sceneRoot)
		{
			//Delete the old one and make a new one, add it the the parent of the scene root
			RemoveEditorDebugDraw();
			
			if (sceneRoot == null)
			{
				return;
			}

			_editorDebugDraw = new DebugDraw();
			Node root = sceneRoot.GetParent();
			if (root != null)
			{
				_editorDebugDraw = new DebugDraw();
				root.AddChild(_editorDebugDraw);
				_editorDebugDraw.Owner = root;
				GD.Print($"added DebugDraw to {root.Name}");
			}
			else
			{
				GD.PrintErr("root is null, can't add DebugDraw!");
			}
		}

		private static void RemoveEditorDebugDraw()
		{
			if (_editorDebugDraw != null)
			{
				_editorDebugDraw.Free();
				_editorDebugDraw = null;
			}
		}
	}
}
#endif
