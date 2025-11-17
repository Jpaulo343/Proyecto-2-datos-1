using System;
using System.Collections.Generic;

public class Persona
{
	public string Nombre { get; set; }
	public string Cedula { get; set; }
	public string FotoPath { get; set; }
	public DateTime FechaNacimiento { get; set; }
	public int Edad { get; set; }
	public bool Vive { get; set; }
	public (double lat, double lon) Coordenadas { get; set; }

	public Persona Padre { get; set; }
	public Persona Madre { get; set; }
	public List<Persona> Hijos { get; set; } = new List<Persona>();

	public Persona(string nombre, string cedula)
	{
		Nombre = nombre;
		Cedula = cedula;
	}

	public override string ToString()
	{
		return $"{Nombre} ({Cedula})";
	}
}
