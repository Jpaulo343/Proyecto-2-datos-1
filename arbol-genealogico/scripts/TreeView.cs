using Godot;
using System;
using System.Collections.Generic;

public partial class TreeView : Control
{
	private VBoxContainer _container;

	public override void _Ready()
	{
		_container = GetNode<VBoxContainer>("MarginContainer/ScrollContainer/Content");
		GetNode<Button>("VolverBtn").Pressed += OnVolverPressed;
		ActualizarArbol();
	}

	private void OnVolverPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/PanelPrincipal.tscn");
	}

	public void ActualizarArbol()
	{
		foreach (Node child in _container.GetChildren())
			child.QueueFree();

		var raiz = Main.Instance.Arbol.Raiz;
		if (raiz == null)
		{
			GD.Print("Árbol vacío");
			return;
		}

		DibujarNodo(raiz, 0);
	}

	private void DibujarNodo(FamilyMember persona, int nivel)
	{
		if (persona == null) return;

		// Crear el panel del nodo
		var panel = CrearPanelPersona(persona, nivel);

		// Si queremos indentar según el nivel, creamos una fila (HBox)
		// y añadimos un spacer (Control) con ancho = nivel * OFFSET
		const int OFFSET_POR_NIVEL = 60;
		var fila = new HBoxContainer();

		if (nivel > 0)
		{
			var spacer = new Control();
			spacer.CustomMinimumSize = new Vector2(nivel * OFFSET_POR_NIVEL, 0);
			// asegurar que el spacer no se colapse
			spacer.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
			fila.AddChild(spacer);
		}

		fila.AddChild(panel);
		_container.AddChild(fila);

		// Obtener vecinos/hijos desde tu grafo (ajusta el método si se llama distinto)
		var hijos = Main.Instance.Arbol.Grafo.ObtenerVecinos(persona);

		foreach (var h in hijos)
		{
			// evitar volver a subir hacia padres si el grafo devuelve vecinos
			if (h != persona.Padre && h != persona.Madre)
				DibujarNodo(h, nivel + 1);
		}
	}

	private Control CrearPanelPersona(FamilyMember p, int nivel)
	{
		var panel = new PanelContainer();
		panel.CustomMinimumSize = new Vector2(350, 180);

		// StyleBoxFlat compatible Godot 4: usar propiedades individuales
		var style = new StyleBoxFlat();
		style.BgColor = new Color(0.20f, 0.21f, 0.25f);
		style.BorderColor = new Color(0.95f, 0.95f, 0.95f);
		style.BorderWidthLeft = 2;
		style.BorderWidthRight = 2;
		style.BorderWidthTop = 2;
		style.BorderWidthBottom = 2;
		style.CornerRadiusTopLeft = 12;
		style.CornerRadiusTopRight = 12;
		style.CornerRadiusBottomLeft = 12;
		style.CornerRadiusBottomRight = 12;

		panel.AddThemeStyleboxOverride("panel", style);

		var hbox = new HBoxContainer();
		hbox.AddThemeConstantOverride("separation", 16);

		panel.AddChild(hbox);

		var foto = new TextureRect();
		foto.CustomMinimumSize = new Vector2(120, 120);
		foto.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		foto.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		foto.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

		if (!string.IsNullOrEmpty(p.FotoPath))
		{
			try
			{
				var img = Image.LoadFromFile(p.FotoPath);
				var tex = ImageTexture.CreateFromImage(img);
				foto.Texture = tex;
			}
			catch
			{
				GD.PrintErr("Error cargando imagen: " + p.FotoPath);
			}
		}

		hbox.AddChild(foto);

		var vbox = new VBoxContainer();
		vbox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

		hbox.AddChild(vbox);

		var nombre = new Label();
		nombre.Text = p.Nombre ?? "Sin nombre";
		nombre.HorizontalAlignment = HorizontalAlignment.Center;
		nombre.VerticalAlignment = VerticalAlignment.Center;
		nombre.AutowrapMode = TextServer.AutowrapMode.Word;
		nombre.AddThemeFontSizeOverride("font_size", 20);

		vbox.AddChild(nombre);

		var cedula = new Label();
		cedula.Text = "Cédula: " + (p.Cedula ?? "-");
		cedula.HorizontalAlignment = HorizontalAlignment.Center;
		cedula.VerticalAlignment = VerticalAlignment.Center;
		cedula.AutowrapMode = TextServer.AutowrapMode.Word;

		vbox.AddChild(cedula);

		var nacido = new Label();
		nacido.Text = "Fecha Nac: " + (p.FechaNacimiento == DateTime.MinValue ? "No registrada" : p.FechaNacimiento.ToShortDateString());
		nacido.HorizontalAlignment = HorizontalAlignment.Center;
		vbox.AddChild(nacido);

		var vive = new Label();
		vive.Text = "Vive: " + (p.Vive ? "Sí" : "No");
		vive.HorizontalAlignment = HorizontalAlignment.Center;
		vbox.AddChild(vive);

		return panel;
	}
}
