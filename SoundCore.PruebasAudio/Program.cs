// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-28 | Versión: 2.0
using NAudio.Wave;
using SoundCore.UI.Audio;

int pruebas = 0;
void Verificar(bool valor, string nombre)
{
    if (!valor) throw new Exception("FALLÓ: " + nombre);
    pruebas++;
}
string wav = Path.Combine(AppContext.BaseDirectory,"AudioDemo","tono-a.wav");
SalidaFalsa salida = new();
var audio = new ReproductorAudio(() => salida = new SalidaFalsa());
int fines = 0; Exception? errorAudio = null;
audio.Finalizada += error => { fines++; errorAudio = error; };
await audio.AbrirYReproducirAsync(wav);
Verificar(audio.Estado == PlaybackState.Playing && audio.Cargado,"Inicio");
Verificar(Math.Abs(audio.Duracion.TotalSeconds-3)<0.01,"Duración WAV real");
audio.Volumen = 0.25f; Verificar(audio.Volumen==0.25f,"Volumen");
audio.Pausar(); Verificar(audio.Estado==PlaybackState.Paused,"Pausa");
audio.Buscar(0.5); Verificar(Math.Abs(audio.Posicion.TotalSeconds-1.5)<0.01,"Posición");
audio.Continuar(); Verificar(audio.Estado==PlaybackState.Playing,"Continuar");
var anterior = salida;
await audio.AbrirYReproducirAsync(wav);
Verificar(anterior.Liberada && fines==0,"Saltar libera salida sin fin natural");
// Un callback tardío de una salida descartada no debe avanzar la nueva cola.
anterior.Terminar(); Verificar(fines==0,"Ignorar evento antiguo");
salida.Terminar(); Verificar(fines==1 && errorAudio==null,"Fin natural");
await audio.DetenerAsync(); Verificar(!audio.Cargado && salida.Liberada,"Liberación al terminar");
await audio.AbrirYReproducirAsync(wav);
salida.Terminar(new IOException("Dispositivo desconectado"));
Verificar(fines==2 && errorAudio is IOException,"Error de salida");
await audio.DetenerAsync();
await audio.AbrirYReproducirAsync(wav);
try { await audio.AbrirYReproducirAsync(wav+".inexistente"); throw new Exception("Debió fallar"); }
catch(FileNotFoundException) { Verificar(audio.Estado==PlaybackState.Playing,"Archivo ausente preserva actual"); }
await audio.DetenerAsync(); await audio.DetenerAsync();
Verificar(!audio.Cargado && fines==2,"Stop idempotente sin avance");
var mala = new SalidaFalsa { FallarInit=true };
var motorMalo = new ReproductorAudio(() => mala);
try { await motorMalo.AbrirYReproducirAsync(wav); throw new Exception("Debió fallar"); }
catch(InvalidOperationException) { Verificar(mala.Liberada && !motorMalo.Cargado,"Error de Init libera recursos"); }
// Stop debe esperar el callback del dispositivo antes de cerrar el lector.
var diferida = new SalidaFalsa { DiferirStop = true };
var motorDiferido = new ReproductorAudio(() => diferida);
await motorDiferido.AbrirYReproducirAsync(wav);
Task parada = motorDiferido.DetenerAsync();
Verificar(!parada.IsCompleted && !diferida.Liberada,"Esperar callback de Stop");
diferida.Terminar(); await parada;
Verificar(diferida.Liberada,"Liberar después del callback");
// Leer realmente muestras PCM: la demo contiene sonido, no silencio.
using(var lector = new AudioFileReader(wav))
{
    float[] muestras = new float[4096]; int leidas=lector.Read(muestras,0,muestras.Length);
    Verificar(leidas>0 && muestras.Any(x=>Math.Abs(x)>0.05),"Decodificación WAV no silenciosa");
}
Console.WriteLine($"OK: {pruebas} pruebas de audio (salida simulada, lectura WAV real). No verifica altavoces ni MP3 en Windows.");

sealed class SalidaFalsa : IWavePlayer
{
    public event EventHandler<StoppedEventArgs>? PlaybackStopped;
    public PlaybackState PlaybackState { get; private set; }
    public float Volume { get; set; }
    public WaveFormat OutputWaveFormat { get; private set; } = new WaveFormat();
    public bool Liberada { get; private set; }
    public bool FallarInit { get; init; }
    public bool DiferirStop { get; init; }
    public void Init(IWaveProvider proveedor)
    {
        if(FallarInit) throw new InvalidOperationException("Error de prueba");
        OutputWaveFormat=proveedor.WaveFormat;
    }
    public void Play() => PlaybackState=PlaybackState.Playing;
    public void Pause() => PlaybackState=PlaybackState.Paused;
    public void Stop() { if (!DiferirStop) Terminar(); }
    public void Terminar(Exception? error=null)
    { PlaybackState=PlaybackState.Stopped; PlaybackStopped?.Invoke(this,new StoppedEventArgs(error)); }
    public void Dispose() { Liberada=true; }
}
