// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-28 | Versión: 2.0 (audio real)
using NAudio.Wave;

namespace SoundCore.UI.Audio;

// Se usa exclusivamente desde el hilo de interfaz. NAudio procesa audio en otro hilo.
// No utiliza Windows Media Player, AxWindowsMediaPlayer, COM ni ActiveX.
public sealed class ReproductorAudio
{
    private sealed class Sesion
    {
        public required AudioFileReader Lector { get; init; }
        public required IWavePlayer Salida { get; init; }
        public TaskCompletionSource<bool> Detenida { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public bool ParadaSolicitada { get; set; }
        public bool Iniciada { get; set; }
    }
    private readonly Func<IWavePlayer> crearSalida;
    public ReproductorAudio() : this(() => new WaveOutEvent()) { }
    // Permite probar el ciclo de vida sin una tarjeta de sonido.
    public ReproductorAudio(Func<IWavePlayer> crearSalida) { this.crearSalida = crearSalida; }
    private Sesion? sesion;
    private float volumen = 0.7f;
    public event Action<Exception?>? Finalizada;
    public bool Cargado => sesion != null;
    public PlaybackState Estado => sesion?.Salida.PlaybackState ?? PlaybackState.Stopped;
    public TimeSpan Posicion => sesion?.Lector.CurrentTime ?? TimeSpan.Zero;
    public TimeSpan Duracion => sesion?.Lector.TotalTime ?? TimeSpan.Zero;
    public float Volumen
    {
        get => volumen;
        set { volumen = Math.Clamp(value, 0f, 1f); if (sesion != null) sesion.Lector.Volume = volumen; }
    }
    public async Task AbrirYReproducirAsync(string ruta)
    {
        if (!File.Exists(ruta)) throw new FileNotFoundException("No se encontró el archivo de audio.", ruta);
        // Abrir antes de detener permite detectar archivos inválidos conservando la pista anterior.
        var lector = new AudioFileReader(ruta) { Volume = volumen };
        IWavePlayer salida;
        try { salida = crearSalida(); }
        catch { lector.Dispose(); throw; }
        var nueva = new Sesion { Lector = lector, Salida = salida };
        salida.PlaybackStopped += (_, e) =>
        {
            nueva.Detenida.TrySetResult(true);
            if (ReferenceEquals(sesion, nueva) && !nueva.ParadaSolicitada)
                Finalizada?.Invoke(e.Exception);
        };
        try
        {
            await DetenerAsync();
            salida.Init(lector);
            sesion = nueva;
            salida.Play();
            nueva.Iniciada = true;
        }
        catch
        {
            if (ReferenceEquals(sesion, nueva)) await DetenerAsync();
            else { salida.Dispose(); lector.Dispose(); }
            throw;
        }
    }
    public void Pausar() { if (Estado == PlaybackState.Playing) sesion!.Salida.Pause(); }
    public void Continuar() { if (Estado == PlaybackState.Paused) sesion!.Salida.Play(); }
    public void Buscar(double proporcion)
    {
        if (sesion == null || Estado == PlaybackState.Stopped) return;
        sesion.Lector.CurrentTime = TimeSpan.FromSeconds(Duracion.TotalSeconds * Math.Clamp(proporcion, 0, 0.999));
    }
    public async Task DetenerAsync()
    {
        var anterior = sesion;
        if (anterior == null) return;
        sesion = null;
        anterior.ParadaSolicitada = true;
        // Esperar a PlaybackStopped antes de liberar el lector usado por el hilo de audio.
        if (anterior.Iniciada && !anterior.Detenida.Task.IsCompleted)
        {
            anterior.Salida.Stop();
            await anterior.Detenida.Task;
        }
        anterior.Salida.Dispose();
        anterior.Lector.Dispose();
    }
}
