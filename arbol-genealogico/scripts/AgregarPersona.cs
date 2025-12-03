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

		// Personas es ListaEnlazada<FamilyMember>
		int index = 0;
		foreach (var p in Main.Instance.Arbol.Personas.Enumerar())
		{
			string texto = $"{p.Nombre} ({p.Cedula})";
			_padreCombo.AddItem(texto, index);
			_madreCombo.AddItem(texto, index);
			index++;
		}
	}

	private void OnGuardarPressed()
	{
		string nombre = _nombreInput.Text.Trim();
		string cedula = _cedulaInput.Text.Trim();

		if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(cedula))
		{
			MostrarError("Nombre y cédula son obligatorios.");
			return;
		}
		string latitud= _latInput.Text.Trim();
		string longitud= _lonInput.Text.Trim();
		if (string.IsNullOrEmpty(latitud) || string.IsNullOrEmpty(longitud))
		{
			MostrarError("las coordenadas son obligatorias.");
			return;
		}

		DateTime fechaNacimiento = DateTime.MinValue;
		DateTime.TryParse(_fechaInput.Text, out fechaNacimiento);


		double lat = 0, lon = 0;
		double.TryParse(_latInput.Text, out lat);
		double.TryParse(_lonInput.Text, out lon);

		var nuevaPersona = new FamilyMember(cedula, nombre, lat, lon, _fotoPath)
		{
			Vive = _viveCheck.ButtonPressed,
			FechaNacimiento = fechaNacimiento
		};

		FamilyMember padre = null;
		int padreItem = _padreCombo.GetSelected();
		if (padreItem > 0)
		{
			int padreRealIndex = _padreCombo.GetItemId(padreItem);
			padre = ObtenerPersonaPorIndice(padreRealIndex);
		}

		FamilyMember madre = null;
		int madreItem = _madreCombo.GetSelected();
		if (madreItem > 0)
		{
			int madreRealIndex = _madreCombo.GetItemId(madreItem);
			madre = ObtenerPersonaPorIndice(madreRealIndex);
		}

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


		Main.Instance.GuardarArbolEnDisco();

		
			MostrarError("Persona agregada: {nuevaPersona.Nombre}");

		GetTree().ChangeSceneToFile("res://scenes/PanelPrincipal.tscn");
	}

	private FamilyMember ObtenerPersonaPorIndice(int idx)
	{
		int i = 0;
		foreach (var p in Main.Instance.Arbol.Personas.Enumerar())
		{
			if (i == idx)
				return p;
			i++;
		}
		return null;
	}

	private void OnVolverPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/PanelPrincipal.tscn");
	}
	
	private void MostrarError(string mensaje)
	{
		var dialog = new AcceptDialog();
		dialog.DialogText = mensaje;
		AddChild(dialog);
		dialog.PopupCentered();
	}

}
