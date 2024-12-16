using Godot;
using System;

public partial class ItemShown : VBoxContainer
{
	[Export] internal string name = "Undefined";
	[Export] internal AutoLoad.Quality quality = AutoLoad.Quality.Consumer;
	public override void _Ready()
	{
		GetNode<Label>("CenterContainer/Label").Text = name;
		GetNode<Label>("Label").Text = (quality == AutoLoad.Quality.RareSpecialItem ? "locRareSpecialItem" : name);
		GetNode<ColorRect>("HBoxContainer/ColorRect").Color = AutoLoad.QualityColor[quality];
		GetNode<Sprite2D>("CenterContainer/Sprite2D").Visible = (quality == AutoLoad.Quality.RareSpecialItem);
		GetNode<Label>("CenterContainer/Label").Visible = !(quality == AutoLoad.Quality.RareSpecialItem);
	}

	public override void _Process(double delta)
	{
	}
}
