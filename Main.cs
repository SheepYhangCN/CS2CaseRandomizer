using Godot;
using GodotTask;
using System;

public partial class Main : Control
{
	PackedScene item = GD.Load<PackedScene>("res://UI/ItemShown.tscn");
	PackedScene roll = GD.Load<PackedScene>("res://UI/Rolling.tscn");
	public override void _Ready()
	{
		var autoload = GetNode<AutoLoad>("/root/AutoLoad");
		var items = GetNode<HFlowContainer>("MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/ScrollContainer/HFlowContainer");
		foreach (var i in autoload.CaseItemList)
		{
			var item_ = item.Instantiate<ItemShown>();
			items.AddChild(item_);
			item_.name = i.Key;
			item_.quality = i.Value;
			item_._Ready();
		}
		GetNode<RichTextLabel>("MarginContainer/VBoxContainer/UnlockCase").Text = "[center]"+TranslationServer.Translate("locUnlockCase").ToString().Replace("{Case}","[b]"+autoload.CaseName+"[/b]")+"[/center]";
		GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
		if (FileAccess.FileExists(AutoLoad.GetGameDirPath("CaseLogo.png")))
		{
			GetNode<Sprite3D>("Background/Sprite3D").Texture = ImageTexture.CreateFromImage(Image.LoadFromFile(AutoLoad.GetGameDirPath("CaseLogo.png")));
		}
		if (FileAccess.FileExists(AutoLoad.GetGameDirPath("CaseLogo.json")))
		{
			var file = FileAccess.Open(AutoLoad.GetGameDirPath("CaseLogo.json"),FileAccess.ModeFlags.Read);
			var json = Json.ParseString(file.GetAsText()).AsGodotDictionary<string,float>();
			file.Close();
			GetNode<Sprite3D>("Background/Sprite3D").Scale = new Vector3(json["scale_x"],json["scale_y"],json["scale_z"]);
		}
	}

	public override void _Process(double delta)
	{
	}

	public void _on_settings_pressed()
	{
		GetTree().CurrentScene.AddChild(GD.Load<PackedScene>("res://UI/Settings.tscn").Instantiate<Settings>());
	}

	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}
	
	public async void _on_unlock_pressed()
	{
		GetNode<Button>("MarginContainer/VBoxContainer/Bottom/Unlock").Disabled = true;
		MoveChild(GetNode<Sprite2D>("Case"),GetNode<MarginContainer>("MarginContainer").GetIndex());
		GetNode<AnimationPlayer>("Background/AnimationPlayer").Play("unlock");
		await GDTask.Delay(100);
		var player = new AudioStreamPlayer();
		GetTree().CurrentScene.AddChild(player);
		player.Stream = GD.Load<AudioStream>("res://Assets/Sounds/case_unlock.wav");
		player.Play();
		await ToSignal(player,"finished");
		player.QueueFree();
		GetTree().CurrentScene.AddChild(roll.Instantiate<Rolling>());
	}
}