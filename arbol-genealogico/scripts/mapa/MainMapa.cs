using Godot;
using System;
using System.Collections.Generic;

public partial class MainMapa : Node2D
{
	

		
	private GrafoFamilia grafo;

	[Export] private Sprite2D MapaMundial;

	public override void _Ready()
	{
		// 1. Crear el grafo y llenarlo con miembros
		grafo = CrearGrafoDePrueba();
		var volverBtn = GetNode<Button>("Camera2d/Button_volver");
		volverBtn.Pressed += OnVolverPressed;

		// 2. Recorrer nodos del grafo
		foreach (FamilyMember miembro in grafo.ObtenerTodosLosMiembros())
		{
			GD.Print("Creando marcador para: " + miembro.Nombre);
			crearMarcador(miembro);
		}
	}

	private void crearMarcador(FamilyMember miembro)
	{
		var escena = GD.Load<PackedScene>("res://scenes/mapa/marcador.tscn");
		var instancia = escena.Instantiate();
		var marcador = instancia as Marcador;

		AddChild(marcador);
		Texture2D texturaFinal = null;

			// Si el miembro tiene foto
			if (!string.IsNullOrEmpty(miembro.FotoPath))
			{
				try
				{
					Image img = Image.LoadFromFile(miembro.FotoPath);
					texturaFinal = ImageTexture.CreateFromImage(img);
				}
				catch (Exception e)
				{
					GD.PrintErr("No se pudo cargar la foto de: ", miembro.Nombre, " - ", miembro.FotoPath);
					GD.PrintErr(e.Message);
				}
			}

		marcador.SetData(miembro.Nombre, texturaFinal);

		marcador.Position = ConvertirCordenadas(
			(float)miembro.Latitud,
			(float)miembro.Longitud
		);
	}

	private GrafoFamilia CrearGrafoDePrueba()
	{
		var g = new GrafoFamilia();

		var jean = new FamilyMember("1", "Jean", 9.9, -84.0,"C:/Users/jpaul/OneDrive/Im\u00E1genes/math cat.jpg");
		var juan = new FamilyMember("2", "Juan", 0, 0);
		
		var pepito = new FamilyMember("3", "Pepito", 60.7, 97.4);
		var islandia = new FamilyMember("4", "Islandia", 64.88, -18.19);

		// Añadir nodos y aristas
		g.AgregarArista(jean, juan);
		g.AgregarArista(juan, pepito);
		g.AgregarArista(pepito, islandia);

		return g;
	}

	private Vector2 ConvertirCordenadas(float lat, float lon)
	{
		float baseWidth = MapaMundial.Scale.X;
		float baseHeight = MapaMundial.Scale.Y;
		float width = MapaMundial.Texture.GetSize().X * baseWidth;
		float height = MapaMundial.Texture.GetSize().Y * baseHeight;

		Vector2 mapPos = MapaMundial.Position;

		float x = (lon + 180f) / 360f * width;
		float y = (90f - lat) / 180f * height;

		x -= width / 2f;
		y -= height / 2f;

		x += mapPos.X;
		y += mapPos.Y;

		return new Vector2(x, y);
	}
	
	private void OnVolverPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/PanelPrincipal.tscn");
	}
}
