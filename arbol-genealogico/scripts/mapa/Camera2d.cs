using Godot;

public partial class Camera2d : Camera2D
{
	[Export] public float ZoomMin = 0.2f;    // zoom base
	[Export] public float ZoomMax = 2.0f;    
	[Export] public float ZoomSpeedFactor = 0.9f; // Factor para reducir (ZoomIn).
	[Export] public float ZoomSpeedInverseFactor = 1.1f; // Factor para aumentar (ZoomOut). 
	private Vector2 originalZoom;
	private Vector2 originalPosition;
	private Viewport _viewport; 

	public override void _Ready()
	{
		_viewport = GetViewport();
		// Inicializa el Zoom en el valor deseado (0.15f) 
		Zoom = new Vector2(ZoomMin, ZoomMin); 
		originalZoom = Zoom;
		originalPosition = Position;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb && mb.Pressed)
		{
			if (mb.ButtonIndex == MouseButton.WheelUp)
				ZoomOut(mb.Position);

			if (mb.ButtonIndex == MouseButton.WheelDown)
				ZoomIn(mb.Position);
		}
	}

	private void ZoomIn(Vector2 screenMousePos)
	{
		ApplyZoom(screenMousePos, ZoomSpeedFactor);
		// pone el zoom en la posición original
		if (Zoom.X <= ZoomMin + 0.0001f)
			Position = originalPosition;
	}

	private void ZoomOut(Vector2 screenMousePos)
	{
		ApplyZoom(screenMousePos, ZoomSpeedInverseFactor);
		// pone el zoom en la posición original
		if (Zoom.X <= ZoomMin + 0.0001f)
			Position = originalPosition;
		
	}

	private void ApplyZoom(Vector2 screenMousePos, float zoomFactor)
	{
		// Obtener la posición actual del ratón en coordenadas
		Vector2 worldMousePos = GetGlobalMousePosition();

		// Calcular el nuevo valor de Zoom
		float newZoomValue = Zoom.X * zoomFactor;
		newZoomValue = Mathf.Clamp(newZoomValue, ZoomMin, ZoomMax);
		Vector2 newZoom = new Vector2(newZoomValue, newZoomValue);

		// Si el zoom no cambia (ya está en el límite), no hace nada
		if (newZoomValue == Zoom.X)
			return;

		//Calcular la nueva posición de la cámara
		Vector2 oldZoom = Zoom;
		Vector2 newCameraPosition = Position + (worldMousePos - Position) * (1.0f - oldZoom.X / newZoom.X);


		// Aplicar el nuevo Zoom y la nueva Posición
		Zoom = newZoom;
		Position = newCameraPosition; // La cámara se mueve para centrar el nuevo zoom en el ratón.
		
	}
}
