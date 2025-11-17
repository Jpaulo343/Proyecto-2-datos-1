using Godot;
using System;

public partial class Main : Node
{
	public override void _Ready()
	{
		GD.Print("Escena cargada.");

		var arbol = new ArbolGenealogico();

		var juan = new Persona("Juan", "101");
		var ana = new Persona("Ana", "102");
		var pedro = new Persona("Pedro", "103");

		arbol.AgregarFamiliar(juan);
		arbol.AgregarFamiliar(ana);
		arbol.AgregarFamiliar(pedro, padre: juan, madre: ana);

		GD.Print($"Total personas: {arbol.Personas.Count}");
		GD.Print($"Padre de Pedro: {pedro.Padre.Nombre}");
		GD.Print($"Madre de Pedro: {pedro.Madre.Nombre}");
		GD.Print($"Hijos de Juan: {juan.Hijos.Count}");
	}
}
