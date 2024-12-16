using Godot;
using System;

public partial class Result : Control
{
	[Export] internal string name = "Undefined";
	[Export] internal AutoLoad.Quality quality = AutoLoad.Quality.Consumer;
	public override void _Ready()
	{
		var autoload = GetNode<AutoLoad>("/root/AutoLoad");
		GetNode<Label>("MarginContainer/VBoxContainer/CaseName").Text = autoload.CaseName;
		GetNode<Label>("MarginContainer/VBoxContainer/UnlockContainer").Text = name;
		GetNode<Label>("MarginContainer/VBoxContainer/HBoxContainer2/VBoxContainer/PanelContainer/Label").Text = name;
		GetNode<ColorRect>("MarginContainer/VBoxContainer/HBoxContainer/Quality").Color = AutoLoad.QualityColor[quality];
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Stream = GD.Load<AudioStream>(AutoLoad.QualitySound[quality]);
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
		GetNode<AnimationPlayer>("AnimationPlayer").Play("fade_in");
	}


	public override void _Process(double delta)
	{
	}

	public void _on_close_pressed()
	{
		//QueueFree();
		GetTree().ReloadCurrentScene();
	}
}
