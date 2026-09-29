// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-20 | Versión: 1.0
using System.Runtime.CompilerServices;
using SoundCore.UI.Audio;
using NAudio.Wave;
using SoundCore.Modelos;
using SoundCore.Servicios;

namespace SoundCore.UI;

public partial class MainForm : Form
{
    private readonly ColaDj cola = new();
    private int contadorId = 1;
    // IDs visuales estables de objetos, no direcciones de memoria (el GC puede moverlos).
    private sealed record Identidad(int Numero);
    private readonly ConditionalWeakTable<object, Identidad> identidades = new();
    private int contadorNodo;
    private string IdNodo(object? nodo) => nodo == null ? "null" :
        $"N{identidades.GetValue(nodo, _ => new Identidad(++contadorNodo)).Numero:D4}";

    public MainForm()
    {
        InitializeComponent();
        ConfigurarAudio();
        rbPropia.CheckedChanged += CambiarModo;
        rbLinkedList.CheckedChanged += CambiarModo;
        rbList.CheckedChanged += CambiarModo;
        btnEncolarFinal.Click += (_, _) => Capturar(false);
        btnReproducirSiguiente.Click += (_, _) => Capturar(true);
        btnAvanzar.Click += async (_, _) => await AccionAudioAsync(() => AvanzarAudioAsync(false));
        btnInvertir.Click += (_, _) => { cola.Invertir(); RefrescarEstado("Enlaces invertidos."); };
        btnOrdenarBpm.Click += (_, _) => { cola.Ordenar(); RefrescarEstado("Cola ordenada por BPM ascendente."); };
        btnInsertarOrdenado.Click += (_, _) => CapturarOrdenado();
        btnPurgar.Click += (_, _) => { cola.Purgar(); RefrescarEstado("Se conservó la primera aparición de cada título."); };
        btnLimpiar.Click += async (_, _) => await AccionAudioAsync(async () =>
        { await audio.DetenerAsync(); cola.Limpiar(); pistaAudible = null; estadoAudio = "Detenido"; RefrescarVista(); });
        btnDemo.Click += (_, _) => CargarDemo();
        btnBenchmark.Click += async (_, _) => await EjecutarBenchmark();
        RefrescarVista();
    }

