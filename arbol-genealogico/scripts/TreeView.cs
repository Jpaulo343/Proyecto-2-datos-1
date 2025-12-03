using Godot;
using System;

public partial class TreeView : Control
{
	private Control _graph;
	private Vector2 _offset = new Vector2(200, 200); 
	private float _verticalSpacing = 200f;
	private float _horizontalSpacing = 260f;

	// ---- DRAG & ZOOM ----
	private bool _dragging = false;
	private Vector2 _dragStartPos;
	private float _zoom = 1f;
	private const float MinZoom = 0.5f;
	private const float MaxZoom = 2.5f;

	public override void _Ready()
	{
		_graph = GetNode<Control>("ZoomArea/Graph");

		GetNode<Button>("VolverBtn").Pressed += OnVolver;

		// Permite que el nodo reciba eventos del mouse
		SetProcessInput(true);

		DibujarArbol();
	}

	private void OnVolver()
	{
		GetTree().ChangeSceneToFile("res://scenes/PanelPrincipal.tscn");
	}

	// ============================================================
	//                       DRAG & ZOOM
	// ============================================================

	public override void _Input(InputEvent e)
	{
		// ---- ZOOM ----
		if (e is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.WheelUp)
			{
				_zoom = Mathf.Clamp(_zoom + 0.1f, MinZoom, MaxZoom);
				_graph.Scale = new Vector2(_zoom, _zoom);
			}
			else if (mb.ButtonIndex == MouseButton.WheelDown)
			{
				_zoom = Mathf.Clamp(_zoom - 0.1f, MinZoom, MaxZoom);
				_graph.Scale = new Vector2(_zoom, _zoom);
			}

			// ---- DRAG START ----
			if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
			{
				_dragging = true;
				_dragStartPos = GetViewport().GetMousePosition();
			}

			if (mb.ButtonIndex == MouseButton.Left && !mb.Pressed)
			{
				_dragging = false;
			}
		}

		// ---- DRAG MOVE ----
		if (e is InputEventMouseMotion motion && _dragging)
		{
			Vector2 delta = motion.Relative;
			_graph.Position += delta;
		}
	}

	// ============================================================
	//                DIBUJAR EL ÁRBOL COMPLETO
	// ============================================================

	private void DibujarArbol()
	{
		// LIMPIAR NODOS ANTERIORES
		foreach (Node child in _graph.GetChildren())
			child.QueueFree();

		FamilyMember raiz = Main.Instance.Arbol.Raiz;

		if (raiz == null)
		{
			GD.PrintErr("Árbol vacío.");
			return;
		}

		DibujarPersona(raiz, 0, 0);
	}

	private void DibujarPersona(FamilyMember p, int nivel, int indice)
	{
		// ----- TARJETA (con foto + nombre + cédula) -----
		var panel = new Button(); // Button para que sea clickeable
		panel.CustomMinimumSize = new Vector2(180, 200);
		panel.Position = CalcularPosicionNodo(nivel, indice);
		panel.AddThemeColorOverride("panel", new Color(0.85f, 0.85f, 0.90f, 1f));

		panel.Pressed += () => AbrirDetalles(p);

		// ---- FOTO ----
		TextureRect foto = new TextureRect();
		foto.CustomMinimumSize = new Vector2(140, 140);
		foto.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;

		if (!string.IsNullOrEmpty(p.FotoPath) && System.IO.File.Exists(p.FotoPath))
		{
			Image img = Image.LoadFromFile(p.FotoPath);
			foto.Texture = ImageTexture.CreateFromImage(img);
		}

		panel.AddChild(foto);

		// ---- NOMBRE ----
		Label nombre = new Label();
		nombre.Text = p.Nombre;
		nombre.HorizontalAlignment = HorizontalAlignment.Center;
		panel.AddChild(nombre);

		// ---- CEDULA ----
		Label cedula = new Label();
		cedula.Text = p.Cedula;
		cedula.HorizontalAlignment = HorizontalAlignment.Center;
		panel.AddChild(cedula);

		_graph.AddChild(panel);

		// ----- DIBUJAR HIJOS -----
		int i = 0;
		foreach (var hijo in p.Hijos.Enumerar())
		{
			Vector2 desde = panel.Position + new Vector2(90, 200);
			Vector2 hasta = CalcularPosicionNodo(nivel + 1, i) + new Vector2(90, 0);

			DibujarLinea(desde, hasta);
			DibujarPersona(hijo, nivel + 1, i);

			i++;
		}
	}

	private Vector2 CalcularPosicionNodo(int nivel, int indice)
	{
		return new Vector2(
			_offset.X + (indice * _horizontalSpacing),
			_offset.Y + (nivel * _verticalSpacing)
		);
	}

	private void DibujarLinea(Vector2 desde, Vector2 hasta)
	{
		var linea = new Line2D();
		linea.Width = 4;
		linea.DefaultColor = Colors.Black;
		linea.Points = new Vector2[] { desde, hasta };
		_graph.AddChild(linea);
	}

	// ============================================================
	//                 ABRIR DETALLES DE PERSONA
	// ============================================================

	private void AbrirDetalles(FamilyMember p)
	{
		var escena = ResourceLoader.Load<PackedScene>("res://scenes/DetallesPersona.tscn");
		var instancia = escena.Instantiate<DetallesPersona>();

		instancia.SetPersona(p);

		GetTree().Root.AddChild(instancia);
		QueueFree(); // cerrar TreeView
	}
}
