// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-28 | Versión: 2.0 (audio real)
using NAudio.Wave;
using SoundCore.Modelos;
using SoundCore.UI.Audio;

namespace SoundCore.UI;

public partial class MainForm
{
    private readonly ReproductorAudio audio = new();
    private readonly System.Windows.Forms.Timer relojAudio = new() { Interval = 250 };
    private string? rutaSeleccionada;
    private Pista? pistaAudible;
    private string estadoAudio = "Detenido";
    private bool accionAudio, cerrando, permitirCierre, buscando;
    private Button btnArchivo = null!, btnImportar = null!, btnPlay = null!, btnPausa = null!, btnStop = null!;
    private Label lblArchivo = null!, lblTiempo = null!;
    private TrackBar barraPosicion = null!, barraVolumen = null!;

    private void ConfigurarAudio()
    {
        btnArchivo.Click += (_, _) => SeleccionarArchivo();
        btnImportar.Click += (_, _) => ImportarArchivos();
        btnPlay.Click += async (_, _) => await AccionAudioAsync(async () =>
        {
            if (audio.Estado == PlaybackState.Paused) { audio.Continuar(); estadoAudio = "Sonando"; }
            else if (audio.Estado != PlaybackState.Playing)
            {
                if (cola.Sonando != null && !string.IsNullOrWhiteSpace(cola.Sonando.RutaArchivo))
                    await ReproducirAsync(cola.Sonando);
                else await AvanzarAudioAsync(false);
            }
            ActualizarReproductor();
        });
        btnPausa.Click += (_, _) => { audio.Pausar(); if (audio.Cargado) estadoAudio = "En pausa"; ActualizarReproductor(); };
        btnStop.Click += async (_, _) => await AccionAudioAsync(async () =>
        { await audio.DetenerAsync(); estadoAudio = "Detenido"; ActualizarReproductor(); });
        barraVolumen.ValueChanged += (_, _) => audio.Volumen = barraVolumen.Value / 100f;
        barraPosicion.MouseDown += (_, _) => buscando = true;
        barraPosicion.MouseUp += (_, _) => { audio.Buscar(barraPosicion.Value / 1000.0); buscando = false; };
        barraPosicion.KeyUp += (_, _) => audio.Buscar(barraPosicion.Value / 1000.0);
        relojAudio.Tick += (_, _) => ActualizarReproductor();
        relojAudio.Start();
        // NAudio publica el evento en el SynchronizationContext de WinForms.
        audio.Finalizada += async error =>
        {
            if (cerrando || accionAudio) return;
            await AccionAudioAsync(async () =>
            {
                await audio.DetenerAsync();
                if (error != null) throw new InvalidOperationException("Falló la salida de audio: " + error.Message, error);
                estadoAudio = "Finalizada";
                await AvanzarAudioAsync(true);
            });
        };
        FormClosing += async (_, e) =>
        {
            if (permitirCierre) return;
            e.Cancel = true;
            if (cerrando) return;
            cerrando = true; Enabled = false; relojAudio.Stop();
            // Una transición de canción puede estar esperando a PlaybackStopped.
            while (accionAudio) await Task.Delay(20);
            try { await audio.DetenerAsync(); }
            finally { relojAudio.Dispose(); permitirCierre = true; Close(); }
        };
    }
    private async Task AccionAudioAsync(Func<Task> accion)
    {
        if (accionAudio || cerrando) return;
        accionAudio = true; HabilitarAudio(false);
        try { await accion(); }
        catch (Exception ex)
        {
            estadoAudio = audio.Estado == PlaybackState.Playing ? "Sonando" : "Error de audio";
            MessageBox.Show(this, ex.Message, "Reproductor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally { accionAudio = false; if (!cerrando) HabilitarAudio(true); ActualizarReproductor(); }
    }
    private void HabilitarAudio(bool valor)
    {
        foreach (Control control in new Control[] {rbPropia,rbLinkedList,rbList,btnPlay,btnPausa,btnStop,btnAvanzar,
            btnLimpiar,btnEncolarFinal,btnReproducirSiguiente,btnInvertir,btnOrdenarBpm,btnInsertarOrdenado,btnPurgar,btnDemo,btnImportar,btnArchivo})
            control.Enabled = valor;
    }
    private async Task ReproducirAsync(Pista pista)
    {
        if (string.IsNullOrWhiteSpace(pista.RutaArchivo))
            throw new InvalidOperationException("Esta pista solo tiene datos. Carga un MP3 o WAV antes de encolarla.");
        await audio.AbrirYReproducirAsync(pista.RutaArchivo);
        pistaAudible = pista; estadoAudio = "Sonando";
    }
    private async Task AvanzarAudioAsync(bool automatico)
    {
        // Peek sin eliminar: si falla el archivo, la pista permanece en la cola.
        Pista? siguiente = cola.Elementos.FirstOrDefault();
        if (siguiente == null)
        {
            if (!automatico) MessageBox.Show(this, "No hay pistas pendientes en la cola.", "Fin del setlist", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ActualizarReproductor(); return;
        }
        if (string.IsNullOrWhiteSpace(siguiente.RutaArchivo))
        {
            // Conserva el caso académico de metadatos, sin afirmar que existe sonido.
            await audio.DetenerAsync(); cola.Avanzar(); pistaAudible = null; estadoAudio = "Sin archivo";
            RefrescarVista();
            if (!automatico) MessageBox.Show(this, "Se avanzó la pista de práctica, pero no tiene audio. Usa Seleccionar audio y encola un archivo para escuchar música.", "Pista sin archivo");
            return;
        }
        await ReproducirAsync(siguiente);
        cola.Avanzar();
        RefrescarVista();
    }
    private void ActualizarReproductor()
    {
        Pista? pista = pistaAudible ?? cola.Sonando;
        lblNowPlaying.Text = pista == null ? "Sin reproducción · carga audio y pulsa Play" :
            $"{estadoAudio}: {pista.Titulo} — {pista.Artista} ({pista.Bpm} BPM)";
        lblTiempo.Text = $"{Duracion((long)audio.Posicion.TotalSeconds)} / {Duracion((long)audio.Duracion.TotalSeconds)}";
        if (!buscando) barraPosicion.Value = audio.Duracion.TotalSeconds > 0 ?
            Math.Clamp((int)(audio.Posicion.TotalSeconds / audio.Duracion.TotalSeconds * 1000),0,1000) : 0;
    }
    private static OpenFileDialog DialogoAudio(bool varios) => new()
    {
        Title = "Selecciona archivos de audio locales", Multiselect = varios, CheckFileExists = true,
        Filter = "Audio MP3 o WAV|*.mp3;*.wav|Todos los archivos|*.*"
    };
    private void SeleccionarArchivo()
    {
        using var dialogo = DialogoAudio(false);
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            using var lector = new AudioFileReader(dialogo.FileName);
            rutaSeleccionada = dialogo.FileName;
            txtTitulo.Text = Path.GetFileNameWithoutExtension(rutaSeleccionada);
            if (string.IsNullOrWhiteSpace(txtArtista.Text)) txtArtista.Text = "Desconocido";
            numDuracion.Value = Math.Clamp((decimal)Math.Ceiling(lector.TotalTime.TotalSeconds),numDuracion.Minimum,numDuracion.Maximum);
            lblArchivo.Text = Path.GetFileName(rutaSeleccionada);
        }
        catch (Exception ex) { MessageBox.Show(this, "No se pudo abrir el audio: " + ex.Message, "Archivo incompatible"); }
    }
    private void LimpiarCapturaAudio()
    {
        txtTitulo.Clear(); rutaSeleccionada = null; lblArchivo.Text = "Sin archivo seleccionado (solo metadatos)";
    }
    private void ImportarArchivos()
    {
        using var dialogo = DialogoAudio(true);
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        int agregados = 0; var errores = new List<string>();
        foreach (string ruta in dialogo.FileNames)
        {
            try
            {
                using var lector = new AudioFileReader(ruta);
                var pista = new Pista(contadorId++, Path.GetFileNameWithoutExtension(ruta), "Desconocido",
                    (int)numBpm.Value, (int)Math.Ceiling(lector.TotalTime.TotalSeconds)) { RutaArchivo = ruta };
                cola.Agregar(pista); agregados++;
            }
            catch (Exception ex) { errores.Add(Path.GetFileName(ruta) + ": " + ex.Message); }
        }
        RefrescarVista(); lblEstado.Text = $"Se agregaron {agregados} archivos al modo activo. BPM tomado del campo de captura; no se analiza automáticamente.";
        if (errores.Count > 0) MessageBox.Show(this, string.Join(Environment.NewLine, errores.Take(10)), "Archivos no importados");
    }
}
