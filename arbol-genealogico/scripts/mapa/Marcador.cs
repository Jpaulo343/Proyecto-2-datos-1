using Godot;
using System;

public partial class Marcador : Node2D
{
	[Export] public Sprite2D foto;
	[Export] public Label NameLabel;
	
	// Tamaño máximo del recuadro donde debe caber la imagen
	private Vector2 targetSize = new Vector2(100, 100);
	private Texture2D defaultImage;

	public override void _Ready()
	{
		// Cargar imagen por defecto 
		defaultImage = GD.Load<Texture2D>("res://assets/imagenes/default.jpg");
		GD.Print("default:");
		GD.Print(defaultImage);
	}

	public void SetData(string name, Texture2D imagen)
	{
		// Poner nombre
		NameLabel.Text = name;
		Texture2D textura=null;
		if (imagen==null){textura = defaultImage;}
		else{textura = imagen;}
		GD.Print(imagen);
		GD.Print(textura);
		ResizeImage(textura);
		foto.Texture=textura;

	}

	private void ResizeImage(Texture2D tex)
	{
		Vector2 texSize = tex.GetSize();

		// Calcular factor de escala para que encaje en targetSize
		float scaleFactor = Math.Min(
			targetSize.X / texSize.X,
			targetSize.Y / texSize.Y
		);

		foto.Scale = new Vector2(scaleFactor, scaleFactor);

		// Opcional — centrar la foto si no está centrada
		// foto.Position = Vector2.Zero;
	}
}
