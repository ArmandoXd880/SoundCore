// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-20 | Versión: 1.0
using System.Diagnostics;
using System.Text;
using SoundCore.EstructurasPropias;
using SoundCore.Modelos;

namespace SoundCore.Servicios;

public static class BenchmarkServicio
{
    private record Medicion(double Ms, long Bytes, int Conteo);

    public static string Ejecutar(int cantidad)
    {
        if (cantidad < 1 || cantidad > 100_000) throw new ArgumentOutOfRangeException(nameof(cantidad));
        // Mismas instancias para las tres estructuras. Crear metadatos queda fuera del cronómetro.
        var pistas = new Pista[cantidad];
        var random = new Random(42);
        for (int i = 0; i < cantidad; i++)
            pistas[i] = new Pista(i + 1, $"Pista {i + 1}", "DJ", random.Next(60, 221), 180);
        var cabeza = new Pista(0, "Cabeza", "DJ", 120, 180);
        var salida = new StringBuilder();
        salida.AppendLine($"ESTRÉS: {cantidad:N0} inserciones después de la cabeza");
        salida.AppendLine("3 repeticiones; mediana de tiempo y bytes asignados. Datos precargados, sin GUI.");
        salida.AppendLine("Bytes = asignaciones administradas del hilo, NO memoria total ni pico de RAM.");
        // Calentar cada implementación antes de medir (incluye JIT básico).
        for (int tipo = 0; tipo < 3; tipo++) Medir(tipo, pistas, cabeza, Math.Min(1000, cantidad));
        double[,] tiempos = new double[3,3];
        long[,] bytes = new long[3,3];
        // Rotación del orden para reducir el sesgo por posición.
        for (int ronda = 0; ronda < 3; ronda++)
            for (int paso = 0; paso < 3; paso++)
            {
                int tipo = (paso + ronda) % 3;
                var medida = Medir(tipo, pistas, cabeza, cantidad);
                if (medida.Conteo != cantidad + 1) throw new InvalidOperationException("Conteo de benchmark incorrecto.");
                tiempos[tipo,ronda] = medida.Ms;
                bytes[tipo,ronda] = medida.Bytes;
            }
        string[] nombres = ["Lista propia", ".NET LinkedList<T>", ".NET List<T>"];
        for (int tipo = 0; tipo < 3; tipo++)
        {
            double[] t = [tiempos[tipo,0], tiempos[tipo,1], tiempos[tipo,2]];
            long[] b = [bytes[tipo,0], bytes[tipo,1], bytes[tipo,2]];
            Array.Sort(t); Array.Sort(b);
            salida.AppendLine($"{nombres[tipo],-20}: {t[1],10:N3} ms | {b[1],12:N0} bytes asignados");
        }
        salida.AppendLine();
        salida.AppendLine("Propia: O(1) por inserción; N inserciones = O(N). Reconecta Siguiente.");
        salida.AppendLine("LinkedList: AddAfter con nodo conocido = O(1); total O(N). No incluye buscar el nodo.");
        salida.AppendLine("List: Insert(1) desplaza referencias mediante copia de arreglo: O(n) por inserción.");
        salida.AppendLine("Al crecer de 1 a N elementos, acumula O(N²) desplazamientos; además puede ampliar el búfer.");
        salida.AppendLine("Los resultados dependen del equipo, GC y JIT; no se garantiza un ganador para todo tamaño.");
        salida.AppendLine("La medición excluye la construcción de la cabeza, pero incluye nodos y crecimiento del arreglo.");
        return salida.ToString();
    }

    private static Medicion Medir(int tipo, Pista[] pistas, Pista cabeza, int cantidad)
    {
        var sw = new Stopwatch();
        // Se construye únicamente la estructura que corresponde a esta medición.
        if (tipo == 0)
        {
            var lista = new ListaSimpleEnlazada<Pista>(); lista.AgregarAlFinal(cabeza);
            long antes = GC.GetAllocatedBytesForCurrentThread(); sw.Start();
            for (int i = 0; i < cantidad; i++) lista.ReproducirSiguiente(pistas[i]);
            sw.Stop(); long asignados = GC.GetAllocatedBytesForCurrentThread() - antes;
            GC.KeepAlive(lista);
            return new Medicion(sw.Elapsed.TotalMilliseconds, asignados, lista.Conteo);
        }
        if (tipo == 1)
        {
            var lista = new LinkedList<Pista>(); var nodoCabeza = lista.AddFirst(cabeza);
            long antes = GC.GetAllocatedBytesForCurrentThread(); sw.Start();
            for (int i = 0; i < cantidad; i++) lista.AddAfter(nodoCabeza, pistas[i]);
            sw.Stop(); long asignados = GC.GetAllocatedBytesForCurrentThread() - antes;
            GC.KeepAlive(lista);
            return new Medicion(sw.Elapsed.TotalMilliseconds, asignados, lista.Count);
        }
        else
        {
            var lista = new List<Pista> { cabeza };
            long antes = GC.GetAllocatedBytesForCurrentThread(); sw.Start();
            for (int i = 0; i < cantidad; i++) lista.Insert(1, pistas[i]);
            sw.Stop(); long asignados = GC.GetAllocatedBytesForCurrentThread() - antes;
            GC.KeepAlive(lista);
            return new Medicion(sw.Elapsed.TotalMilliseconds, asignados, lista.Count);
        }
    }
}
