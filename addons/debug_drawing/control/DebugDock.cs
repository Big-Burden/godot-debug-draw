using Godot;

namespace Burden.DebugDrawing;

public partial class DebugDock : Control
{
	private CheckBox[] _layerChecks;
	private CheckBox _allCheck;
	private CheckBox _depthTestCheck;
	private GridContainer _checkGrid;

	private Key _toggleKey;

	public override void _Ready()
	{
		Hide();
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

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		base._UnhandledKeyInput(@event);

		//Keycode isn't playing ball on my uk keyboard
		if (@event is InputEventKey key && key.KeyLabel == _toggleKey && key.IsPressed()
		    && key.IsEcho() == false)
		{
			if (!Visible)
			{
				Show();
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			else
			{
				Hide();
				Input.MouseMode = Input.MouseModeEnum.Hidden;
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

			cb.ButtonPressed = (DebugDraw.EnabledLayers & (1 << (i + 1))) != 0;

			_checkGrid.AddChild(cb);
			_layerChecks[i] = cb;
		}
	}
	
	private void OnCheckPressed(int check)
	{
		DebugDraw.SetLayerEnabled(1 << (check + 1),
			_layerChecks[check].ButtonPressed);
	}

	private void RefreshButtonStates()
	{
		for (int i = 0; i < 8; i++)
		{
			_layerChecks[i].ButtonPressed = (DebugDraw.EnabledLayers & (1 << (i + 1))) != 0;
		}

		_depthTestCheck.ButtonPressed = DebugDraw.DoDepthTest;
		_allCheck.ButtonPressed = (DebugDraw.EnabledLayers == (int)DebugLayers.All);
	}


	private void ToggleAllLayers()
	{
		DebugDraw.SetEnabledLayers(_allCheck.ButtonPressed 
			? (int)DebugLayers.All : (int)DebugLayers.None);
	}

	private void SetDoDepthTest()
	{
		DebugDraw.SetDrawingDepthTestEnabled(_depthTestCheck.ButtonPressed);
	}
}
