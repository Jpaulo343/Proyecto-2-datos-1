using Godot;
using System;
using System.Collections.Generic; 

public partial class Estadisticas : Control
{
	private Label _masLejosLabel;
	private Label _masCercaLabel;
	private Label _promedioLabel;
	private Button _volverBtn;

	public override void _Ready()
	{
		_masLejosLabel = GetNode<Label>("Contenido/MasLejosLabel");
		_masCercaLabel = GetNode<Label>("Contenido/MasCercaLabel");
		_promedioLabel = GetNode<Label>("Contenido/PromedioLabel");
		_volverBtn = GetNode<Button>("Contenido/VolverBtn");

		_volverBtn.Pressed += OnVolverPressed;

		MostrarEstadisticas();
	}

	private void OnVolverPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/PanelPrincipal.tscn");
	}

	private void MostrarEstadisticas()
	{
		var grafo = ConstruirGrafoDesdePersonas();


		int count = 0;
		foreach (var _ in grafo.ObtenerTodosLosMiembros())
		{
			count++;
		}

		if (count < 2)
		{
			_masLejosLabel.Text  = "Par más lejano: No hay suficientes familiares con coordenadas.";
			_masCercaLabel.Text  = "Par más cercano: No hay suficientes familiares con coordenadas.";
			_promedioLabel.Text  = "Distancia promedio: No hay suficientes familiares con coordenadas.";
			return;
		}

		var parLejano  = grafo.ParMasLejano();  
		var parCercano = grafo.ParMasCercano();  
		double promedio = grafo.DistanciaPromedio();

		_masLejosLabel.Text =
			$"Par más lejano: {parLejano.Item1.Nombre} y {parLejano.Item2.Nombre} ({parLejano.Item3:F2} km)";

		_masCercaLabel.Text =
			$"Par más cercano: {parCercano.Item1.Nombre} y {parCercano.Item2.Nombre} ({parCercano.Item3:F2} km)";

		_promedioLabel.Text =
			$"Distancia promedio entre familiares: {promedio:F2} km";
	}

	private GrafoFamilia ConstruirGrafoDesdePersonas()
	{
		var grafo = new GrafoFamilia();

		foreach (var persona in Main.Instance.Arbol.Personas.Enumerar())
		{
			if (persona.Latitud == 0 && persona.Longitud == 0)
				continue;

			var fm = new FamilyMember(
				persona.Cedula,
				persona.Nombre,
				persona.Latitud,
				persona.Longitud,
				persona.FotoPath
			);

			grafo.AgregarNodo(fm);
		}

		IEnumerable<FamilyMember> miembrosEnum = grafo.ObtenerTodosLosMiembros();

		int total = 0;
		foreach (var _ in miembrosEnum)
			total++;

		FamilyMember[] miembros = new FamilyMember[total];
		int k = 0;
		foreach (var m in grafo.ObtenerTodosLosMiembros())
		{
			miembros[k++] = m;
		}

		for (int i = 0; i < total; i++)
		{
			for (int j = i + 1; j < total; j++)
			{
				grafo.AgregarArista(miembros[i], miembros[j]);
			}
		}

		return grafo;
	}
}
