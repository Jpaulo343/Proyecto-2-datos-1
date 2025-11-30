using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ArbolGenealogico
{
	public Persona Raiz { get; private set; }
	public List<Persona> Personas { get; private set; } = new List<Persona>();

	public void AgregarPersona(Persona p)
	{
		if (p == null) throw new ArgumentNullException("No se puede agregar una persona nula");
		if (Personas.Any(x => x.Cedula == p.Cedula)) throw new Exception("Ya existe una persona con esa cédula");
		Personas.Add(p);
		if (Raiz == null) Raiz = p;
	}

	public void AgregarFamiliar(Persona nueva, Persona padre = null, Persona madre = null)
	{
		AgregarPersona(nueva);
		if (padre != null) AsignarPadre(nueva, padre);
		if (madre != null) AsignarMadre(nueva, madre);
	}

	public void AsignarPadre(Persona hijo, Persona padre)
	{
		if (hijo == padre) throw new Exception("Una persona no puede ser su propio padre");
		if (EsAncestro(hijo, padre)) throw new Exception("Ciclo detectado en el árbol");
		if (hijo.Padre != null) hijo.Padre.Hijos.Remove(hijo);
		hijo.Padre = padre;
		padre.Hijos.Add(hijo);
	}

	public void AsignarMadre(Persona hijo, Persona madre)
	{
		if (hijo == madre) throw new Exception("Una persona no puede ser su propia madre");
		if (EsAncestro(hijo, madre)) throw new Exception("Ciclo detectado en el árbol");
		if (hijo.Madre != null) hijo.Madre.Hijos.Remove(hijo);
		hijo.Madre = madre;
		madre.Hijos.Add(hijo);
	}

	private bool EsAncestro(Persona posibleAncestro, Persona persona)
	{
		if (persona == null) return false;
		if (persona == posibleAncestro) return true;
		return EsAncestro(posibleAncestro, persona.Padre) ||
			   EsAncestro(posibleAncestro, persona.Madre);
	}

	public Persona BuscarPorCedula(string cedula) => Personas.FirstOrDefault(p => p.Cedula == cedula);

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
			lista.AddRange(ObtenerDescendientes(hijo));
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

	// ---------------- ELIMINAR ----------------
	public bool EliminarPersona(Persona p)
	{
		if (p == null) return false;
		if (!Personas.Contains(p)) return false;

		if (p.Padre != null) { p.Padre.Hijos.Remove(p); p.Padre = null; }
		if (p.Madre != null) { p.Madre.Hijos.Remove(p); p.Madre = null; }

		foreach (var hijo in p.Hijos)
		{
			if (hijo.Padre == p) hijo.Padre = null;
			if (hijo.Madre == p) hijo.Madre = null;
		}
		p.Hijos.Clear();

		if (Raiz == p) Raiz = Personas.FirstOrDefault(x => x != p);

		Personas.Remove(p);
		return true;
	}

	public bool EliminarPorCedula(string cedula)
	{
		var p = BuscarPorCedula(cedula);
		if (p == null) return false;
		return EliminarPersona(p);
	}

	// ---------------- JSON SAVE / LOAD ----------------
	private class PersonaDto
	{
		public string Nombre { get; set; }
		public string Cedula { get; set; }
		public string FotoPath { get; set; }
		public DateTime FechaNacimiento { get; set; }
		public bool Vive { get; set; }
		public double Lat { get; set; }
		public double Lon { get; set; }
		public string PadreCedula { get; set; }
		public string MadreCedula { get; set; }
	}

	public bool GuardarAArchivo(string path = null)
	{
		try
		{
			var listaDto = Personas.Select(p => new PersonaDto
			{
				Nombre = p.Nombre,
				Cedula = p.Cedula,
				FotoPath = p.FotoPath,
				FechaNacimiento = p.FechaNacimiento,
				Vive = p.Vive,
				Lat = p.Coordenadas.lat,
				Lon = p.Coordenadas.lon,
				PadreCedula = p.Padre?.Cedula,
				MadreCedula = p.Madre?.Cedula
			}).ToList();

			string json = JsonSerializer.Serialize(listaDto, new JsonSerializerOptions { WriteIndented = true });
			if (string.IsNullOrEmpty(path)) path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "arbol.json");
			File.WriteAllText(path, json);
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error guardando arbol: " + ex.Message);
			return false;
		}
	}

	public bool CargarDesdeArchivo(string path = null)
	{
		try
		{
			if (string.IsNullOrEmpty(path)) path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "arbol.json");
			if (!File.Exists(path)) return false;

			string json = File.ReadAllText(path);
			var listaDto = JsonSerializer.Deserialize<List<PersonaDto>>(json);

			// Limpiar estado actual
			Personas.Clear();
			Raiz = null;

			// crear objetos Persona sin relaciones
			foreach (var d in listaDto)
			{
				var persona = new Persona(d.Nombre, d.Cedula)
				{
					FotoPath = d.FotoPath,
					FechaNacimiento = d.FechaNacimiento,
					Vive = d.Vive,
					Coordenadas = (d.Lat, d.Lon)
				};
				Personas.Add(persona);
			}

			// asociar padres/madres por cédula
			foreach (var d in listaDto)
			{
				var persona = Personas.FirstOrDefault(p => p.Cedula == d.Cedula);
				if (persona == null) continue;

				if (!string.IsNullOrEmpty(d.PadreCedula))
				{
					var padre = Personas.FirstOrDefault(x => x.Cedula == d.PadreCedula);
					if (padre != null) { persona.Padre = padre; padre.Hijos.Add(persona); }
				}
				if (!string.IsNullOrEmpty(d.MadreCedula))
				{
					var madre = Personas.FirstOrDefault(x => x.Cedula == d.MadreCedula);
					if (madre != null) { persona.Madre = madre; madre.Hijos.Add(persona); }
				}
			}

			if (Personas.Count > 0) Raiz = Personas[0];
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error cargando arbol: " + ex.Message);
			return false;
		}
	}
}
