using Godot;
using System;
using System.Collections.Generic;

public partial class MainMapa : Node2D
{
	[Export] public Sprite2D MapaMundial;
	public override void _Ready()
	{
		// Carga los usuario previamente creados y los coloca en el mapa
		var usuarios = CrearUsuariosPrueba();
		foreach(UsuarioPrueba u in usuarios)
		{
			
			GD.Print("se va a crear el usuario") ;
			crearMarcador(u);
			GD.Print("se creo el usuario") ;
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
		marcador.Position =ConvertirCordenadas(u.x,u.y);   
		
		GD.Print("se añadió el marcador de "+u.nombre) ;
	}
	
	public Vector2 ConvertirCordenadas(float lat, float lon)
	{
		float baseWidth=MapaMundial.Scale.X;
		float baseHeight=MapaMundial.Scale.Y;
		float width = MapaMundial.Texture.GetSize().X*baseWidth;
		float height = MapaMundial.Texture.GetSize().Y*baseHeight;
		 // Posición del mapa en el mundo de Godot
		Vector2 mapPos = MapaMundial.Position;

		// Conversión dentro del mapa sin offset
		float x = (lon + 180f) / 360f * width;
		float y = (90f - lat) / 180f * height;

		// Ajuste porque el mapa está centrado
		x -= width / 2f;
		y -= height / 2f;

		// Ajuste por la posición del mapa
		x += mapPos.X;
		y += mapPos.Y;
		return new Vector2(x, y);
	}

	//A partir de aqui son funciones de prueba
	public List<UsuarioPrueba> CrearUsuariosPrueba()
	{
		List<UsuarioPrueba> listaUsuarios = new();
		listaUsuarios.Add(new UsuarioPrueba("Jean",0f,0f));
		listaUsuarios.Add(new UsuarioPrueba("Juan",9.857503f, -83.897971f));
		listaUsuarios.Add(new UsuarioPrueba("Pepito",60.770374f, 97.469550f));
		listaUsuarios.Add(new UsuarioPrueba("islandia",64.882908f, -18.191032f));
		return listaUsuarios;
	}
	
	public class UsuarioPrueba //clase de prueba para cargar usuarios al mapa
	{
		public string nombre;
		public float x;
		public float y;
		
		public UsuarioPrueba(string nombre,float x, float y)
		{
			this.nombre=nombre;
			this.x=x;
			this.y=y;
		}
	}
}
