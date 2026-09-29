// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-20 | Versión: 1.0
using SoundCore.EstructurasPropias;
using SoundCore.Modelos;
using SoundCore.Servicios;

int verificaciones = 0;
void Verificar(bool condicion, string mensaje)
{
    if (!condicion) throw new Exception("FALLÓ: " + mensaje);
    verificaciones++;
}
void Secuencia<T>(IEnumerable<T> real, IEnumerable<T> esperado, string mensaje) =>
    Verificar(real.SequenceEqual(esperado), mensaje);

// CP-01 a CP-07 en las tres implementaciones utilizadas por la GUI.
foreach (TipoEstructura modo in Enum.GetValues<TipoEstructura>())
{
    var cola = new ColaDj { Modo = modo };
    Pista a = new(1,"Track 1","DJ",100,200), b = new(2,"Track 2","DJ",110,180), c = new(3,"Track 3","DJ",120,240);
    cola.Agregar(a); cola.Agregar(c); cola.Agregar(b,true);
    Secuencia(cola.Elementos, new[] { a,b,c }, $"{modo}: CP-01/02");
    Verificar(cola.Avanzar() == a && cola.Sonando == a, $"{modo}: CP-03");
    Secuencia(cola.Elementos, new[] { b,c }, "La cabeza sale de la cola");
    cola.Limpiar(); cola.Agregar(a); cola.Agregar(b); cola.Agregar(c); cola.Invertir();
    Secuencia(cola.Elementos.Select(p => p.Bpm), new[] { 120,110,100 }, $"{modo}: CP-04");
    cola.Limpiar();
    foreach (int bpm in new[] {128,115,140,120}) cola.Agregar(new Pista(bpm,$"Tema {bpm}","DJ",bpm,180));
    cola.Ordenar();
    Secuencia(cola.Elementos.Select(p => p.Bpm), new[] {115,120,128,140}, $"{modo}: CP-05");
    cola.Limpiar(); cola.Agregar(a); cola.Agregar(b); cola.Agregar(a with { Id=4,Titulo=" TRACK 1 " });
    cola.Agregar(b with { Id=5 }); cola.Purgar(); cola.Agregar(c);
    Secuencia(cola.Elementos, new[] {a,b,c}, $"{modo}: CP-06 y cola tras purga");
    cola.Limpiar();
    bool lanzo = false;
    try { cola.Avanzar(); } catch (InvalidOperationException) { lanzo = true; }
    Verificar(lanzo, $"{modo}: CP-07 (excepción que captura la GUI)");
    cola.Invertir(); cola.Ordenar(); cola.Purgar(); cola.Agregar(a,true); cola.Invertir();
    Verificar(cola.Avanzar() == a, "Vacío / unitario / Up Next");
    cola.Agregar(b); Verificar(cola.Avanzar() == b, "Reutilización tras vaciar");
}

var lista = new ListaSimpleEnlazada<int>();
lista.AgregarAlFinal(1); lista.AgregarAlFinal(2); lista.AgregarAlFinal(3);
var n1 = lista.Cabeza!; var n2 = n1.Siguiente!; var n3 = n2.Siguiente!;
lista.Invertir();
Verificar(ReferenceEquals(lista.Cabeza,n3) && ReferenceEquals(n3.Siguiente,n2) &&
    ReferenceEquals(n2.Siguiente,n1) && n1.Siguiente == null, "Inversión conserva identidades y cambia enlaces");
Verificar(n1.Valor == 1 && n2.Valor == 2 && n3.Valor == 3, "No intercambia valores");
lista.AgregarAlFinal(4); Secuencia(lista,new[] {3,2,1,4},"Cola actualizada después de invertir");
lista.Ordenar((a,b)=>a.CompareTo(b));
Verificar(ReferenceEquals(lista.Cabeza,n1) && ReferenceEquals(n1.Siguiente,n2),"Ordenar reutiliza nodos");
lista.InsertarOrdenado(0,(a,b)=>a.CompareTo(b)); lista.InsertarOrdenado(5,(a,b)=>a.CompareTo(b));
lista.InsertarOrdenado(2,(a,b)=>a.CompareTo(b));
Secuencia(lista,new[] {0,1,2,2,3,4,5},"Inserción ordenada: cabeza, medio, final e igualdad");
lista.DepurarDuplicados((a,b)=>a==b); lista.AgregarAlFinal(6);
Secuencia(lista,new[] {0,1,2,3,4,5,6},"Purga e inserción al final");
Verificar(lista.Conteo==7,"Conteo tras purga");

// Secuencias mixtas detectan enlaces perdidos, ciclos y colas obsoletas.
var random = new Random(42);
lista.Limpiar(); var referencia = new List<int>();
for (int i=0;i<2000;i++)
{
    int valor=random.Next(20);
    switch(random.Next(7))
    {
        case 0: lista.AgregarAlFinal(valor); referencia.Add(valor); break;
        case 1: lista.ReproducirSiguiente(valor); referencia.Insert(referencia.Count==0 ? 0 : 1,valor); break;
        case 2: lista.Invertir(); referencia.Reverse(); break;
        case 3: lista.Ordenar((a,b)=>a.CompareTo(b)); referencia.Sort();
            lista.InsertarOrdenado(valor,(a,b)=>a.CompareTo(b)); referencia.Add(valor); referencia.Sort(); break;
        case 4: lista.DepurarDuplicados((a,b)=>a==b); referencia=referencia.Distinct().ToList(); break;
        case 5: if(referencia.Count>0) { Verificar(lista.AvanzarPista()==referencia[0],"Avanzar mixto"); referencia.RemoveAt(0); } break;
        case 6: if(i%13==0) {lista.Limpiar(); referencia.Clear();} break;
    }
    // Take acota el recorrido incluso si hubiera un ciclo.
    Secuencia(lista.Take(referencia.Count+1),referencia,"Secuencia aleatoria");
    Verificar(lista.Conteo==referencia.Count,"Conteo aleatorio");
}
// Benchmark ejecutado realmente, no valores de muestra incrustados.
string resultado = BenchmarkServicio.Ejecutar(25000);
Verificar(resultado.Contains("LinkedList<T>") && resultado.Contains("List<T>"),"Telemetría de las tres estructuras");
Console.WriteLine($"OK: {verificaciones} verificaciones superadas.");
Console.WriteLine(resultado);
