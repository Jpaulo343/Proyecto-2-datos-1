using Godot;
using System;

public partial class DetallesPersona : Control
{
	private Persona _persona;
	private Label _nombre;
	private Label _cedula;
	private Label _fecha;
	private Label _vive;
	private Label _ubicacion;
	private TextureRect _foto;

	public override void _Ready()
	{
		_nombre = GetNode<Label>("MarginContainer/VBoxContainer/Nombre");
		_cedula = GetNode<Label>("MarginContainer/VBoxContainer/Cedula");
		_fecha = GetNode<Label>("MarginContainer/VBoxContainer/FechaNacimiento");
		_vive = GetNode<Label>("MarginContainer/VBoxContainer/Vive");
		_ubicacion = GetNode<Label>("MarginContainer/VBoxContainer/Ubicacion");
		_foto = GetNode<TextureRect>("MarginContainer/VBoxContainer/Foto");

		GetNode<Button>("MarginContainer/VBoxContainer/BotonesCont/VolverBtn")
			.Pressed += OnVolverPressed;

		var eliminarBtn = GetNode<Button>("MarginContainer/VBoxContainer/BotonesCont/EliminarBtn");
		if (eliminarBtn != null) eliminarBtn.Pressed += OnEliminarPressed;
	}

	public void SetPersona(Persona p)
	{
		_persona = p;
		if (_persona == null) return;

		_nombre.Text = $"Nombre: {p.Nombre}";
		_cedula.Text = $"Cédula: {p.Cedula}";
		_fecha.Text = $"Fecha Nac: {(p.FechaNacimiento == DateTime.MinValue ? "No registrada" : p.FechaNacimiento.ToShortDateString())}";
		_vive.Text = $"Vive: {(p.Vive ? "Sí" : "No")}";
		_ubicacion.Text = $"Coords: {p.Coordenadas.lat}, {p.Coordenadas.lon}";

		if (!string.IsNullOrEmpty(p.FotoPath) && System.IO.File.Exists(p.FotoPath))
		{
			var img = Image.LoadFromFile(p.FotoPath);
			var tex = ImageTexture.CreateFromImage(img);
			_foto.Texture = tex;
		}
	}

	private void OnVolverPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/ListaPersonas.tscn");
	}

	private void OnEliminarPressed()
	{
		if (_persona == null) return;

		Main.Instance.Arbol.EliminarPersona(_persona);
		Main.Instance.GuardarArbolEnDisco();

		GetTree().ChangeSceneToFile("res://scenes/ListaPersonas.tscn");
	}
}
