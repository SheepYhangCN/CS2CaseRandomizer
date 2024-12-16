using Godot;
using Godot.Collections;
using GodotTask;

public partial class Settings : Control
{
	static readonly Array<string> langs = new(){"en","zh_CN","zh_TW"};
	public override void _Ready()
	{
		//var Global = GetTree().CurrentScene.GetNode<Main>("Main");
		GetNode<OptionButton>("CenterContainer/VBoxContainer/HSplitContainer/OptionButton").Selected = langs.IndexOf(TranslationServer.GetLocale());
		GetNode<OptionButton>("CenterContainer/VBoxContainer/HSplitContainer2/OptionButton").Selected = (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen ? 1 : 0);
	}

	public override void _Process(double delta)
	{
	}

	/*public async void _on_option_button_item_selected(int selected)
	{
		TranslationServer.SetLocale(langs[selected]);
		GetTree().ReloadCurrentScene();
		await GDTask.Delay(1000);
		GetTree().CurrentScene.GetNode<Main>("Main")._on_settings_pressed();
	}*/

	public void _on_back_pressed()
	{
		var lang = langs[GetNode<OptionButton>("CenterContainer/VBoxContainer/HSplitContainer/OptionButton").Selected];
		var cfg = new ConfigFile();
		cfg.SetValue("Settings","Language",lang);
		cfg.SetValue("Settings","CaseName",GetNode<AutoLoad>("/root/AutoLoad").CaseName);
		cfg.SetValue("Settings","WindowMode",(int)DisplayServer.WindowMode.Windowed);
		cfg.Save("user://Settings.ini");
		TranslationServer.SetLocale(lang);
		//QueueFree();
		GetTree().ReloadCurrentScene();
	}

	public void _on_import_pressed()
	{
		/*var filediag = new FileDialog();
		filediag.Access = FileDialog.AccessEnum.Filesystem;
		filediag.UseNativeDialog = true;
		filediag.FileMode = FileDialog.FileModeEnum.OpenFile;
		filediag.AddFilter("*.csv");
		filediag.Visible = true;
		filediag.FileSelected += selected;*/
		GetNode<FileDialog>("FileDialog").Visible = true;
	}

	public void _on_option_button_window_item_selected(int selected)
	{
		if (selected == 0)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
		else if (selected == 1)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
	}

	public void _on_github_pressed()
	{
		OS.ShellOpen("https://github.com/SheepYhangCN/CS2CaseRandomizer");
	}

	public void selected(string path)
	{
		GD.Print(path);
		var autoload = GetNode<AutoLoad>("/root/AutoLoad");
		autoload.CaseItemList = AutoLoad.LoadToItems(path);
		autoload.CaseName = path.TrimSuffix(".csv").Replace("\\","/").Split("/")[^1];
	}
}
