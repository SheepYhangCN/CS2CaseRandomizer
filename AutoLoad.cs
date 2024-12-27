using System;
using Godot;
using Godot.Collections;

public partial class AutoLoad : Node
{
	internal Dictionary<string,Quality> CaseItemList;
	internal string CaseName;
	internal enum Quality
	{
		Consumer,
		MilSpec,
		Industrial,
		Restricted,
		Classified,
		Covert,
		RareSpecialItem
	}
	internal static readonly Dictionary<Quality,Color> QualityColor = new()
	{
		{Quality.Consumer,new Color(0.8f,0.8f,0.8f,1f)},
		{Quality.MilSpec,new Color(0.3f,0.4f,1f,1f)},
		{Quality.Industrial,new Color(0.4f,0.6f,0.8f,1f)},
		{Quality.Restricted,new Color(0.5f,0.3f,1f,1f)},
		{Quality.Classified,new Color(0.8f,0.2f,0.9f,1f)},
		{Quality.Covert,new Color(0.9f,0.3f,0.3f,1f)},
		{Quality.RareSpecialItem,new Color(0.9f,0.7f,0.2f,1f)}
	};
	internal static readonly Dictionary<Quality,string> QualitySound = new()
	{
		{Quality.Consumer,"res://Assets/Sounds/case_awarded_common.wav"},
		{Quality.MilSpec,"res://Assets/Sounds/case_awarded_rare.wav"},
		{Quality.Industrial,"res://Assets/Sounds/case_awarded_uncommon.wav"},
		{Quality.Restricted,"res://Assets/Sounds/case_awarded_mythical.wav"},
		{Quality.Classified,"res://Assets/Sounds/case_awarded_legendary.wav"},
		{Quality.Covert,"res://Assets/Sounds/case_awarded_ancient.wav"},
		{Quality.RareSpecialItem,"res://Assets/Sounds/case_awarded_ancient.wav"}
	};
	public override void _Ready()
	{
		CaseItemList = new()
		{
			{"Test Item 1",Quality.Consumer},
			{"Test Item 2",Quality.MilSpec},
			{"Test Item 3",Quality.Industrial},
			{"Test Item 4",Quality.Restricted},
			{"Test Item 5",Quality.Classified},
			{"Test Item 6",Quality.Covert},
			{"Test Item 11",Quality.Consumer},
			{"Test Item 22",Quality.MilSpec},
			{"Test Item 33",Quality.Industrial},
			{"Test Item 44",Quality.Restricted},
			{"Test Item 55",Quality.Classified},
			{"Test Item 66",Quality.Covert},
			{"Roll again",Quality.RareSpecialItem}
		};
		CaseName = "Undefined Weapon Case";
		if (OS.GetLocale() == "zh_TW" || OS.GetLocale() == "zh_HK" || OS.GetLocale() == "zh_MO")
		{
			TranslationServer.SetLocale("zh_TW");
		}
		else if (OS.GetLocaleLanguage() == "zh")
		{
			TranslationServer.SetLocale("zh_CN");
		}
		else
		{
			TranslationServer.SetLocale("en");
		}
		if (FileAccess.FileExists("user://Settings.ini"))
		{
			var cfg = new ConfigFile();
			cfg.Load("user://Settings.ini");
			TranslationServer.SetLocale(cfg.GetValue("Settings","Language","en").AsString());
			CaseName = cfg.GetValue("Settings","CaseName","Undefined Weapon Case").AsString();
			DisplayServer.WindowSetMode((DisplayServer.WindowMode)(cfg.GetValue("Settings","WindowMode",(int)DisplayServer.WindowMode.Windowed).AsInt32()));
			CaseItemList = LoadToItems("user://Case.csv",true);
		}
		Engine.MaxFps = Mathf.CeilToInt(DisplayServer.ScreenGetRefreshRate(DisplayServer.WindowGetCurrentScreen()));
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("fullscreen"))
		{
			if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed || DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Maximized || DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Minimized)
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			}
			else
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			}
		}
		if (Input.IsActionJustPressed("debug_reboot"))
		{
			_Ready();
			GetTree().ReloadCurrentScene();
		}
	}

	internal static Dictionary<string,Quality> LoadToItems(string path, bool from_saved = false)
	{
		var file = FileAccess.Open(path,FileAccess.ModeFlags.Read);
		FileAccess save = null;
		if (!from_saved){save = FileAccess.Open("user://Case.csv",FileAccess.ModeFlags.Write);}
		var result = new Dictionary<string,Quality>();
		var lines=file.GetAsText(true).Split("\n");
		foreach (var i in lines)
		{
			var line = file.GetCsvLine();
			if (i != "")
			{
				if (!from_saved){save.StoreCsvLine(line);}
				result[line[0]] = (Quality)(line[1].ToInt());
			}
		}
		file.Close();
		if (!from_saved){save.Close();}
		return result;
	}
	internal static string GetGameDirPath(string str)
	{
		if (OS.HasFeature("editor"))
		{
			return ProjectSettings.GlobalizePath("res://"+str);
		}
		else
		{
			return OS.GetExecutablePath().GetBaseDir()+"/"+str;
		}
	}
}
