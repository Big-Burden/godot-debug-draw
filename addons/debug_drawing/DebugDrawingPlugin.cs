using Godot;
using GC = Godot.Collections;

#if TOOLS
namespace Burden.DebugDrawing;

[Tool]
public partial class DebugDrawingPlugin : EditorPlugin
{
	private static DebugDraw _editorDebugDraw;
	public bool EnabledInEditor { get; private set; }

	private Callable _addEditorDebugCall;

	public const string EnabledInEditorOption = "debug_drawing/editor/enableInEditor";
	public const string ToggleKeyOption = "debug_drawing/settings_toggle_key";
	public const string MaxPoolSizeOption = "debug_drawing/MaxPoolSize";
	public const string StartingPoolSizeOption = "debug_drawing/StartingPoolSize";


	public override void _EnterTree()
	{
		
		
		base._EnterTree();

		_addEditorDebugCall = new Callable(this, nameof(AddEditorDebugDrawToScene));

		AddAutoloadSingleton("DebugDraw", "res://addons/debug_drawing/DebugDraw.cs");

		//Add plugin settings

		const string settingName = "debug_drawing/layers/layer_";

		for (int i = 0; i < 8; i++)
		{
			AddProjectSetting(settingName + (i + 1), Variant.Type.String, i + 1);
		}
		
		// AddProjectSetting(EnabledInEditorOption, Variant.Type.Bool, false);
		AddProjectSetting(ToggleKeyOption, Variant.Type.Int, (int)Key.Apostrophe);

		AddProjectSetting(MaxPoolSizeOption, Variant.Type.Int, 1024);
		AddProjectSetting(StartingPoolSizeOption, Variant.Type.Int, 256);

		bool settingEnabledInEditor =
			(bool)ProjectSettings.GetSetting(EnabledInEditorOption, false);
		if (settingEnabledInEditor)
		{
			EnabledInEditor = true;
			Connect("scene_changed", new Callable(this, nameof(AddEditorDebugDrawToScene)));
			AddEditorDebugDrawToScene(GetTree().EditedSceneRoot);
		}

		ProjectSettings.Save();
		ProjectSettingsChanged += OnProjectSettingsChanged;
	}


	private void OnProjectSettingsChanged()
	{
		bool settingEnableInEditor =
			(bool)ProjectSettings.GetSetting(EnabledInEditorOption, false);
		if (settingEnableInEditor && !EnabledInEditor)
		{
			EnabledInEditor = true;
			Connect("scene_changed", _addEditorDebugCall);
			AddEditorDebugDrawToScene(GetTree().EditedSceneRoot);
		}
		else
		{
			if (EnabledInEditor && !settingEnableInEditor)
			{
				EnabledInEditor = false;
				RemoveEditorDebugDraw();
				Disconnect("scene_changed", _addEditorDebugCall);
			}
		}
	}


	public override void _ExitTree()
	{
		base._ExitTree();

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
			GD.PushError("root is null, can't add DebugDraw!");
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
#endif //TOOLS
