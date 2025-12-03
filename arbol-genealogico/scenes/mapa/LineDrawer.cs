using Godot;
using System.Collections.Generic;

public partial class LineDrawer : Node2D
{
	private ListaEnlazada<Vector2> LinePoints = new ListaEnlazada<Vector2>();
	private ListaEnlazada<(Vector2 pos, string text)> Labels = new ListaEnlazada<(Vector2 pos, string text)>();

	public void ClearAll()
	{
		LinePoints = new ListaEnlazada<Vector2>();
		Labels = new ListaEnlazada<(Vector2 pos, string text)>();
		QueueRedraw();  
	}

	public void DrawConnection(Vector2 from, Vector2 to, string text)
	{
		from = ToLocal(from);
		to = ToLocal(to);

		// Guardar los puntos
		LinePoints.Añadir(from);
		LinePoints.Añadir(to);

		// Punto medio
		Vector2 mid = (from + to) * 0.5f;

		// Guardamos la etiqueta
		Labels.Añadir((mid, text));

		QueueRedraw();
	}
	


	public override void _Draw()
	{
		// Dibujar líneas
		Vector2? puntoPrevio = null;
		foreach (var punto in LinePoints.Enumerar())
		{
			if (puntoPrevio == null)
			{
				puntoPrevio = punto;
			}
			else
			{
				DrawLine(puntoPrevio.Value, punto, Colors.Red, 10);
				puntoPrevio = null;
			}
		}

		// Dibujar textos
		foreach (var label in Labels.Enumerar())
		{
			Vector2 pos = label.pos + new Vector2(10, 0);
			DrawString(ThemeDB.FallbackFont, pos, label.text, HorizontalAlignment.Left, -1, 75, Colors.Black);
		}
	}	
}
