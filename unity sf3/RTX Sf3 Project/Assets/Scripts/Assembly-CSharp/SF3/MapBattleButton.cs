using System;
using Godot;

public class MapBattleButton : Node
{
	[Export]
	public Button button;

	[Export]
	public Label label;

	[Export]
	public TextureRect icon;

	[Export]
	public Node lockedOverlay;

	public int BattleId { get; private set; }

	public void Init(int battleId, string name)
	{
		BattleId = battleId;
		label.Text = name;
		GD.Print("STUB: MapBattleButton.Init");
	}

	public void SetLocked(bool locked)
	{
		if (lockedOverlay != null)
			lockedOverlay.Visible = locked;
		button.Disabled = locked;
	}

	public void SetSelected(bool selected)
	{
		GD.Print("STUB: MapBattleButton.SetSelected: " + selected);
	}
}
