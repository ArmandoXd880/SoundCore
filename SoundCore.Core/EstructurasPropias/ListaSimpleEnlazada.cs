// Integrantes: Jesus Armando Zendejo Altamirano
// Números de control TecNM: I24050394
// Fecha: 2026-09-20 | Versión: 1.0
using System.Collections;

namespace SoundCore.EstructurasPropias;

// Basada en Head, Next y el recorrido current del ejemplo de rriojas/edSimpleLinkedList.
// No contiene arreglos, List<T>, HashSet ni colecciones auxiliares.
public class ListaSimpleEnlazada<T> : IEnumerable<T>
{
    public Nodo<T>? Cabeza { get; private set; }
    private Nodo<T>? cola;
    public int Conteo { get; private set; }
    public bool EstaVacia => Cabeza == null;

    // O(1): conservar la cola evita recorrer todos los nodos.
    public void AgregarAlFinal(T valor)
    {
        Nodo<T> nuevo = new Nodo<T>(valor);
        if (cola == null)
        {
            Cabeza = nuevo;
        }
        else
        {
            cola.Siguiente = nuevo;
        }

        cola = nuevo;
        Conteo++;
    }

    // O(1): el nuevo elemento queda en la posición 2; en vacío será la cabeza.
    public void ReproducirSiguiente(T valor)
    {
        if (Cabeza == null) { AgregarAlFinal(valor); return; }
        Nodo<T> nuevo = new Nodo<T>(valor) { Siguiente = Cabeza.Siguiente };
        Cabeza.Siguiente = nuevo;
        if (cola == Cabeza)
        {
            cola = nuevo;
        }

        Conteo++;
    }

    // O(1): devuelve y desconecta la antigua cabeza.
    public T AvanzarPista()
    {
        if (Cabeza == null)
        {
            throw new InvalidOperationException("La cola está vacía.");
        }

        var eliminado = Cabeza;
        Cabeza = eliminado.Siguiente;
        eliminado.Siguiente = null;
        if (Cabeza == null)
        {
            cola = null;
        }

        Conteo--;
        return eliminado.Valor;
    }

    // O(n) tiempo, O(1) espacio auxiliar: mismos nodos y mismos valores.
    public void Invertir()
    {
        cola = Cabeza; // La antigua cabeza será el último nodo.
        Nodo<T>? previo = null;
        Nodo<T>? actual = Cabeza;
        Nodo<T>? siguiente = null;
        while (actual != null)
        {
            siguiente = actual.Siguiente; // 1. Conservar el resto de la cadena.
            actual.Siguiente = previo;    // 2. Dar vuelta al enlace.
            previo = actual;              // 3. Avanzar el extremo invertido.
            actual = siguiente;           // 4. Continuar con el resto guardado.
        }
        Cabeza = previo;                  // 5. El último nodo pasa a ser cabeza.
    }

    // O(n). Precondición: la lista ya está ordenada con este comparador.
    public void InsertarOrdenado(T valor, Comparison<T> comparador)
    {
        ArgumentNullException.ThrowIfNull(comparador);
        EnlazarOrdenado(new Nodo<T>(valor), comparador);
        Conteo++;
    }

    private void EnlazarOrdenado(Nodo<T> nuevo, Comparison<T> comparador)
    {
        if (Cabeza == null || comparador(nuevo.Valor, Cabeza.Valor) < 0)
        {
            nuevo.Siguiente = Cabeza;
            Cabeza = nuevo;
            if (cola == null)
            {
                cola = nuevo;
            }

            return;
        }
        var actual = Cabeza;
        while (actual.Siguiente != null && comparador(nuevo.Valor, actual.Siguiente.Valor) >= 0)
        {
            actual = actual.Siguiente;
        }

        nuevo.Siguiente = actual.Siguiente;
        actual.Siguiente = nuevo;
        if (nuevo.Siguiente == null)
        {
            cola = nuevo;
        }
    }

    // Ordenamiento por inserción estable: O(n²), O(1) auxiliar.
    // Reutiliza nodos existentes; no copia pistas ni construye otra lista.
    public void Ordenar(Comparison<T> comparador)
    {
        ArgumentNullException.ThrowIfNull(comparador);
        var pendiente = Cabeza;
        Cabeza = cola = null;
        while (pendiente != null)
        {
            var siguiente = pendiente.Siguiente;
            pendiente.Siguiente = null;
            EnlazarOrdenado(pendiente, comparador);
            pendiente = siguiente;
        }
    }

    // O(n²), O(1) auxiliar. Conserva la primera aparición.
    public void DepurarDuplicados(Func<T, T, bool> sonIguales)
    {
        ArgumentNullException.ThrowIfNull(sonIguales);
        var actual = Cabeza;
        while (actual != null)
        {
            var corredor = actual;
            while (corredor.Siguiente != null)
            {
                if (sonIguales(actual.Valor, corredor.Siguiente.Valor))
                {
                    var eliminado = corredor.Siguiente;
                    corredor.Siguiente = eliminado.Siguiente;
                    if (cola == eliminado)
                    {
                        cola = corredor;
                    }

                    eliminado.Siguiente = null;
                    Conteo--;
                }
                else
                {
                    corredor = corredor.Siguiente;
                }
            }
            actual = actual.Siguiente;
        }
    }

    public void Limpiar() { Cabeza = cola = null; Conteo = 0; }
    public IEnumerator<T> GetEnumerator()
    {
        var actual = Cabeza;
        while (actual != null) { yield return actual.Valor; actual = actual.Siguiente; }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
