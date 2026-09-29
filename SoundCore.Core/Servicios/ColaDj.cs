// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-20 | Versión: 1.0
using SoundCore.EstructurasPropias;
using SoundCore.Modelos;

namespace SoundCore.Servicios;

public enum TipoEstructura { Propia, LinkedList, List }

// Tres colas independientes: cambiar el radio no borra ni convierte los datos.
public class ColaDj
{
    public ListaSimpleEnlazada<Pista> Propia { get; } = new();
    public LinkedList<Pista> Enlazada { get; } = new();
    public List<Pista> Arreglo { get; } = new();
    private readonly Pista?[] sonando = new Pista?[3];
    public TipoEstructura Modo { get; set; }
    public Pista? Sonando => sonando[(int)Modo];
    public IEnumerable<Pista> Elementos => Modo switch
    {
        TipoEstructura.Propia => Propia,
        TipoEstructura.LinkedList => Enlazada,
        _ => Arreglo
    };
    public void Agregar(Pista pista, bool siguiente = false)
    {
        switch (Modo)
        {
            case TipoEstructura.Propia:
                if (siguiente) Propia.ReproducirSiguiente(pista); else Propia.AgregarAlFinal(pista);
                break;
            case TipoEstructura.LinkedList:
                if (siguiente && Enlazada.First != null) Enlazada.AddAfter(Enlazada.First, pista);
                else Enlazada.AddLast(pista);
                break;
            default:
                if (siguiente && Arreglo.Count > 0) Arreglo.Insert(1, pista); else Arreglo.Add(pista);
                break;
        }
    }
    public Pista Avanzar()
    {
        Pista pista;
        switch (Modo)
        {
            case TipoEstructura.Propia: pista = Propia.AvanzarPista(); break;
            case TipoEstructura.LinkedList:
                if (Enlazada.First == null) throw new InvalidOperationException("La cola está vacía.");
                pista = Enlazada.First.Value; Enlazada.RemoveFirst(); break;
            default:
                if (Arreglo.Count == 0) throw new InvalidOperationException("La cola está vacía.");
                pista = Arreglo[0]; Arreglo.RemoveAt(0); break;
        }
        sonando[(int)Modo] = pista;
        return pista;
    }
    public void Invertir()
    {
        if (Modo == TipoEstructura.Propia) Propia.Invertir();
        else if (Modo == TipoEstructura.List) Arreglo.Reverse();
        else
        {
            // Se mueven los nodos nativos; no se crea una colección temporal.
            var actual = Enlazada.First?.Next;
            while (actual != null)
            {
                var siguiente = actual.Next;
                Enlazada.Remove(actual); Enlazada.AddFirst(actual);
                actual = siguiente;
            }
        }
    }
    public void Ordenar()
    {
        if (Modo == TipoEstructura.Propia) Propia.Ordenar((a,b) => a.Bpm.CompareTo(b.Bpm));
        else if (Modo == TipoEstructura.List) Arreglo.Sort((a,b) => a.Bpm.CompareTo(b.Bpm));
        else
        {
            var actual = Enlazada.First?.Next;
            while (actual != null)
            {
                var siguiente = actual.Next;
                var previo = actual.Previous;
                while (previo != null && previo.Value.Bpm > actual.Value.Bpm) previo = previo.Previous;
                Enlazada.Remove(actual);
                if (previo == null) Enlazada.AddFirst(actual); else Enlazada.AddAfter(previo, actual);
                actual = siguiente;
            }
        }
    }
    private static bool Iguales(Pista a, Pista b) =>
        string.Equals(a.Titulo.Trim(), b.Titulo.Trim(), StringComparison.OrdinalIgnoreCase);
    public void Purgar()
    {
        if (Modo == TipoEstructura.Propia) Propia.DepurarDuplicados(Iguales);
        else if (Modo == TipoEstructura.LinkedList)
        {
            for (var actual = Enlazada.First; actual != null; actual = actual.Next)
                for (var corredor = actual.Next; corredor != null;)
                {
                    var siguiente = corredor.Next;
                    if (Iguales(actual.Value, corredor.Value)) Enlazada.Remove(corredor);
                    corredor = siguiente;
                }
        }
        else
            for (int i = 0; i < Arreglo.Count; i++)
                for (int j = i + 1; j < Arreglo.Count;)
                    if (Iguales(Arreglo[i], Arreglo[j])) Arreglo.RemoveAt(j); else j++;
    }
    public void Limpiar()
    {
        if (Modo == TipoEstructura.Propia) Propia.Limpiar();
        else if (Modo == TipoEstructura.LinkedList) Enlazada.Clear();
        else Arreglo.Clear();
        sonando[(int)Modo] = null;
    }
}
