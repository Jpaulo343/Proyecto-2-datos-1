using Godot;
using System;

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
		// Limpiar contenido previo
		foreach (Node child in _container.GetChildren())
			child.QueueFree();

		if (Main.Instance.Arbol.Raiz == null)
		{
			GD.Print("Árbol vacío");
			return;
		}

		// Dibujar desde la raíz
		DibujarPersona(Main.Instance.Arbol.Raiz, 0);
	}

	private void DibujarPersona(FamilyMember persona, int nivel)
	{
		if (persona == null) return;

		// Crea fila horizontal: indentación + tarjeta
		var fila = new HBoxContainer();

		// Indentación según nivel (padres- hijos- nietos…)
		var indent = new Control();
		indent.CustomMinimumSize = new Vector2(nivel * 60, 0);
		fila.AddChild(indent);

		// Crear tarjeta visual
		var card = CrearPanelPersona(persona);
		fila.AddChild(card);

		_container.AddChild(fila);

		// Dibujar recursivamente los hijos
		foreach (var hijo in persona.Hijos.Enumerar())
		{
			DibujarPersona(hijo, nivel + 1);
		}
	}

	private Control CrearPanelPersona(FamilyMember p)
	{
		var panel = new PanelContainer();
		panel.CustomMinimumSize = new Vector2(320, 120);

		// Estilo elegante
		var style = new StyleBoxFlat();
		style.BgColor = new Color(0.20f, 0.21f, 0.25f);
		style.BorderColor = new Color(1, 1, 1);
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
		hbox.AddThemeConstantOverride("separation", 12);
		panel.AddChild(hbox);

		// FOTO
		var foto = new TextureRect();
		foto.CustomMinimumSize = new Vector2(70, 70);
		foto.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		foto.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;

		if (!string.IsNullOrEmpty(p.FotoPath))
		{
			try
			{
				var img = Image.LoadFromFile(p.FotoPath);
				foto.Texture = ImageTexture.CreateFromImage(img);
			}
			catch
			{
				GD.PrintErr("Error cargando foto: " + p.FotoPath);
			}
		}

		hbox.AddChild(foto);

		// INFO
		var vbox = new VBoxContainer();
		hbox.AddChild(vbox);

		var nombre = new Label();
		nombre.Text = p.Nombre;
		nombre.HorizontalAlignment = HorizontalAlignment.Center;
		nombre.AddThemeFontSizeOverride("font_size", 18);
		nombre.AutowrapMode = TextServer.AutowrapMode.Word;
		vbox.AddChild(nombre);

		var cedula = new Label();
		cedula.Text = "Cédula: " + p.Cedula;
		cedula.HorizontalAlignment = HorizontalAlignment.Center;
		vbox.AddChild(cedula);

		var fecha = new Label();
		fecha.Text = "Nacimiento: " +
			(p.FechaNacimiento == DateTime.MinValue ? "N/A" : p.FechaNacimiento.ToShortDateString());
		fecha.HorizontalAlignment = HorizontalAlignment.Center;
		vbox.AddChild(fecha);

		var vive = new Label();
		vive.Text = "Vive: " + (p.Vive ? "Sí" : "No");
		vive.HorizontalAlignment = HorizontalAlignment.Center;
		vbox.AddChild(vive);

		return panel;
	}
}
