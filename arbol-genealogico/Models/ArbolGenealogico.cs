using System;
using System.Collections.Generic;
using System.Linq;

public class ArbolGenealogico
{
	public Persona Raiz { get; private set; }
	public List<Persona> Personas { get; private set; } = new List<Persona>();

	public void AgregarPersona(Persona p)
	{
		if (p == null)
			throw new ArgumentNullException("No se puede agregar una persona nula");

		if (Personas.Any(x => x.Cedula == p.Cedula))
			throw new Exception("Ya existe una persona con esa cédula");

		Personas.Add(p);
		if (Raiz == null) Raiz = p;
	}

	public void AsignarPadre(Persona hijo, Persona padre)
	{
		if (hijo == padre) throw new Exception("Una persona no puede ser su propio padre");
		if (EsAncestro(hijo, padre)) throw new Exception("Ciclo detectado en el árbol");

		hijo.Padre = padre;
		padre.Hijos.Add(hijo);
	}

	public void AsignarMadre(Persona hijo, Persona madre)
	{
		if (hijo == madre) throw new Exception("Una persona no puede ser su propia madre");
		if (EsAncestro(hijo, madre)) throw new Exception("Ciclo detectado en el árbol");

		hijo.Madre = madre;
		madre.Hijos.Add(hijo);
	}

	public void AgregarFamiliar(Persona nueva, Persona padre = null, Persona madre = null)
	{
		AgregarPersona(nueva);

		if (padre != null) AsignarPadre(nueva, padre);
		if (madre != null) AsignarMadre(nueva, madre);
	}

	private bool EsAncestro(Persona posibleAncestro, Persona persona)
	{
		if (persona == null) return false;
		if (persona == posibleAncestro) return true;

		return EsAncestro(posibleAncestro, persona.Padre) ||
			   EsAncestro(posibleAncestro, persona.Madre);
	}
	
	public Persona BuscarPorCedula(string cedula)
	{
		return Personas.FirstOrDefault(p => p.Cedula == cedula);
	}

	public List<Persona> ObtenerHermanos(Persona p)
	{
		return Personas.Where(x =>
			x != p &&
			((x.Padre != null && x.Padre == p.Padre) ||
			 (x.Madre != null && x.Madre == p.Madre))
		).ToList();
	}

	public List<Persona> ObtenerAbuelos(Persona p)
	{
		var abuelos = new List<Persona>();

		if (p.Padre != null)
		{
			if (p.Padre.Padre != null) abuelos.Add(p.Padre.Padre);
			if (p.Padre.Madre != null) abuelos.Add(p.Padre.Madre);
		}

		if (p.Madre != null)
		{
			if (p.Madre.Padre != null) abuelos.Add(p.Madre.Padre);
			if (p.Madre.Madre != null) abuelos.Add(p.Madre.Madre);
		}

		return abuelos;
	}

	public List<Persona> ObtenerDescendientes(Persona p)
	{
		var lista = new List<Persona>();

		foreach (var hijo in p.Hijos)
		{
			lista.Add(hijo);
			lista.AddRange(ObtenerDescendientes(hijo)); // Recursión
		}

		return lista;
	}

	public bool EstanRelacionados(Persona a, Persona b)
	{
		var ancestrosA = ObtenerAncestros(a);
		var ancestrosB = ObtenerAncestros(b);

		return ancestrosA.Intersect(ancestrosB).Any();
	}

	private List<Persona> ObtenerAncestros(Persona p)
	{
		var lista = new List<Persona>();

		if (p.Padre != null)
		{
			lista.Add(p.Padre);
			lista.AddRange(ObtenerAncestros(p.Padre));
		}

		if (p.Madre != null)
		{
			lista.Add(p.Madre);
			lista.AddRange(ObtenerAncestros(p.Madre));
		}

		return lista;
	}


}
