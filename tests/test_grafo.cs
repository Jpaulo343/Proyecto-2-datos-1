using NUnit.Framework;

[TestFixture]
public class GrafoTests
{
	private FamilyMember jean;
	private FamilyMember juan;
	private FamilyMember pepito;
	private FamilyMember islandia;
	private GrafoFamilia grafo;

	[SetUp]
	public void Setup()
	{
		grafo = new GrafoFamilia();

		jean = new FamilyMember("1", "Jean", 9.9, -84.0);
		juan = new FamilyMember("2", "Juan", 0, 0);
		pepito = new FamilyMember("3", "Pepito", 60.7, 97.4);
		islandia = new FamilyMember("4", "Islandia", 64.88, -18.19);

		grafo.AgregarArista(jean, juan);
		grafo.AgregarArista(juan, pepito);
		grafo.AgregarArista(pepito, islandia);
	}

	[Test]
	public void Test_Se_Agregan_Los_Nodos_Correctamente()
	{
		var miembros = grafo.ObtenerTodosLosMiembros();

		Assert.That(miembros, Has.Exactly(4).Items);
		Assert.That(miembros, Does.Contain(jean));
		Assert.That(miembros, Does.Contain(juan));
		Assert.That(miembros, Does.Contain(pepito));
		Assert.That(miembros, Does.Contain(islandia));
	}

	[Test]
	public void Test_Aristas_Estan_Bien_Creadas()
	{
		var vecinos_jean = grafo.ObtenerVecinos(jean);
		var vecinos_juan = grafo.ObtenerVecinos(juan);

		Assert.That(vecinos_jean, Does.Contain(juan));
		Assert.That(vecinos_juan, Does.Contain(jean));

		Assert.That(grafo.ObtenerVecinos(juan), Does.Contain(pepito));
		Assert.That(grafo.ObtenerVecinos(pepito), Does.Contain(juan));
	}

	[Test]
	public void Test_CalcularDistancia_Da_Valor_Positivo()
	{
		double d = grafo.CalcularDistancia(jean, juan);
		Assert.That(d, Is.GreaterThan(0));
	}

	[Test]
	public void Test_ParMasLejano_Funciona()
	{
		var (a, b, dist) = grafo.ParMasLejano();

		Assert.That(a, Is.Not.Null);
		Assert.That(b, Is.Not.Null);
		Assert.That(dist, Is.GreaterThan(0));
	}

	[Test]
	public void Test_DistanciaPromedio_Funciona()
	{
		double promedio = grafo.DistanciaPromedio();
		Assert.That(promedio, Is.GreaterThan(0));
	}
}
