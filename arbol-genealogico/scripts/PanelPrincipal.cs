using Godot;
using System;

public partial class PanelPrincipal : Control
{
	public override void _Ready()
	{
		GD.Print("Pantalla principal cargada.");

		var btnAgregar = GetNode<Button>("MarginContainer/VBoxContainer/AgregarPersonaBtn");
		var btnVerPersonas = GetNode<Button>("MarginContainer/VBoxContainer/VerPersonasBtn");
		var btnArbol = GetNode<Button>("MarginContainer/VBoxContainer/VerArbolBtn");
		var btnMapa = GetNode<Button>("MarginContainer/VBoxContainer/VerMapaBtn");
		var btnEstadisticas = GetNode<Button>("MarginContainer/VBoxContainer/VerEstadisticasBtn");
		var btnSalir = GetNode<Button>("MarginContainer/VBoxContainer/SalirBtn");

		btnAgregar.Pressed += OnAgregarPersonaPressed;
		btnVerPersonas.Pressed += OnVerPersonasPressed;
		btnArbol.Pressed += OnVerArbolPressed;
		btnMapa.Pressed += OnVerMapaPressed;
		btnEstadisticas.Pressed += OnVerEstadisticasPressed;
		btnSalir.Pressed += OnSalirPressed;
	}

	private void OnAgregarPersonaPressed()
	{
		GD.Print("Ir a pantalla AgregarPersona");
		GetTree().ChangeSceneToFile("res://scenes/AgregarPersona.tscn");
	}

	private void OnVerPersonasPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/ListaPersonas.tscn");
	}

	private void OnVerArbolPressed()
	{
		GD.Print("Ir a vista del Árbol");
		GetTree().ChangeSceneToFile("res://scenes/TreeView.tscn");
	}

	private void OnVerMapaPressed()
	{
		GD.Print("Ir al mapa");
		GetTree().ChangeSceneToFile("res://scenes/mapa/mapa.tscn");
	}

	private void OnVerEstadisticasPressed()
	{
		GD.Print("Ir a estadísticas");
		GetTree().ChangeSceneToFile("res://scenes/Estadisticas.tscn");
	}

	private void OnSalirPressed()
	{
		GetTree().Quit();
	}
}
