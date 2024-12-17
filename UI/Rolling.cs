using Godot;
using System;
using SheepYhangCN.RandomExtension;
using System.Linq;
using Kryz.Tweening;
using GodotTask;

public partial class Rolling : Control
{
	PackedScene item = GD.Load<PackedScene>("res://UI/ItemRoll.tscn");
	static readonly int speed_base = 20;
	int speed_start = 20;
	int speed = 20;
	double timer = 0;
	double played = 0;
	bool over = false;
	public override void _Ready()
	{
		speed_start = Mathf.RoundToInt(speed_base/(Engine.GetFramesPerSecond()/120));
		speed = speed_start;
		GD.Print("FPS: "+Engine.GetFramesPerSecond().ToString(),", Speed: "+speed.ToString());
		var autoload = GetNode<AutoLoad>("/root/AutoLoad");
		var random = new Random();
		GetNode<RichTextLabel>("SubViewportContainer/SubViewport/Control/MarginContainer/VBoxContainer/UnlockCase").Text = "[center]"+TranslationServer.Translate("locUnlockCase").ToString().Replace("{Case}","[b]"+autoload.CaseName+"[/b]")+"[/center]";
		for (var i = 0; i < 100; i += 1)
		{
			var inst = item.Instantiate<ItemRoll>();
			inst.name = random.Choose(autoload.CaseItemList.Keys.ToArray());
			inst.quality = autoload.CaseItemList[inst.name];
			GetNode<HBoxContainer>("SubViewportContainer/SubViewport/Control/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/ScrollContainer/HBoxContainer").AddChild(inst);
		}
		GetNode<AnimationPlayer>("AnimationPlayer").Play("fade_in");
	}

	public override async void _Process(double delta)
	{
		timer += delta;
		if (timer >= 5)
		{
			GetNode<Button>("SubViewportContainer/SubViewport/Control/MarginContainer/VBoxContainer/Bottom/Skip").Disabled = true;
		}
		var scroll = GetNode<ScrollContainer>("SubViewportContainer/SubViewport/Control/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/ScrollContainer");
		scroll.ScrollHorizontal += speed;
		speed = Math.Clamp(Mathf.RoundToInt(speed_start - speed_start * EasingFunctions.OutCubic((float)((timer-3d)/4d))),0,speed_start);
		foreach (var i in GetNode<HBoxContainer>("SubViewportContainer/SubViewport/Control/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/ScrollContainer/HBoxContainer").GetChildren())
		{
			if (i is ItemRoll item && item.GlobalPosition.X <= 0 && !item.audioed)
			{
				item.audioed = true;
				GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
			}
		}
		string name = null;
		if (speed <= 0 && !over)
		{
			over = true;
			foreach (var i in GetNode<HBoxContainer>("SubViewportContainer/SubViewport/Control/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/ScrollContainer/HBoxContainer").GetChildren())
			{
				if (i is ItemRoll item && item.GlobalPosition.X <= 960 && item.GlobalPosition.X >= 960-320)
				{
					GD.Print("First success: "+item.name+" "+item.GlobalPosition.X.ToString());
					name = item.name;
					break;
				}
			}
			if (name == null)
			{
				foreach (var i in GetNode<HBoxContainer>("SubViewportContainer/SubViewport/Control/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/ScrollContainer/HBoxContainer").GetChildren())
				{
					if (i is ItemRoll item && item.GlobalPosition.X <= 960+16 && item.GlobalPosition.X >= 960-320-16)
					{
						GD.Print("Second success: "+item.name+" "+item.GlobalPosition.X.ToString());
						name = item.name;
						break;
					}
				}
			}
			await GDTask.Delay(100);
			var autoload = GetNode<AutoLoad>("/root/AutoLoad");
			var result = GD.Load<PackedScene>("res://UI/Result.tscn").Instantiate<Result>();
			result.name = name;
			result.quality = autoload.CaseItemList[result.name];
			GetTree().CurrentScene.AddChild(result);
			QueueFree();
		}
	}

	public void _on_skip_pressed()
	{
		GetNode<Button>("SubViewportContainer/SubViewport/Control/MarginContainer/VBoxContainer/Bottom/Skip").Disabled = true;
		timer = 5;
		GetNode<ScrollContainer>("SubViewportContainer/SubViewport/Control/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/ScrollContainer").ScrollHorizontal += speed * 60;
		foreach (var i in GetNode<HBoxContainer>("SubViewportContainer/SubViewport/Control/MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/ScrollContainer/HBoxContainer").GetChildren())
		{
			if (i is ItemRoll item && item.GlobalPosition.X <= 0 && !item.audioed)
			{
				item.audioed = true;
			}
		}
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Stop();
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
	}
}
