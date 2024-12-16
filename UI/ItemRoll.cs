using Godot;
using System;

public partial class ItemRoll : VBoxContainer
{
	[Export] internal string name = "Undefined";
	[Export] internal AutoLoad.Quality quality = AutoLoad.Quality.Consumer;
	internal bool audioed = false;
	public override void _Ready()
	{
		GetNode<Label>("CenterContainer/Label").Text = name;
		GetNode<ColorRect>("ColorRect").Color = AutoLoad.QualityColor[quality];
		GetNode<Sprite2D>("CenterContainer/Cover").Modulate = AutoLoad.QualityColor[quality];
		GetNode<Sprite2D>("CenterContainer/Sprite2D").Visible = (quality == AutoLoad.Quality.RareSpecialItem);
		GetNode<Label>("CenterContainer/Label").Visible = !(quality == AutoLoad.Quality.RareSpecialItem);
	}

	public override void _Process(double delta)
	{
	}
}
