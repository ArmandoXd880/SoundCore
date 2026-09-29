// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-20 | Versión: 1.0
namespace SoundCore.UI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
