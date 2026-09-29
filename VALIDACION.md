# Actualización de diseño 2.1 — 2026-09-29

Se modificó únicamente MainForm.Designer.cs en el código del programa. Se conservan los eventos de audio, las estructuras de datos y el benchmark. La distribución se corrigió a partir de la captura del usuario: la fila de 38 píxeles no alojaba los botones con sus márgenes; ahora las filas se dimensionan según el contenido y la barra tiene altura propia.

Comprobación actual: compilación de la solución en Release completada con 0 errores y 0 advertencias. No se ejecutó la GUI en Windows en este entorno Linux; queda pendiente comprobar la distribución a diferentes escalas DPI y tamaños de ventana. Las pruebas que siguen corresponden a la versión de lógica y audio anterior, que no se modificó.

---

# Validación — versión 2.0 con audio real

Fecha: 2026-09-28 UTC (2026-09-27 en México). Entorno: Linux x64, SDK .NET 10.0.401.

## Resultado

- Compilación de toda la solución en Release: 0 errores, 0 advertencias.
- 4,307 verificaciones de listas superadas y benchmark de 25,000 inserciones ejecutado.
- 17 comprobaciones de audio superadas: carga, pausa, continuación, búsqueda, volumen, cambios, fin natural, archivo ausente, error de dispositivo, liberación y espera de callback al detener.
- Los archivos WAV incluidos se abren y decodifican realmente; se comprobó duración y muestras no silenciosas.

**Limitación de las pruebas:** la salida de sonido se sustituye por un dispositivo simulado en las pruebas automatizadas. No se escuchó audio ni se abrió la ventana WinForms en este entorno Linux. Falta comprobar en Windows los altavoces, los MP3, la barra de progreso y la interacción visual, siguiendo el README. No se afirma que esas pruebas manuales se hayan ejecutado.

## Compilación

```text
  Determining projects to restore...
  All projects are up-to-date for restore.
  SoundCore.Core -> /workspace/scratch/436b693e36b6/audio-update/src/SoundCore/SoundCore.Core/bin/Release/net10.0/SoundCore.Core.dll
  SoundCore.UI -> /workspace/scratch/436b693e36b6/audio-update/src/SoundCore/SoundCore.UI/bin/Release/net10.0-windows/SoundCore.UI.dll
  SoundCore.Pruebas -> /workspace/scratch/436b693e36b6/audio-update/src/SoundCore/SoundCore.Pruebas/bin/Release/net10.0/SoundCore.Pruebas.dll
  SoundCore.PruebasAudio -> /workspace/scratch/436b693e36b6/audio-update/src/SoundCore/SoundCore.PruebasAudio/bin/Release/net10.0/SoundCore.PruebasAudio.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.50
```

## Pruebas de audio

```text
OK: 17 pruebas de audio (salida simulada, lectura WAV real). No verifica altavoces ni MP3 en Windows.
```

## Pruebas de listas y benchmark

```text
OK: 4307 verificaciones superadas.
ESTRÉS: 25,000 inserciones después de la cabeza
3 repeticiones; mediana de tiempo y bytes asignados. Datos precargados, sin GUI.
Bytes = asignaciones administradas del hilo, NO memoria total ni pico de RAM.
Lista propia        :      0.526 ms |      800,000 bytes asignados
.NET LinkedList<T>  :      0.693 ms |    1,200,000 bytes asignados
.NET List<T>        :     47.850 ms |      524,536 bytes asignados

Propia: O(1) por inserción; N inserciones = O(N). Reconecta Siguiente.
LinkedList: AddAfter con nodo conocido = O(1); total O(N). No incluye buscar el nodo.
List: Insert(1) desplaza referencias mediante copia de arreglo: O(n) por inserción.
Al crecer de 1 a N elementos, acumula O(N²) desplazamientos; además puede ampliar el búfer.
Los resultados dependen del equipo, GC y JIT; no se garantiza un ganador para todo tamaño.
La medición excluye la construcción de la cabeza, pero incluye nodos y crecimiento del arreglo.

```

Los tiempos son observaciones de esta ejecución. La prueba de estrés usa registros de canciones sin audio, inserta 25,000 después de una cabeza y termina con 25,001 elementos en cada prueba. No abre 25,000 MP3 simultáneamente.

## Pendientes del alumno

1. Probar sonido y controles en Windows con la demo y con sus propios MP3/WAV.
2. Completar nombres y números de control en las cabeceras.
3. Subir el proyecto a su repositorio y grabar la demostración.
