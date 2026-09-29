// Integrantes: [ESCRIBIR NOMBRES COMPLETOS]
// Números de control TecNM: [ESCRIBIR NÚMEROS]
// Fecha: 2026-09-20 | Versión: 1.0
namespace SoundCore.UI;

partial class MainForm
{
    private TextBox txtTitulo = null!, txtArtista = null!, txtResultadosBenchmark = null!;
    private NumericUpDown numBpm = null!, numDuracion = null!, numCantidad = null!;
    private RadioButton rbPropia = null!, rbLinkedList = null!, rbList = null!;
    private Button btnEncolarFinal = null!, btnReproducirSiguiente = null!, btnAvanzar = null!,
        btnInvertir = null!, btnOrdenarBpm = null!, btnInsertarOrdenado = null!, btnPurgar = null!,
        btnBenchmark = null!, btnDemo = null!, btnLimpiar = null!;
    private DataGridView dgvCola = null!;
    private Label lblNowPlaying = null!, lblEstadisticas = null!, lblEstado = null!;

    private void InitializeComponent()
    {
        SuspendLayout();
        Text = "SoundCore Engine — DJ Set Controller | TecNM Monclova";
        ClientSize = new Size(1200, 900);
        MinimumSize = new Size(960, 640);
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(19, 25, 39);
        ForeColor = Color.FromArgb(235, 240, 248);

        // El contenido conserva un tamaño legible; en pantallas pequeñas se desplaza.
        var vista = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var raiz = new TableLayoutPanel { Dock = DockStyle.Top, Height = 960,
            MinimumSize = new Size(1040, 960), Padding = new Padding(16), ColumnCount = 1, RowCount = 7 };
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 116));
        raiz.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        raiz.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 260));
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        var titulo = new Label { Text = "SOUNDCORE  /  DJ QUEUE LAB", Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(77, 209, 221), TextAlign = ContentAlignment.MiddleLeft };
        raiz.Controls.Add(titulo, 0, 0);

        var registro = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 8, RowCount = 2, Padding = new Padding(8) };
        for (int i = 0; i < 8; i++) registro.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, i % 2 == 0 ? 8 : 17));
        registro.RowStyles.Add(new RowStyle(SizeType.Absolute, 43));
        registro.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        txtTitulo = new TextBox { Name = "txtTitulo", Dock = DockStyle.Fill, MaxLength = 150 };
        txtArtista = new TextBox { Name = "txtArtista", Dock = DockStyle.Fill, MaxLength = 150 };
        numBpm = new NumericUpDown { Name = "numBpm", Minimum = 60, Maximum = 220, Value = 124, Dock = DockStyle.Fill };
        numDuracion = new NumericUpDown { Name = "numDuracion", Minimum = 1, Maximum = 86400, Value = 210, Dock = DockStyle.Fill };
        string[] etiquetas = ["Título", "Artista", "BPM", "Segundos"];
        Control[] campos = [txtTitulo, txtArtista, numBpm, numDuracion];
        for (int i = 0; i < 4; i++)
        {
            registro.Controls.Add(new Label { Text = etiquetas[i], Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, i * 2, 0);
            registro.Controls.Add(campos[i], i * 2 + 1, 0);
        }
        var modos = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        rbPropia = new RadioButton { Name = "rbPropia", Text = "Lista simple propia", Checked = true, AutoSize = true, Margin = new Padding(5,8,22,0) };
        rbLinkedList = new RadioButton { Name = "rbLinkedList", Text = ".NET LinkedList<T>", AutoSize = true, Margin = new Padding(5,8,22,0) };
        rbList = new RadioButton { Name = "rbList", Text = ".NET List<T>", AutoSize = true, Margin = new Padding(5,8,22,0) };
        modos.Controls.AddRange([rbPropia, rbLinkedList, rbList]);
        registro.Controls.Add(modos, 0, 1); registro.SetColumnSpan(modos, 8);
        raiz.Controls.Add(registro, 0, 1);

        var centro = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        centro.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 246));
        centro.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
        Button Boton(string nombre, string texto)
        {
            return new Button { Name = nombre, Text = texto, Width = 220, Height = 40, MinimumSize = new Size(220, 40),
                AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(6, 3, 6, 3),
                FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(35, 48, 69), ForeColor = ForeColor,
                Margin = new Padding(3, 3, 3, 5), Cursor = Cursors.Hand };
        }
        var archivos = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink, ColumnCount = 3, RowCount = 1,
            Margin = new Padding(0, 0, 0, 8) };
        archivos.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        archivos.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        archivos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        archivos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        btnArchivo = Boton("btnArchivo", "Seleccionar audio…");
        btnImportar = Boton("btnImportar", "Importar varios a la cola…");
        lblArchivo = new Label { AutoEllipsis = true, Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft, Text = "Sin archivo seleccionado (solo metadatos)",
            Margin = new Padding(12,3,0,5) };
        archivos.Controls.Add(btnArchivo,0,0); archivos.Controls.Add(btnImportar,1,0);
        archivos.Controls.Add(lblArchivo,2,0); raiz.Controls.Add(archivos,0,2);

        // Filas independientes con altura automática: la barra nunca recorta botones.
        var reproductor = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink, ColumnCount = 1, RowCount = 2,
            Padding = new Padding(10,8,10,8), Margin = new Padding(0,0,0,12),
            BackColor = Color.FromArgb(25,34,50) };
        reproductor.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        reproductor.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var transporte = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink, ColumnCount = 7, RowCount = 1,
            Margin = new Padding(0,0,0,8) };
        for(int i=0;i<7;i++) transporte.ColumnStyles.Add(new ColumnStyle(i==4 ? SizeType.Percent : SizeType.AutoSize, i==4 ? 100 : 0));
        transporte.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        btnPlay = Boton("btnPlay", "Play / Continuar"); btnPlay.MinimumSize = new Size(150,40);
        btnPausa = Boton("btnPausa", "Pausa"); btnPausa.MinimumSize = new Size(100,40);
        btnStop = Boton("btnStop", "Detener"); btnStop.MinimumSize = new Size(100,40);
        btnPlay.BackColor = Color.FromArgb(22,100,113);
        lblTiempo = new Label { Text = "00:00:00 / 00:00:00", AutoSize = true,
            Anchor = AnchorStyles.Left, Margin = new Padding(16,0,12,0) };
        barraVolumen = new TrackBar { Minimum = 0, Maximum = 100, Value = 70,
            AutoSize = false, Size = new Size(140,36), Anchor = AnchorStyles.Right,
            TickStyle = TickStyle.None, Margin = new Padding(4,3,0,5), AccessibleName = "Volumen" };
        transporte.Controls.Add(btnPlay,0,0); transporte.Controls.Add(btnPausa,1,0);
        transporte.Controls.Add(btnStop,2,0); transporte.Controls.Add(lblTiempo,3,0);
        transporte.Controls.Add(new Label { Text = "Volumen", AutoSize = true, Anchor = AnchorStyles.Right,
            Margin = new Padding(8,0,4,0) },5,0);
        transporte.Controls.Add(barraVolumen,6,0);
        barraPosicion = new TrackBar { Dock = DockStyle.Top, AutoSize = false, Height = 36,
            Minimum = 0, Maximum = 1000, TickStyle = TickStyle.None,
            Margin = new Padding(0,4,0,0), AccessibleName = "Posición de reproducción" };
        reproductor.Controls.Add(transporte,0,0); reproductor.Controls.Add(barraPosicion,0,1);
        raiz.Controls.Add(reproductor,0,3);
        btnEncolarFinal = Boton("btnEncolarFinal", "+ Encolar al final");
        btnReproducirSiguiente = Boton("btnReproducirSiguiente", "Reproducir siguiente (fila 2)");
        btnAvanzar = Boton("btnAvanzar", "Avanzar pista");
        btnInvertir = Boton("btnInvertir", "Invertir lista in-place");
        btnOrdenarBpm = Boton("btnOrdenarBpm", "Ordenar por curva BPM");
        btnInsertarOrdenado = Boton("btnInsertarOrdenado", "Insertar por BPM");
        btnPurgar = Boton("btnPurgar", "Purgar títulos duplicados");
        btnDemo = Boton("btnDemo", "Añadir demo a los 3 modos");
        btnLimpiar = Boton("btnLimpiar", "Vaciar modo activo");
        acciones.Controls.AddRange([btnEncolarFinal,btnReproducirSiguiente,btnAvanzar,btnInvertir,btnOrdenarBpm,btnInsertarOrdenado,btnPurgar,btnDemo,btnLimpiar]);
        centro.Controls.Add(acciones, 0, 0);
        var playlist = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        playlist.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        playlist.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        playlist.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        lblNowPlaying = new Label { Name = "lblNowPlaying", Dock = DockStyle.Fill, AutoEllipsis = true,
            ForeColor = Color.FromArgb(94,234,172), Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
        dgvCola = new DataGridView { Name = "dgvCola", Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            AllowUserToDeleteRows = false, RowHeadersVisible = false, AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false, BackgroundColor = Color.FromArgb(25,34,50), BorderStyle = BorderStyle.None,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize };
        dgvCola.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(43,57,77);
        dgvCola.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvCola.DefaultCellStyle.BackColor = Color.FromArgb(25,34,50);
        dgvCola.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
        dgvCola.DefaultCellStyle.SelectionBackColor = Color.FromArgb(34,100,115);
        dgvCola.DefaultCellStyle.SelectionForeColor = Color.White;
        dgvCola.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(30,40,57);
        string[] columnas = ["Pos.", "ID", "Título", "Artista", "BPM", "Duración", "Nodo / enlace lógico"];
        float[] anchos = [40,40,135,110,45,75,185];
        for (int i = 0; i < columnas.Length; i++) dgvCola.Columns.Add(new DataGridViewTextBoxColumn {
            HeaderText = columnas[i], FillWeight = anchos[i], SortMode = DataGridViewColumnSortMode.NotSortable });
        // Deshabilitar el orden visual evita ocultar el orden real de los nodos.
        lblEstadisticas = new Label { Name = "lblEstadisticas", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        playlist.Controls.Add(lblNowPlaying,0,0); playlist.Controls.Add(dgvCola,0,1); playlist.Controls.Add(lblEstadisticas,0,2);
        centro.Controls.Add(playlist,1,0); raiz.Controls.Add(centro,0,4);

        var benchmark = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Padding = new Padding(0,8,0,0) };
        benchmark.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        benchmark.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = false, Margin = new Padding(0,0,0,6) };
        barra.Controls.Add(new Label { Text = "BENCHMARK  /  Inserciones", AutoSize = true, Margin = new Padding(3,10,15,0) });
        numCantidad = new NumericUpDown { Name = "numCantidad", Minimum = 1000, Maximum = 100000, Increment = 5000, Value = 25000, ThousandsSeparator = true, Width = 115, Margin = new Padding(3,6,15,0) };
        btnBenchmark = Boton("btnBenchmark", "Iniciar prueba de rendimiento");
        barra.Controls.Add(numCantidad); barra.Controls.Add(btnBenchmark);
        txtResultadosBenchmark = new TextBox { Name = "txtResultadosBenchmark", Dock = DockStyle.Fill,
            Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, WordWrap = false,
            BackColor = Color.FromArgb(12,18,29), ForeColor = Color.FromArgb(159,227,224),
            Font = new Font("Consolas", 10), Text = "Pulsa Iniciar para medir tiempos y asignaciones de memoria reales." };
        benchmark.Controls.Add(barra,0,0); benchmark.Controls.Add(txtResultadosBenchmark,0,1); raiz.Controls.Add(benchmark,0,5);
        lblEstado = new Label { Name = "lblEstado", Dock = DockStyle.Fill, AutoEllipsis = true, Font = new Font("Segoe UI", 9), TextAlign = ContentAlignment.MiddleLeft };
        raiz.Controls.Add(lblEstado,0,6);
        vista.Controls.Add(raiz); Controls.Add(vista);
        vista.Resize += (_, _) => raiz.Height = Math.Max(raiz.MinimumSize.Height, vista.ClientSize.Height);
        Shown += (_, _) =>
        {
            // Ajustar a la zona útil real, incluida la barra de tareas y el escalado DPI.
            var zona = Screen.FromControl(this).WorkingArea;
            int ancho = Math.Max(320,zona.Width-32), alto = Math.Max(240,zona.Height-32);
            MinimumSize = new Size(Math.Min(MinimumSize.Width,ancho),Math.Min(MinimumSize.Height,alto));
            Size = new Size(Math.Min(Width,ancho),Math.Min(Height,alto));
            Location = new Point(zona.Left+(zona.Width-Width)/2,zona.Top+(zona.Height-Height)/2);
            raiz.Height = Math.Max(raiz.MinimumSize.Height, vista.ClientSize.Height);
        };
        ResumeLayout(true);
    }
}
