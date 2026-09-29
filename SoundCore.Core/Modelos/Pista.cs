// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-20 | Versión: 1.0
namespace SoundCore.Modelos;

public record Pista(int Id, string Titulo, string Artista, int Bpm, int DuracionSegundos)
{
    // Solo se abre el archivo de la pista que está sonando; las listas guardan rutas.
    public string? RutaArchivo { get; init; }

    public override string ToString() =>
        $"[{Id:D3}] {Titulo} - {Artista} | {Bpm} BPM ({DuracionSegundos}s)";
}
