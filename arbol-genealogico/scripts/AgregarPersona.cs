using Godot;
using System;

public partial class AgregarPersona : Control
{
	private LineEdit _nombreInput;
	private LineEdit _cedulaInput;
	private LineEdit _fechaInput;
	private CheckBox _viveCheck;
	private TextureRect _fotoPreview;
	private OptionButton _padreCombo;
	private OptionButton _madreCombo;
	private LineEdit _latInput;
	private LineEdit _lonInput;

	private string _fotoPath = "";

	public override void _Ready()
	{
		_nombreInput = GetNode<LineEdit>("MarginContainer/VBoxContainer/NombreCont/NombreInput");
		_cedulaInput = GetNode<LineEdit>("MarginContainer/VBoxContainer/CedulaCont/CedulaInput");
		_fechaInput = GetNode<LineEdit>("MarginContainer/VBoxContainer/FechaCont/FechaInput");
		_viveCheck = GetNode<CheckBox>("MarginContainer/VBoxContainer/ViveCont/ViveCheck");
		_fotoPreview = GetNode<TextureRect>("MarginContainer/VBoxContainer/FotoPreview");

		_padreCombo = GetNode<OptionButton>("MarginContainer/VBoxContainer/PadreCont/PadreCombo");
		_madreCombo = GetNode<OptionButton>("MarginContainer/VBoxContainer/MadreCont/MadreCombo");

		// nuevos inputs para coordenadas
		_latInput = GetNode<LineEdit>("MarginContainer/VBoxContainer/CoordCont/LatInput");
		_lonInput = GetNode<LineEdit>("MarginContainer/VBoxContainer/CoordCont/LonInput");

		var seleccionarBtn = GetNode<Button>("MarginContainer/VBoxContainer/FotoCont/SeleccionarFotoBtn");
		seleccionarBtn.Pressed += OnSeleccionarFotoPressed;

		var guardarBtn = GetNode<Button>("MarginContainer/VBoxContainer/BotonesCont/GuardarBtn");
		guardarBtn.Pressed += OnGuardarPressed;

		var volverBtn = GetNode<Button>("MarginContainer/VBoxContainer/BotonesCont/VolverBtn");
		volverBtn.Pressed += OnVolverPressed;

		CargarPersonasEnCombos();
	}

	private void OnSeleccionarFotoPressed()
	{
		var dialog = new FileDialog();
		dialog.Access = FileDialog.AccessEnum.Filesystem;
		dialog.Filters = new string[] { "*.png ; PNG Images", "*.jpg ; JPG Images" };
		dialog.FileSelected += OnFotoSeleccionada;
		AddChild(dialog);
		dialog.PopupCentered();
	}

	private void OnFotoSeleccionada(string path)
	{
		_fotoPath = path;
		var image = Image.LoadFromFile(path);
		var texture = ImageTexture.CreateFromImage(image);
		_fotoPreview.Texture = texture;
		GD.Print($"Foto seleccionada: {path}");
	}

	private void CargarPersonasEnCombos()
	{
		_padreCombo.Clear();
		_madreCombo.Clear();

		_padreCombo.AddItem("Sin asignar", -1);
		_madreCombo.AddItem("Sin asignar", -1);

		var lista = Main.Instance.Arbol.Personas;
		for (int i = 0; i < lista.Count; i++)
		{
			var p = lista[i];
			string texto = $"{p.Nombre} ({p.Cedula})";
			_padreCombo.AddItem(texto, i);
			_madreCombo.AddItem(texto, i);
		}
	}

	private void OnGuardarPressed()
	{
		string nombre = _nombreInput.Text.Trim();
		string cedula = _cedulaInput.Text.Trim();

		if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(cedula))
		{
			GD.PrintErr("Nombre y cédula son obligatorios.");
			return;
		}

		DateTime fechaNacimiento = DateTime.MinValue;
		DateTime.TryParse(_fechaInput.Text, out fechaNacimiento);

		var nuevaPersona = new Persona(nombre, cedula)
		{
			Vive = _viveCheck.ButtonPressed,
			FechaNacimiento = fechaNacimiento,
			FotoPath = _fotoPath
		};

		// coordenadas (si las puso)
		double lat = 0, lon = 0;
		if (double.TryParse(_latInput.Text, out lat) && double.TryParse(_lonInput.Text, out lon))
		{
			nuevaPersona.Coordenadas = (lat, lon);
		}

		// padre/madre
		Persona padre = null;
		int padreItem = _padreCombo.GetSelected();
		if (padreItem > 0)
		{
			int padreRealIndex = _padreCombo.GetItemId(padreItem);
			padre = Main.Instance.Arbol.Personas[padreRealIndex];
		}

		Persona madre = null;
		int madreItem = _madreCombo.GetSelected();
		if (madreItem > 0)
		{
			int madreRealIndex = _madreCombo.GetItemId(madreItem);
			madre = Main.Instance.Arbol.Personas[madreRealIndex];
		}

		// validaciones
		if (padre != null && Main.Instance.Arbol.EstanRelacionados(nuevaPersona, padre))
		{
			GD.PrintErr("Asignar este padre crearía un ciclo en el árbol.");
			return;
		}
		if (madre != null && Main.Instance.Arbol.EstanRelacionados(nuevaPersona, madre))
		{
			GD.PrintErr("Asignar esta madre crearía un ciclo en el árbol.");
			return;
		}

		Main.Instance.Arbol.AgregarFamiliar(nuevaPersona, padre, madre);

		// guardar en disco
		Main.Instance.GuardarArbolEnDisco();

		GD.Print($"Persona agregada: {nuevaPersona.Nombre}");

		GetTree().ChangeSceneToFile("res://scenes/PanelPrincipal.tscn");
	}

	private void OnVolverPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/PanelPrincipal.tscn");
	}
}
