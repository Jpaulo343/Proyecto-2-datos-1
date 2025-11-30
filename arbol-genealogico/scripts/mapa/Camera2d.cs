using Godot;

public partial class Camera2d : Camera2D
{
	[Export] public float ZoomMin = 0.2f;    // Tu zoom base
	[Export] public float ZoomMax = 2.0f;     // O lo que quieras
	[Export] public float ZoomSpeedFactor = 0.9f; // Factor para reducir (ZoomIn). 0.9f significa 10% de zoom.
	[Export] public float ZoomSpeedInverseFactor = 1.1f; // Factor para aumentar (ZoomOut). 1.1f significa 10% de zoom.
	[Export] public float ZoomMoveSpeed = 5.0f; // Velocidad de interpolación para la posición (opcional, para suavidad)
	private Vector2 originalZoom;
	private Vector2 originalPosition;
	private Viewport _viewport; 

	public override void _Ready()
	{
		_viewport = GetViewport();
		// Inicializa el Zoom en el valor deseado (0.15f) si no lo haces en el editor
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
		// Si estamos en el zoom mínimo → volvemos a posición original
		if (Zoom.X <= ZoomMin + 0.0001f)
			Position = originalPosition;
	}

	private void ZoomOut(Vector2 screenMousePos)
	{
		ApplyZoom(screenMousePos, ZoomSpeedInverseFactor);
		// Si estamos en el zoom mínimo → volvemos a posición original
		if (Zoom.X <= ZoomMin + 0.0001f)
			Position = originalPosition;
		
	}

	private void ApplyZoom(Vector2 screenMousePos, float zoomFactor)
	{
		// 1. Obtener la posición actual del ratón en coordenadas del mundo
		Vector2 worldMousePos = GetGlobalMousePosition();

		// 2. Calcular el nuevo valor de Zoom
		float newZoomValue = Zoom.X * zoomFactor;
		newZoomValue = Mathf.Clamp(newZoomValue, ZoomMin, ZoomMax);
		Vector2 newZoom = new Vector2(newZoomValue, newZoomValue);

		// Si el zoom no cambia (ya está en el límite), salimos.
		if (newZoomValue == Zoom.X)
			return;

		// 3. Calcular la nueva posición de la cámara
		Vector2 oldZoom = Zoom;
		Vector2 newCameraPosition = Position + (worldMousePos - Position) * (1.0f - oldZoom.X / newZoom.X);


		// 4. Aplicar el nuevo Zoom y la nueva Posición
		Zoom = newZoom;
		Position = newCameraPosition; // La cámara se mueve para centrar el nuevo zoom en el ratón.
		
	}
}
