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

public class Nodo<T>
{
	public T Value { get; private set; }
	public Nodo<T>? Next { get; set; }

	public Nodo(T dato)
	{
		Value = dato;
	}
}

public class ListaEnlazada<T>
{
	public Nodo<T>? Head { get; protected set; }
	public Nodo<T>? Tail { get; protected set; }
	public int size { get; protected set; }


	public void Añadir(T dato)
	{
		Nodo<T> nodo = new Nodo<T>(dato);
		size++;
		if (Head == null)
		{
			Tail = Head = nodo;
			return;
		}
		else
		{
			Nodo<T> Temp = Head;
			Head = nodo;
			Head.Next = Temp;
			return;
		}
	}


	public bool Buscar(T dato)
	{

		Nodo<T>? actual = Head;
		while (actual != null)
		{
			if (actual.Value!.Equals(dato))
			{
				return true;
			}

			actual = actual.Next!;
		}
		return false;
	}


	public IEnumerable<T> Enumerar()
	{
		Nodo<T>? actual = Head!;
		while (actual != null)
		{
			yield return actual.Value;
			actual = actual.Next!;
		}
	}
}


public class GrafoFamilia
{
	private Dictionary<FamilyMember, ListaEnlazada<FamilyMember>> adyacencia =
		new Dictionary<FamilyMember, ListaEnlazada<FamilyMember>>();

	public void AgregarNodo(FamilyMember miembro)
	{
		if (!adyacencia.ContainsKey(miembro))
		{
			adyacencia[miembro] = new ListaEnlazada<FamilyMember>();
		}
	}

	public void AgregarArista(FamilyMember a, FamilyMember b, bool bidireccional = true)
	{
		AgregarNodo(a);
		AgregarNodo(b);

		if (!adyacencia[a].Buscar(b))
			adyacencia[a].Añadir(b);

		if (bidireccional && !adyacencia[b].Buscar(a))
			adyacencia[b].Añadir(a);
	}

	public IEnumerable<FamilyMember> ObtenerVecinos(FamilyMember miembro)
	{
		if (adyacencia.ContainsKey(miembro))
			return adyacencia[miembro].Enumerar();

		return new ListaEnlazada<FamilyMember>().Enumerar();
	}

	public IEnumerable<FamilyMember> ObtenerTodosLosMiembros()
	{
		return adyacencia.Keys;
	}

	public FamilyMember ObtenerMiembroPorNombre(string nombre)
	{
		foreach (var miembro in adyacencia.Keys)
		{
			if (miembro.Nombre == nombre)
			{
				return miembro;
			}
		}

		return null; 
	}

	private double Haversine(double lat1, double lon1, double lat2, double lon2)
	{
		const double R = 6371.0; 

		double dLat = GradosARadianes(lat2 - lat1);
		double dLon = GradosARadianes(lon2 - lon1);

		lat1 = GradosARadianes(lat1);
		lat2 = GradosARadianes(lat2);

		double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
				   Math.Cos(lat1) * Math.Cos(lat2) *
				   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

		double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

		return R * c; 
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

		foreach (var miembroA in ObtenerTodosLosMiembros())
		{
			foreach (var miembroB in ObtenerTodosLosMiembros())
			{
				if (miembroA == miembroB) continue;

				double d = CalcularDistancia(miembroA, miembroB);
				if (d > maxDist)
				{
					maxDist = d;
					m1 = miembroA;
					m2 = miembroB;
				}
			}
		}

		return (m1, m2, maxDist);
	}

	public (FamilyMember, FamilyMember, double) ParMasCercano()
	{
		double minDist = double.MaxValue;
		FamilyMember m1 = null, m2 = null;

		foreach (var miembroA in ObtenerTodosLosMiembros())
		{
			foreach (var miembroB in ObtenerTodosLosMiembros())
			{
				if (miembroA == miembroB) continue;

				double d = CalcularDistancia(miembroA, miembroB);
				if (d < minDist)
				{
					minDist = d;
					m1 = miembroA;
					m2 = miembroB;
				}
			}
		}

		return (m1, m2, minDist);
	}

	public double DistanciaPromedio()
	{
		double suma = 0;
		int conteo = 0;

		foreach (var miembroA in ObtenerTodosLosMiembros())
		{
			foreach (var miembroB in ObtenerTodosLosMiembros())
			{
				if (miembroA == miembroB) continue;

				suma += CalcularDistancia(miembroA, miembroB);
				conteo++;
			}
		}

		if (conteo == 0) return 0;

		return suma / conteo;
	}
}
