using Godot;

public partial class Main : Node
{
	public static Main Instance { get; private set; }
	public ArbolGenealogico Arbol { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		Arbol = new ArbolGenealogico();

		// Intentar cargar guardado (devuelve true si cargó)
		var userPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "arbol.json");
		Arbol.CargarDesdeArchivo(userPath);

		GD.Print("Main cargado. Árbol listo.");
	}

	public void GuardarArbolEnDisco()
	{
		var userPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "arbol.json");
		Arbol.GuardarAArchivo(userPath);
		GD.Print("Árbol guardado en disco.");
	}
}
