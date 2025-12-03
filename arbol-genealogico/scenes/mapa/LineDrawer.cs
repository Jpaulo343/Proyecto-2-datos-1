using Godot;
using System.Collections.Generic;

public partial class LineDrawer : Node2D
{
	public List<Vector2> LinePoints = new List<Vector2>();
	public List<(Vector2 pos, string text)> Labels = new List<(Vector2, string)>();

	public void ClearAll()
	{
		LinePoints.Clear();
		Labels.Clear();
		QueueRedraw();  
	}

	public void DrawConnection(Vector2 from, Vector2 to, string text)
	{
		from = ToLocal(from);
		to = ToLocal(to);

		LinePoints.Add(from);
		LinePoints.Add(to);		
		Vector2 mid = (from + to) * 0.5f;
		Labels.Add((mid, text));
		QueueRedraw(); 
	}


	public override void _Draw()
	{
		// Dibujar líneas
		for (int i = 0; i < LinePoints.Count; i += 2)
		{
			DrawLine(LinePoints[i], LinePoints[i+1], Colors.Red, 10);	
		}

		// Dibujar textos
		foreach (var label in Labels)
		{
			Vector2 pos = label.pos + new Vector2(10, 0);
			DrawString(ThemeDB.FallbackFont, pos, label.text, HorizontalAlignment.Left, -1, 75, Colors.Black);
		}
	}
}
