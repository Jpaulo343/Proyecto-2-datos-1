using Godot;
using System;
using System.Collections.Generic;

public partial class MainMapa : Node2D
{
	
	public override void _Ready()
	{
		// Carga los usuario previamente creados y los coloca en el mapa
		GD.Print("Cargando datos de usuarios");
		var usuarios = CrearUsuariosPrueba();
				GD.Print("Cargando datos de usuarios2");
		foreach(UsuarioPrueba u in usuarios)
		{
			GD.Print("antes de cargar jugador");
			crearMarcador(u);
			GD.Print("despues de cargar jugador");
		}
	}
	
	public void crearMarcador(UsuarioPrueba u)
	{

		// 1. Cargar escena
		var escena = GD.Load<PackedScene>("res://scenes/mapa/marcador.tscn");

		// 2. Instanciar SIN genérico
		var instancia = escena.Instantiate();

		// 3. Probar cast
		var marcador = instancia as Marcador;
		
		//Añadirlo como hijo al mapa
		AddChild(marcador);
		
		//Configurar datos
		marcador.SetData(u.nombre,null);

		//Posicionar en el mapa
		marcador.Position = new Vector2(u.x,u.y);   

		
		GD.Print("se añadió el marcador de "+u.nombre) ;
	}

	
	public List<UsuarioPrueba> CrearUsuariosPrueba()
	{
		List<UsuarioPrueba> listaUsuarios = new();
		listaUsuarios.Add(new UsuarioPrueba("Jean",0,0));
		listaUsuarios.Add(new UsuarioPrueba("Juan",890,400));
		listaUsuarios.Add(new UsuarioPrueba("Pepito",340,800));
		return listaUsuarios;
	}
	
	public class UsuarioPrueba //clase de prueba para cargar usuarios al mapa
	{
		public string nombre;
		public int x;
		public int y;
		
		public UsuarioPrueba(string nombre,int x, int y)
		{
			this.nombre=nombre;
			this.x=x;
			this.y=y;
		}
	}
	
}
