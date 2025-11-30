using Godot;
using System;

public partial class ListaPersonas : Control
{
	private VBoxContainer _lista;

	public override void _Ready()
	{
		_lista = GetNode<VBoxContainer>("MarginContainer/VBoxContainer/ScrollContainer/PersonasList");

		foreach (var p in Main.Instance.Arbol.Personas)
		{
			var btn = new Button();
			btn.Text = $"{p.Nombre} ({p.Cedula})";
			btn.CustomMinimumSize = new Vector2(0, 45);
			btn.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

			btn.Pressed += () => OnPersonaSeleccionada(p);

			_lista.AddChild(btn);
		}

		GetNode<Button>("MarginContainer/VBoxContainer/VolverBtn")
			.Pressed += OnVolverPressed;
	}

	private void OnPersonaSeleccionada(Persona p)
	{
		var escena = (PackedScene)ResourceLoader.Load("res://scenes/DetallesPersona.tscn");
		var instancia = escena.Instantiate<DetallesPersona>();

		instancia.SetPersona(p);

		GetTree().Root.AddChild(instancia);

		QueueFree(); // cierra la pantalla actual
	}

	private void OnVolverPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/PanelPrincipal.tscn");
	}
}
