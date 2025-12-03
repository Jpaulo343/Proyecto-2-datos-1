using System;
using System.IO;
using System.Text.Json;

public class ArbolGenealogico
{
	public FamilyMember Raiz { get; private set; }

	public ListaEnlazada<FamilyMember> Personas { get; private set; } = new ListaEnlazada<FamilyMember>();

	public GrafoFamilia Grafo { get; private set; } = new GrafoFamilia();

	// ---------------- ALTAS ----------------

	public void AgregarPersona(FamilyMember p)
	{
		if (p == null) throw new ArgumentNullException("No se puede agregar una persona nula");
		if (BuscarPorCedula(p.Cedula) != null) throw new Exception("Ya existe una persona con esa cédula");

		Personas.Añadir(p);
		Grafo.AgregarNodo(p);

		if (Raiz == null) Raiz = p;
	}

	public void AgregarFamiliar(FamilyMember nueva, FamilyMember padre = null, FamilyMember madre = null)
	{
		AgregarPersona(nueva);

		if (padre != null) AsignarPadre(nueva, padre);
		if (madre != null) AsignarMadre(nueva, madre);
	}

	// ---------------- RELACIONES ----------------

	public void AsignarPadre(FamilyMember hijo, FamilyMember padre)
	{
		if (hijo == padre) throw new Exception("Una persona no puede ser su propio padre");
		if (EsAncestro(hijo, padre)) throw new Exception("Ciclo detectado en el árbol");

		// Quitar padre anterior
		if (hijo.Padre != null)
		{
			hijo.Padre.Hijos.Eliminar(hijo);
			Grafo.QuitarRelacion(hijo.Padre, hijo);
		}

		hijo.Padre = padre;
		padre.Hijos.Añadir(hijo);

		Grafo.AgregarArista(hijo, padre, true);
	}

	public void AsignarMadre(FamilyMember hijo, FamilyMember madre)
	{
		if (hijo == madre) throw new Exception("Una persona no puede ser su propia madre");
		if (EsAncestro(hijo, madre)) throw new Exception("Ciclo detectado en el árbol");

		if (hijo.Madre != null)
		{
			hijo.Madre.Hijos.Eliminar(hijo);
			Grafo.QuitarRelacion(hijo.Madre, hijo);
		}

		hijo.Madre = madre;
		madre.Hijos.Añadir(hijo);

		Grafo.AgregarArista(hijo, madre, true);
	}

	private bool EsAncestro(FamilyMember posibleAncestro, FamilyMember persona)
	{
		if (persona == null) return false;
		if (persona == posibleAncestro) return true;

		return EsAncestro(posibleAncestro, persona.Padre) ||
			   EsAncestro(posibleAncestro, persona.Madre);
	}

	// ---------------- BÚSQUEDAS Y CONSULTAS ----------------

	public FamilyMember BuscarPorCedula(string cedula)
	{
		if (cedula == null) return null;

		Nodo<FamilyMember>? actual = Personas.Head;
		while (actual != null)
		{
			if (actual.Value.Cedula == cedula)
				return actual.Value;

			actual = actual.Next;
		}

		return null;
	}

	public ListaEnlazada<FamilyMember> ObtenerHermanos(FamilyMember p)
	{
		ListaEnlazada<FamilyMember> resultado = new ListaEnlazada<FamilyMember>();

		Nodo<FamilyMember>? actual = Personas.Head;
		while (actual != null)
		{
			var x = actual.Value;
			if (x != p)
			{
				bool compartenPadre = (x.Padre != null && p.Padre != null && x.Padre == p.Padre);
				bool compartenMadre = (x.Madre != null && p.Madre != null && x.Madre == p.Madre);

				if (compartenPadre || compartenMadre)
					resultado.Añadir(x);
			}

			actual = actual.Next;
		}

		return resultado;
	}

	public ListaEnlazada<FamilyMember> ObtenerAbuelos(FamilyMember p)
	{
		ListaEnlazada<FamilyMember> abuelos = new ListaEnlazada<FamilyMember>();

		if (p.Padre != null)
		{
			if (p.Padre.Padre != null) abuelos.Añadir(p.Padre.Padre);
			if (p.Padre.Madre != null) abuelos.Añadir(p.Padre.Madre);
		}

		if (p.Madre != null)
		{
			if (p.Madre.Padre != null) abuelos.Añadir(p.Madre.Padre);
			if (p.Madre.Madre != null) abuelos.Añadir(p.Madre.Madre);
		}

		return abuelos;
	}

	public ListaEnlazada<FamilyMember> ObtenerDescendientes(FamilyMember p)
	{
		ListaEnlazada<FamilyMember> resultado = new ListaEnlazada<FamilyMember>();
		AgregarDescendientesRec(p, resultado);
		return resultado;
	}

