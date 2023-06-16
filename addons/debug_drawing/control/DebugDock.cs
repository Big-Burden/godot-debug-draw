using Godot;

#if TOOLS
namespace Burden.DebugDrawing;

public partial class DebugDock : Control
{
	private CheckBox[] _layerChecks;
	private CheckBox _allCheck;
	private CheckBox _depthTestCheck;
	private GridContainer _checkGrid;

	private Label[] _poolLabels = new Label[4];

	private Key _toggleKey;


	public override void _Ready()
	{
		Hide();

		_poolLabels[0] =
			GetNode<Label>("MarginContainer/VBoxContainer/HBoxMeshPool/MeshPoolDataLabel");
		_poolLabels[1] =
			GetNode<Label>("MarginContainer/VBoxContainer/HBoxLinePool/LinePoolDataLabel");
		_poolLabels[2] =
			GetNode<Label>("MarginContainer/VBoxContainer/HBoxTextPool/TextPoolDataLabel");
		_poolLabels[3] =
			GetNode<Label>("MarginContainer/VBoxContainer/HBoxText3DPool/Text3DPoolDataLabel");


		if (!Engine.IsEditorHint())
		{
			ConstructLayerCheckBoxes();

			_toggleKey = (Key)(int)ProjectSettings.GetSetting(DebugDrawingPlugin.ToggleKeyOption,
				(int)Key.Quoteleft);
		}

		_allCheck = GetNode<CheckBox>("MarginContainer/VBoxContainer/HBoxContainer/AllCheck");
		_allCheck.Pressed += ToggleAllLayers;


		_depthTestCheck = GetNode<CheckBox>("MarginContainer/VBoxContainer/DepthTestCheck");
		_depthTestCheck.Pressed += SetDoDepthTest;


		DebugDraw.OnDrawSettingsUpdated += RefreshButtonStates;
		RefreshButtonStates();
	}


	public override void _Process(double delta)
	{
		base._Process(delta);
		Vector3I[] pools = DebugDraw.GetPoolSizes();
		for (int i = 0; i < pools.Length; i++)
		{
			Vector3I pool = pools[i];
			_poolLabels[i].Text = $"{pool.X}/{pool.Y} ({pool.Z})";
		}
	}


	public override void _UnhandledKeyInput(InputEvent @event)
	{
		base._UnhandledKeyInput(@event);

		//Keycode isn't playing ball on my uk keyboard
		if (@event is InputEventKey key && key.KeyLabel == _toggleKey && key.IsPressed()
		    && key.IsEcho() == false)
		{
			if (!Visible)
			{
				//ProcessMode = ProcessModeEnum.Always;
				Show();
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			else
			{
				Hide();
				Input.MouseMode = Input.MouseModeEnum.Captured;
				//ProcessMode = ProcessModeEnum.Disabled;
			}
		}
	}


	private void ConstructLayerCheckBoxes()
	{
		_checkGrid = GetNode<GridContainer>("MarginContainer/VBoxContainer/LayerChecks");
		_layerChecks = new CheckBox[8];
		for (int i = 0; i < 8; i++)
		{
			var cb = new CheckBox();
			cb.Text = (string)ProjectSettings.GetSetting("debug_drawing/layers/layer_" + (i + 1),
				i + 1);

			int checkId = i;
			cb.Pressed += () => OnCheckPressed(checkId);
			cb.ButtonPressed = (DebugDraw.GetEnabledLayers() & (1 << (i + 1))) != 0;

			_checkGrid.AddChild(cb);
			_layerChecks[i] = cb;
		}
	}


	private void OnCheckPressed(int check)
	{
		DebugDraw.SetLayerEnabled((uint)(1 << (check + 1)),
			_layerChecks[check].ButtonPressed);
	}


	private void RefreshButtonStates()
	{
		for (int i = 0; i < 8; i++)
		{
			_layerChecks[i].ButtonPressed = (DebugDraw.GetEnabledLayers() & (1 << (i + 1))) != 0;
		}

		_depthTestCheck.ButtonPressed = DebugDraw.GetDoDepthTest();
		_allCheck.ButtonPressed = DebugDraw.GetEnabledLayers() == (uint)DebugLayers.All;
	}


	private void ToggleAllLayers()
	{
		DebugDraw.SetEnabledLayers(_allCheck.ButtonPressed
			? (uint)DebugLayers.All
			: (uint)DebugLayers.None);
	}


	private void SetDoDepthTest()
	{
		DebugDraw.SetDrawingDepthTestEnabled(_depthTestCheck.ButtonPressed);
	}
}
#endif //TOOLS
