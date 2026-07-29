using System;
using Godot;

public class MapBattleUI : Node
{
	[Export]
	public Button fightButton;

	[Export]
	public Button infoButton;

	[Export]
	public Label battleNameLabel;

	[Export]
	public Label difficultyLabel;

	[Export]
	public Node battleInfoPanel;

	public void ShowBattleInfo(object battleData)
	{
		GD.Print("STUB: MapBattleUI.ShowBattleInfo");
	}

	public void Hide()
	{
		Visible = false;
	}

	public void Show()
	{
		Visible = true;
	}
}