	private void AgregarDescendientesRec(FamilyMember p, ListaEnlazada<FamilyMember> acumulador)
	{
		foreach (var hijo in p.Hijos.Enumerar())
		{
			acumulador.Añadir(hijo);
			AgregarDescendientesRec(hijo, acumulador);
		}
	}

	public bool EstanRelacionados(FamilyMember a, FamilyMember b)
	{
		var ancestrosA = ObtenerAncestros(a);
		var ancestrosB = ObtenerAncestros(b);

		foreach (var x in ancestrosA.Enumerar())
		{
			foreach (var y in ancestrosB.Enumerar())
			{
				if (x == y) return true;
			}
		}

		return false;
	}

	private ListaEnlazada<FamilyMember> ObtenerAncestros(FamilyMember p)
	{
		ListaEnlazada<FamilyMember> lista = new ListaEnlazada<FamilyMember>();
		AgregarAncestrosRec(p, lista);
		return lista;
	}

	private void AgregarAncestrosRec(FamilyMember p, ListaEnlazada<FamilyMember> acumulador)
	{
		if (p.Padre != null)
		{
			acumulador.Añadir(p.Padre);
			AgregarAncestrosRec(p.Padre, acumulador);
		}

		if (p.Madre != null)
		{
			acumulador.Añadir(p.Madre);
			AgregarAncestrosRec(p.Madre, acumulador);
		}
	}

	// ---------------- ELIMINAR ----------------

	public bool EliminarPersona(FamilyMember p)
	{
		if (p == null) return false;

		if (p.Padre != null)
		{
			p.Padre.Hijos.Eliminar(p);
			Grafo.QuitarRelacion(p.Padre, p);
			p.Padre = null;
		}

		if (p.Madre != null)
		{
			p.Madre.Hijos.Eliminar(p);
			Grafo.QuitarRelacion(p.Madre, p);
			p.Madre = null;
		}

		foreach (var hijo in p.Hijos.Enumerar())
		{
			if (hijo.Padre == p) hijo.Padre = null;
			if (hijo.Madre == p) hijo.Madre = null;
			Grafo.QuitarRelacion(p, hijo);
		}

		p.Hijos = new ListaEnlazada<FamilyMember>();

		bool eliminado = Personas.Eliminar(p);

		if (eliminado && Raiz == p)
		{
			Raiz = (Personas.Head != null) ? Personas.Head.Value : null;
		}

		Grafo.EliminarNodo(p);

		return eliminado;
	}

	public bool EliminarPorCedula(string cedula)
	{
		var p = BuscarPorCedula(cedula);
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
			int n = Personas.size;
			PersonaDto[] array = new PersonaDto[n];

			int i = 0;
			Nodo<FamilyMember>? actual = Personas.Head;
			while (actual != null)
			{
				var p = actual.Value;

				array[i++] = new PersonaDto
				{
					Nombre = p.Nombre,
					Cedula = p.Cedula,
					FotoPath = p.FotoPath,
					FechaNacimiento = p.FechaNacimiento,
					Vive = p.Vive,
					Lat = p.Latitud,
					Lon = p.Longitud,
					PadreCedula = p.Padre != null ? p.Padre.Cedula : null,
					MadreCedula = p.Madre != null ? p.Madre.Cedula : null
				};

				actual = actual.Next;
			}

			string json = JsonSerializer.Serialize(array, new JsonSerializerOptions { WriteIndented = true });

			if (string.IsNullOrEmpty(path))
				path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "arbol.json");

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
			if (string.IsNullOrEmpty(path))
				path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "arbol.json");

			if (!File.Exists(path)) return false;

			string json = File.ReadAllText(path);
			PersonaDto[] datos = JsonSerializer.Deserialize<PersonaDto[]>(json);

			Personas = new ListaEnlazada<FamilyMember>();
			Grafo = new GrafoFamilia();
			Raiz = null;

			if (datos == null || datos.Length == 0)
				return true;

			// Crear miembros sin relaciones
			foreach (var d in datos)
			{
				var persona = new FamilyMember(d.Cedula, d.Nombre, d.Lat, d.Lon, d.FotoPath)
				{
					FechaNacimiento = d.FechaNacimiento,
					Vive = d.Vive
				};

				Personas.Añadir(persona);
				Grafo.AgregarNodo(persona);
				if (Raiz == null) Raiz = persona;
			}

			// Asignar padres y madres
			foreach (var d in datos)
			{
				var persona = BuscarPorCedula(d.Cedula);
				if (persona == null) continue;

				if (!string.IsNullOrEmpty(d.PadreCedula))
				{
					var padre = BuscarPorCedula(d.PadreCedula);
					if (padre != null)
					{
						AsignarPadre(persona, padre);
					}
				}

				if (!string.IsNullOrEmpty(d.MadreCedula))
				{
					var madre = BuscarPorCedula(d.MadreCedula);
					if (madre != null)
					{
						AsignarMadre(persona, madre);
					}
				}
			}

			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error cargando arbol: " + ex.Message);
			return false;
		}
	}
}
