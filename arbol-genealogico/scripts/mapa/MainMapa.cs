using Godot;
using System;
using System.Collections.Generic;

public partial class MainMapa : Node2D
{
		
	private GrafoFamilia grafo;
	private Marcador marcadorSeleccionado = null;
	private LineDrawer lineDrawer;
	[Export] private Sprite2D MapaMundial;

	public override void _Ready()
	{
		// cargar el nodo que dibuja las distancias entre personas
		 lineDrawer = GetNode<LineDrawer>("LineDrawer");
		// Crear el grafo y llenarlo con miembros
		grafo  = ConstruirGrafoDesdePersonas();
		var volverBtn = GetNode<Button>("Camera2d/Button_volver");
		volverBtn.Pressed += OnVolverPressed;

		// Recorrer nodos del grafo
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
		
		marcador.MarkerClicked += OnMarcadorClicked;

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
	private void OnMarcadorClicked(Marcador m)
	{
		// borra las lineas si clickea otra vez
		if (marcadorSeleccionado == m)
		{
			foreach (Node child in GetChildren())
			{
				if (child is Marcador other && other != m)
				{
					other.foto.Visible = true;
				}
			}
			
			marcadorSeleccionado.foto.Visible = true;
			marcadorSeleccionado = null;
			lineDrawer.ClearAll();
			return;
		}

		marcadorSeleccionado = m;		
		marcadorSeleccionado.foto.Visible = false;
		FamilyMember Objeto_Seleccionado = grafo.ObtenerMiembroPorNombre(m.ObtenerNombre());
		lineDrawer.ClearAll();

		// Obtener todos los marcadores del MainMapa
		foreach (Node child in GetChildren())
		{
			if (child is Marcador other && other != m)
			{
				Vector2 p1 = m.GlobalPosition;
				Vector2 p2 = other.GlobalPosition;
				FamilyMember Otro_Objeto_Seleccionado = grafo.ObtenerMiembroPorNombre(other.ObtenerNombre());
				other.foto.Visible = false;
				string texto = grafo.CalcularDistancia(Objeto_Seleccionado,Otro_Objeto_Seleccionado).ToString("0.#")+" metros";
				lineDrawer.DrawConnection(p1, p2, texto);
			}
		}
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
