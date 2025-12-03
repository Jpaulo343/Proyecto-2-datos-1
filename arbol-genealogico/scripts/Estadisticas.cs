using Godot;
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

		// IMPORTANTE: convertir a List para poder usar .Count y [i]
		var miembros = new List<FamilyMember>(grafo.ObtenerTodosLosMiembros());

		if (miembros.Count < 2)
		{
			_masLejosLabel.Text  = "Par más lejano: No hay suficientes familiares con coordenadas.";
			_masCercaLabel.Text  = "Par más cercano: No hay suficientes familiares con coordenadas.";
			_promedioLabel.Text  = "Distancia promedio: No hay suficientes familiares con coordenadas.";
			return;
		}

		var parLejano  = grafo.ParMasLejano();     // (FamilyMember, FamilyMember, double)
		var parCercano = grafo.ParMasCercano();    // (FamilyMember, FamilyMember, double)
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
		var mapaCedulaMiembro = new Dictionary<string, FamilyMember>();

		foreach (var persona in Main.Instance.Arbol.Personas)
		{
			// Coordenadas es un tuple (double lat, double lon), no puede ser null.
			// Si quieres saltar personas "sin coordenadas", usa una condición como esta:
			var coords = persona.Coordenadas;

			// Ajusta esto si 0,0 puede ser una coordenada válida para vos.
			if (coords.lat == 0 && coords.lon == 0)
				continue;

			var fm = new FamilyMember(
				persona.Cedula,
				persona.Nombre,
				coords.lat,
				coords.lon,
				persona.FotoPath
			);

			grafo.AgregarNodo(fm);
			mapaCedulaMiembro[persona.Cedula] = fm;
		}

		// Grafo completo: todos conectados con todos
		var miembros = new List<FamilyMember>(grafo.ObtenerTodosLosMiembros());
		for (int i = 0; i < miembros.Count; i++)
		{
			for (int j = i + 1; j < miembros.Count; j++)
			{
				grafo.AgregarArista(miembros[i], miembros[j]);
			}
		}

		return grafo;
	}
}
