// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-20 | Versión: 1.0
namespace SoundCore.EstructurasPropias;

// Evolución de Node.Next del ejemplo del profesor: ahora el dato es genérico.
public class Nodo<T>
{
    public T Valor { get; set; }
    public Nodo<T>? Siguiente { get; set; }
    public Nodo(T valor) { Valor = valor; }
}
