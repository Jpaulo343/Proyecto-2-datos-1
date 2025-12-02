using Godot;
using System;

public partial class Marcador : Node2D
{
	[Export] public Sprite2D foto;
	[Export] public Label NameLabel;
	[Signal] public delegate void MarkerClickedEventHandler(Marcador marcador);
	[Export] public Node2D Punto;
	
	// Tamaño máximo del recuadro donde debe caber la imagen
	private Vector2 targetSize = new Vector2(100, 100);
	private Texture2D defaultImage;
	private String Nombre;

	public override void _Ready()
	{
		// Cargar imagen por defecto 
		defaultImage = GD.Load<Texture2D>("res://assets/imagenes/default.jpg");
		var area = GetNode<Area2D>("Area2D");
		area.InputEvent += OnInputEvent;
	}

	public void SetData(string name, Texture2D imagen)
	{
		Nombre = name;
		NameLabel.Text = name;

		Texture2D textura = imagen ?? defaultImage;

		// Redimensionar real
		textura = ResizeTexture(textura, 800, 800);

		foto.Texture = textura;
	}

	private Texture2D ResizeTexture(Texture2D tex, int width, int height)
	{
		Image img = tex.GetImage();
		img.Resize(width, height);
		return ImageTexture.CreateFromImage(img);
	}

	
	public string ObtenerNombre(){ return Nombre;}
	
	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton mouse &&
			mouse.Pressed &&
			mouse.ButtonIndex == MouseButton.Left)
		{
			EmitSignal(SignalName.MarkerClicked, this);
		}
	}
	
}