    private async void CambiarModo(object? sender, EventArgs e)
    {
        if (sender is RadioButton rb && rb.Checked)
        {
            await AccionAudioAsync(async () =>
            {
                await audio.DetenerAsync(); pistaAudible = null; estadoAudio = "Detenido";
                cola.Modo = rbPropia.Checked ? TipoEstructura.Propia :
                    rbLinkedList.Checked ? TipoEstructura.LinkedList : TipoEstructura.List;
                RefrescarVista();
            });
        }
    }
    private Pista? LeerPista()
    {
        if (string.IsNullOrWhiteSpace(txtTitulo.Text) || string.IsNullOrWhiteSpace(txtArtista.Text))
        {
            MessageBox.Show(this, "Escribe el título y el artista.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (string.IsNullOrWhiteSpace(txtTitulo.Text)) txtTitulo.Focus(); else txtArtista.Focus();
            return null;
        }
        return new Pista(contadorId++, txtTitulo.Text.Trim(), txtArtista.Text.Trim(), (int)numBpm.Value, (int)numDuracion.Value)
        { RutaArchivo = rutaSeleccionada };
    }
    private void Capturar(bool siguiente)
    {
        var pista = LeerPista();
        if (pista == null) return;
        cola.Agregar(pista, siguiente);
        RefrescarVista(); LimpiarCapturaAudio(); txtTitulo.Focus();
    }
    private void CapturarOrdenado()
    {
        var pista = LeerPista();
        if (pista == null) return;
        // Garantizar la precondición del método InsertarOrdenado.
        cola.Ordenar();
        if (cola.Modo == TipoEstructura.Propia)
            cola.Propia.InsertarOrdenado(pista, (a,b) => a.Bpm.CompareTo(b.Bpm));
        else if (cola.Modo == TipoEstructura.LinkedList)
        {
            var nodo = cola.Enlazada.First;
            while (nodo != null && nodo.Value.Bpm <= pista.Bpm) nodo = nodo.Next;
            if (nodo == null) cola.Enlazada.AddLast(pista); else cola.Enlazada.AddBefore(nodo, pista);
        }
        else
        {
            int indice = 0;
            while (indice < cola.Arreglo.Count && cola.Arreglo[indice].Bpm <= pista.Bpm) indice++;
            cola.Arreglo.Insert(indice, pista);
        }
        RefrescarVista(); LimpiarCapturaAudio(); txtTitulo.Focus();
    }
    private void RefrescarEstado(string mensaje) { RefrescarVista(); lblEstado.Text = mensaje; }
    private static string Duracion(long segundos) => $"{segundos / 3600:D2}:{segundos / 60 % 60:D2}:{segundos % 60:D2}";

    private void RefrescarVista()
    {
        dgvCola.Rows.Clear();
        int posicion = 0;
        long total = 0;
        void Fila(Pista p, string referencia)
        {
            dgvCola.Rows.Add(++posicion, p.Id, p.Titulo, p.Artista, p.Bpm, Duracion(p.DuracionSegundos), referencia);
            total += p.DuracionSegundos;
        }
        if (cola.Modo == TipoEstructura.Propia)
            for (var nodo = cola.Propia.Cabeza; nodo != null; nodo = nodo.Siguiente)
                Fila(nodo.Valor, $"{IdNodo(nodo)} → {IdNodo(nodo.Siguiente)}");
        else if (cola.Modo == TipoEstructura.LinkedList)
            for (var nodo = cola.Enlazada.First; nodo != null; nodo = nodo.Next)
                Fila(nodo.Value, $"{IdNodo(nodo.Previous)} ← {IdNodo(nodo)} → {IdNodo(nodo.Next)}");
        else
            foreach (var pista in cola.Elementos) Fila(pista, $"índice [{posicion}]");
        ActualizarReproductor();
        lblEstadisticas.Text = $"Total en cola: {posicion} pistas  |  Duración pendiente: {Duracion(total)}";
        lblEstado.Text = "Cada modo conserva su cola. N0001 identifica un objeto, no su dirección física. Audio real con NAudio. Los datos sin archivo sirven solo para practicar listas.";
    }
    private void CargarDemo()
    {
        // Añade una muestra idéntica a las tres colas; conserva sus datos anteriores.
        var anterior = cola.Modo;
        string Demo(string archivo) => Path.Combine(AppContext.BaseDirectory, "AudioDemo", archivo);
        Pista[] pistas = [new(contadorId++, "Tono A", "SoundCore demo", 128, 3) { RutaArchivo = Demo("tono-a.wav") },
            new(contadorId++, "Tono B", "SoundCore demo", 115, 3) { RutaArchivo = Demo("tono-b.wav") },
            new(contadorId++, "Tono C", "SoundCore demo", 140, 3) { RutaArchivo = Demo("tono-c.wav") },
            new(contadorId++, "Tono A", "SoundCore demo", 128, 3) { RutaArchivo = Demo("tono-a.wav") }];
        foreach (TipoEstructura modo in Enum.GetValues<TipoEstructura>())
        {
            cola.Modo = modo;
            foreach (var pista in pistas) cola.Agregar(pista);
        }
        cola.Modo = anterior;
        RefrescarVista();
    }
    private async Task EjecutarBenchmark()
    {
        btnBenchmark.Enabled = false; numCantidad.Enabled = false;
        txtResultadosBenchmark.Text = "Midiendo las tres estructuras… Puedes seguir usando la cola.";
        int n = (int)numCantidad.Value;
        try
        {
            string resultado = await Task.Run(() => BenchmarkServicio.Ejecutar(n));
            if (!IsDisposed) txtResultadosBenchmark.Text = resultado;
        }
        catch (Exception ex)
        {
            if (!IsDisposed) txtResultadosBenchmark.Text = "No fue posible completar la medición: " + ex.Message;
        }
        finally
        {
            if (!IsDisposed) { btnBenchmark.Enabled = true; numCantidad.Enabled = true; }
        }
    }
}
