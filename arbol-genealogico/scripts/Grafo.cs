using Godot;
using System;
using System.Collections.Generic;

public class FamilyMember
{
	public string Cedula { get; set; }
	public string Nombre { get; set; }
	public double Latitud { get; set; }
	public double Longitud { get; set; }
	public string FotoPath { get; set; }

	public FamilyMember(string cedula, string nombre, double lat, double lon, string fotoPath = "")
	{
		Cedula = cedula;
		Nombre = nombre;
		Latitud = lat;
		Longitud = lon;
		FotoPath = fotoPath;
	}
}


public class GrafoFamilia
{
	private Dictionary<FamilyMember, List<FamilyMember>> adyacencia =
		new Dictionary<FamilyMember, List<FamilyMember>>();

	public void AgregarNodo(FamilyMember miembro)
	{
		if (!adyacencia.ContainsKey(miembro))
		{
			adyacencia[miembro] = new List<FamilyMember>();
		}
	}

	public void AgregarArista(FamilyMember a, FamilyMember b, bool bidireccional = true)
	{
		AgregarNodo(a);
		AgregarNodo(b);

		if (!adyacencia[a].Contains(b))
			adyacencia[a].Add(b);

		if (bidireccional && !adyacencia[b].Contains(a))
			adyacencia[b].Add(a);
	}

	public IEnumerable<FamilyMember> ObtenerVecinos(FamilyMember miembro)
	{
		if (adyacencia.ContainsKey(miembro))
			return adyacencia[miembro];

		return new List<FamilyMember>();
	}

	public IEnumerable<FamilyMember> ObtenerTodosLosMiembros()
	{
		return adyacencia.Keys;
	}

	private double Haversine(double lat1, double lon1, double lat2, double lon2)
	{
		const double R = 6371.0; // Radio de la Tierra en km

		double dLat = GradosARadianes(lat2 - lat1);
		double dLon = GradosARadianes(lon2 - lon1);

		lat1 = GradosARadianes(lat1);
		lat2 = GradosARadianes(lat2);

		double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
				   Math.Cos(lat1) * Math.Cos(lat2) *
				   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

		double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

		return R * c; // km
	}

	private double GradosARadianes(double grados)
	{
		return grados * Math.PI / 180.0;
	}

	public double CalcularDistancia(FamilyMember a, FamilyMember b)
	{
		return Haversine(a.Latitud, a.Longitud, b.Latitud, b.Longitud);
	}


	public Dictionary<FamilyMember, double> ObtenerDistanciasDesde(FamilyMember origen)
	{
		var distancias = new Dictionary<FamilyMember, double>();

		foreach (var miembro in ObtenerTodosLosMiembros())
		{
			if (miembro != origen)
			{
				double d = CalcularDistancia(origen, miembro);
				distancias[miembro] = d;
			}
		}

		return distancias;
	}


	public (FamilyMember, FamilyMember, double) ParMasLejano()
	{
		double maxDist = -1;
		FamilyMember m1 = null, m2 = null;

		var miembros = new List<FamilyMember>(ObtenerTodosLosMiembros());

		for (int i = 0; i < miembros.Count; i++)
		{
			for (int j = i + 1; j < miembros.Count; j++)
			{
				double d = CalcularDistancia(miembros[i], miembros[j]);
				if (d > maxDist)
				{
					maxDist = d;
					m1 = miembros[i];
					m2 = miembros[j];
				}
			}
		}

		return (m1, m2, maxDist);
	}

	public (FamilyMember, FamilyMember, double) ParMasCercano()
	{
		double minDist = double.MaxValue;
		FamilyMember m1 = null, m2 = null;

		var miembros = new List<FamilyMember>(ObtenerTodosLosMiembros());

		for (int i = 0; i < miembros.Count; i++)
		{
			for (int j = i + 1; j < miembros.Count; j++)
			{
				double d = CalcularDistancia(miembros[i], miembros[j]);
				if (d < minDist)
				{
					minDist = d;
					m1 = miembros[i];
					m2 = miembros[j];
				}
			}
		}

		return (m1, m2, minDist);
	}

	public double DistanciaPromedio()
	{
		double suma = 0;
		int conteo = 0;

		var miembros = new List<FamilyMember>(ObtenerTodosLosMiembros());

		for (int i = 0; i < miembros.Count; i++)
		{
			for (int j = i + 1; j < miembros.Count; j++)
			{
				suma += CalcularDistancia(miembros[i], miembros[j]);
				conteo++;
			}
		}

		if (conteo == 0) return 0;

		return suma / conteo;
	}
}
