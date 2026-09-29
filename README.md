# SoundCore Engine — DJ Queue & Transition Manager

Proyecto de Estructura de Datos, Unidad 2. TecNM Campus Monclova.
C# 14, .NET 10 y Windows Forms. Audio real mediante el paquete NuGet **NAudio 2.2.1**. No usa Windows Media Player, COM ni ActiveX.

## Abrir y ejecutar

1. Extrae **todo** el ZIP en una carpeta de tu PC con Windows.
2. Necesitas el SDK de .NET 10 y Visual Studio compatible con .NET 10, con la carga de trabajo **Desarrollo de escritorio de .NET** instalada.
3. Abre `SoundCore.sln`. En el Explorador de soluciones, clic derecho en **SoundCore.UI → Establecer como proyecto de inicio**.
4. Selecciona **Release** y pulsa **Ctrl+F5**.
5. Pulsa **Añadir demo a los 3 modos** y después **Play / Continuar**: oirás tonos breves de prueba. Para música real, usa **Importar varios a la cola…** y selecciona tus MP3/WAV.

También puedes usar una terminal en la carpeta de la solución:

```powershell
dotnet build SoundCore.sln -c Release
dotnet run --project SoundCore.UI -c Release
```

Visual Studio restaura NAudio desde NuGet al compilar (necesitas conexión en la primera restauración). No tienes que arrastrar un control multimedia al formulario ni instalar Windows Media Player. Si faltan paquetes, ejecuta `dotnet restore SoundCore.sln`. Se fija la versión 2.2.1 para mantener reproducible esta entrega.

Las pruebas de lógica se ejecutan sin Windows Forms y también funcionan en Linux:

```powershell
dotnet run --project SoundCore.Pruebas -c Release
dotnet run --project SoundCore.PruebasAudio -c Release
```

La aplicación WinForms requiere Windows; no se ejecuta directamente en Linux Mint.
El proyecto de pruebas es una consola de comprobaciones sin dependencias de xUnit/NUnit.

## Reproducción real

1. **Importar varios a la cola…** agrega tus archivos MP3 o WAV directamente al final de la estructura activa.
2. **Seleccionar audio…** prepara un solo archivo en la captura. Puedes editar título, artista y BPM, y luego elegir Encolar, Reproducir siguiente o Insertar por BPM.
3. **Play / Continuar** comienza con la cabeza cuando aún no hay una pista actual; reanuda la pausa, o reinicia la pista actual después de Detener/finalizar.
4. **Pausa** conserva la posición. **Detener** libera el archivo; Play lo reinicia desde el inicio.
5. **Avanzar pista** reproduce y retira la primera pista pendiente. Al terminar un archivo se avanza automáticamente por la cola activa.
6. Ajusta **Volumen** y arrastra la barra de posición para buscar dentro del audio.

El botón **Reproducir siguiente (fila 2)** conserva exactamente la operación académica: inserta detrás de la cabeza de la cola pendiente. Si hay una pista sonando fuera de la cola, esto NO coloca la nueva pista delante de la cabeza pendiente.

Las colas de los tres modos son independientes. Cambiar de modo detiene el audio; al volver, Play reinicia la última pista extraída de ese modo (no conserva su posición de reproducción). Invertir, ordenar y purgar afectan las pistas pendientes, no el archivo que está sonando.

Se detecta la duración del archivo. El título inicial es el nombre del archivo y el artista es editable; no se extraen etiquetas ID3. **El BPM se captura manualmente** y, en importación múltiple, se toma del campo BPM; no se calcula a partir del sonido.

Las rutas son locales: no borres ni muevas los archivos después de agregarlos. Un archivo inexistente o incompatible muestra un aviso y permanece en la cola si no pudo comenzar. Puedes vaciar la cola e importarlo nuevamente desde su ubicación válida. La aplicación no guarda playlists entre ejecuciones.

Los datos capturados sin archivo siguen disponibles para practicar estructuras: se identifican como “Sin archivo” al avanzar y no generan audio. Si el avance automático encuentra uno, se detiene en ese registro. El benchmark utiliza metadatos sin audio de forma intencional.

## Cómo usar las listas

- **Encolar al final:** captura título, artista, BPM (60–220) y duración (1–86,400 segundos).
- **Reproducir siguiente:** inserta los datos capturados en la fila 2, detrás de la cabeza. En una cola vacía crea la cabeza.
- **Avanzar pista:** extrae la primera fila y la muestra en “Sonando”, tal como indica CP-03.
- **Invertir:** cambia el orden de la cola pendiente; en la lista propia invierte los enlaces sin crear nodos ni intercambiar valores.
- **Ordenar por curva BPM:** ordena las pistas ya presentes de menor a mayor BPM.
- **Insertar por BPM:** ordena primero la cola y después inserta la pista capturada en su posición. Permite demostrar directamente `InsertarOrdenado`.
- **Purgar:** conserva el primer título repetido, ignorando mayúsculas y espacios en los extremos.
- **Vaciar modo activo:** limpia únicamente la estructura seleccionada y su indicador de reproducción.
- **Añadir demo:** agrega cuatro registros con tonos de prueba a las tres colas; incluye un título duplicado. No sustituye lo que ya ingresaste.
- **Benchmark:** selecciona la cantidad (25,000 por defecto) y pulsa Iniciar. Corre en un hilo de trabajo sin modificar las colas de la GUI.

Cada modo conserva su propia cola y pista sonando. La tabla se refresca al cambiar de radio o ejecutar una acción; no se ordenan filas haciendo clic en encabezados, pues eso ocultaría el orden real de la estructura.

La reproducción usa `AudioFileReader` y `WaveOutEvent` de NAudio. Solo se abre el archivo actual: no se decodifican todos los archivos de la cola a la vez. Los identificadores `N0001` corresponden a la identidad lógica de un nodo y se conservan al invertir; no son direcciones físicas, que pueden cambiar con el recolector de basura. `List<T>` muestra índices porque no tiene nodos.

## Relación con el programa del profesor

Base consultada: https://github.com/rriojas/edSimpleLinkedList
Commit revisado: `6d4696fe0ffc9bc8112779b9a503ea663c601efd`.

| Ejemplo del profesor | Adaptación del proyecto |
|---|---|
| `Node`, datos de estudiante | `Nodo<T>` y registro separado `Pista` |
| `Head` | `Cabeza` |
| `Next` | `Siguiente` |
| `current` y recorrido con `while` | `actual` y recorrido con `while` |
| `Add` ordenado por ID | `InsertarOrdenado` con comparador, usado para BPM |
| `current.Next = current.Next.Next` | Reconexión al purgar duplicados |
| `Count()` recorre la lista | `Conteo` se mantiene en cada inserción/eliminación |
| Programa de consola | Presentación Windows Forms separada de la lógica |

Es una adaptación y ampliación académica: el original ordenaba por ID y rechazaba IDs repetidos. Aquí se respetan el orden de la cola y los duplicados de título hasta que el usuario los purga. No se copian los métodos de búsqueda con corte por ID porque ya no sería válido asumir ese orden.

## Organización

- `SoundCore.Core/Modelos/Pista.cs`: record de los metadatos.
- `SoundCore.Core/EstructurasPropias/Nodo.cs`: dato y referencia al siguiente nodo.
- `SoundCore.Core/EstructurasPropias/ListaSimpleEnlazada.cs`: seis algoritmos, ordenamiento, limpieza e `IEnumerable<T>` con `yield return`.
- `SoundCore.Core/Servicios/ColaDj.cs`: operaciones equivalentes para los tres modos, sin referencias a WinForms.
- `SoundCore.Core/Servicios/BenchmarkServicio.cs`: medición real con `Stopwatch` y contadores de asignación.
- `SoundCore.UI/UI/MainForm.cs`: eventos, validaciones y refresco de la tabla.
- `SoundCore.UI/Audio/ReproductorAudio.cs`: apertura, salida, pausa, búsqueda y liberación de audio.
- `SoundCore.UI/UI/MainForm.Audio.cs`: conexión entre la cola y los botones de audio, importación y avance automático.
- `SoundCore.UI/AudioDemo/`: tres WAV originales de tonos sintetizados, no canciones comerciales.
- `SoundCore.PruebasAudio/`: lectura WAV real y pruebas del reproductor con salida simulada.
- `SoundCore.UI/UI/MainForm.Designer.cs`: construcción de controles y distribución adaptable. El layout está escrito en C#; no necesita recursos externos.
- `SoundCore.Pruebas/Program.cs`: pruebas de comportamiento, identidad de nodos y operaciones mixtas.

## Complejidad de la estructura propia

| Método | Tiempo | Espacio auxiliar | Explicación |
|---|---|---|---|
| `AgregarAlFinal` | O(1) | O(1) | Conserva referencia a la cola; asigna un nodo nuevo |
| `ReproducirSiguiente` | O(1) | O(1) | Inserta detrás de la cabeza |
| `AvanzarPista` | O(1) | O(1) | Desconecta la cabeza |
| `Invertir` | O(n) | O(1) | Reorienta los mismos nodos |
| `InsertarOrdenado` | O(n) | O(1) | Busca posición en una lista previamente ordenada; asigna un nodo |
| `DepurarDuplicados` | O(n²) | O(1) | Dos recorridos, sin HashSet ni arreglos |
| `Ordenar` | O(n²) | O(1) | Ordenamiento por inserción estable reutilizando nodos |

La propia lista no usa arreglos, listas temporales ni colecciones externas. Los arreglos del benchmark y de las pruebas están fuera de esa implementación. Los comparadores deben definir un orden consistente y no modificar la lista.

En .NET `LinkedList<T>`, AddAfter con el nodo conocido y RemoveFirst son O(1). En `List<T>`, Insert(1) y RemoveAt(0) son O(n). El refresco de la tabla cuesta O(n), independientemente de que una operación sobre nodos sea O(1); el benchmark excluye ese refresco.

## Metodología del benchmark

1. Crea una sola colección de pistas reproducible con semilla 42, fuera del tramo medido.
2. Calienta las tres implementaciones.
3. Ejecuta tres rondas y rota el orden de las estructuras.
4. Cada prueba parte de una cabeza e inserta N referencias a las mismas pistas detrás de ella.
5. Muestra la mediana de milisegundos y de bytes asignados en el hilo.

Los bytes incluyen nodos nuevos o ampliaciones del arreglo durante las inserciones; excluyen los metadatos precargados y la construcción de la cabeza. Son **asignaciones acumuladas**, no memoria retenida ni pico de RAM. No se fuerza GC. El recolector, JIT, carga del equipo y tamaño de entrada influyen: una sola medición no demuestra por sí sola una ley asintótica.

En `List<T>`, insertar en la posición 1 desplaza referencias en el arreglo. Al repetir la operación mientras crece, la suma de desplazamientos es del orden de N². No significa que se copien todos los objetos Pista ni que cualquier aplicación de audio necesariamente se congele.

## Casos de evaluación manual

Ejecuta cada caso en los tres modos, limpiando el modo cuando corresponda.

| ID | Pasos | Resultado |
|---|---|---|
| CP-01 | Capturar Track 1 / DJ / 120 / 180; Encolar | Aparece al final |
| CP-02 | Tener al menos dos pistas; capturar Track VIP; Reproducir siguiente | Track VIP en fila 2 |
| CP-03 | Avanzar | Desaparece la primera fila y su pista aparece en Sonando |
| CP-04 | Ingresar BPM 100, 110, 120; Invertir | 120, 110, 100; mismos IDs de nodo con enlaces invertidos |
| CP-05 | Ingresar BPM 128, 115, 140, 120; Ordenar | 115, 120, 128, 140 |
| CP-06 | Ingresar Strobe, Otra, strobe; Purgar | Conserva el primer Strobe y Otra |
| CP-07 | Vaciar; Avanzar | MessageBox informativo y aplicación abierta |

Comprobar además: título/artista vacío; Up Next con cola vacía; un solo nodo; inversión doble; agregar después de purgar el último nodo; duración total mayor a una hora; cambio de radios conservando datos; benchmark de 25,000 manteniendo la ventana utilizable.

## Antes de entregar

- Completa los marcadores de nombres completos y números de control en **todos los archivos .cs**. No se inventaron datos personales.
- Revisa `VALIDACION.md` para saber qué se ejecutó y qué falta probar visualmente en Windows.
- Graba el video de máximo tres minutos usando `GUION_VIDEO.md`.
- Sube esta carpeta completa a tu propio repositorio Git. No incluyas `bin`, `obj` ni `.vs`.
- La creación de este ZIP no publica un repositorio ni graba el video.

## Comprobación de audio en Windows

- Añadir demo y Play: escuchar tonos A, B y C con avance automático.
- Importar dos MP3 propios: comprobar que ambos suenan en orden.
- Pausar y continuar: conservar la posición; Detener y Play: volver al comienzo.
- Cambiar volumen y buscar dentro de una canción.
- Avanzar varias veces: no superponer dos canciones.
- Cambiar de modo durante reproducción y cerrar mientras suena: detener sonido y liberar archivo.
- Encolar un archivo y moverlo antes de Play: mostrar aviso y conservar la pista pendiente.
- Ejecutar benchmark de 25,000 durante la reproducción: continuar utilizando la ventana.

## Referencias del paquete

- NAudio 2.2.1: https://www.nuget.org/packages/NAudio/2.2.1
- Proyecto: https://github.com/naudio/NAudio
- Reproducción y liberación de recursos: https://github.com/naudio/NAudio/blob/main/Docs/PlaybackStopped.md

NAudio es una biblioteca de audio, no un reproductor visual prefabricado. En Windows usa APIs de sonido del sistema; eso es distinto de insertar el control Windows Media Player. La licencia y dependencias transitivas del paquete se obtienen mediante NuGet.

## Ajuste de diseño 2.1

- Botones de transporte en una fila de altura automática; barra de posición en otra fila con margen propio.
- Panel de reproducción diferenciado, botón Play destacado y volumen alineado a la derecha.
- Selector de archivo con texto recortado mediante puntos suspensivos para no desplazar otros controles.
- Ventana ajustada al área útil de la pantalla; desplazamiento cuando el contenido no cabe.
- Cabecera del benchmark con altura automática y botones con espacio para texto y márgenes.

Extraer esta actualización en una carpeta nueva y abrir su SoundCore.sln. Para verificar el cambio en Windows, probar a 100%, 125% y 150% de escala, reducir/maximizar la ventana y comprobar que Play, Pausa, Detener y ambas barras están accesibles. La tabla sigue siendo de solo lectura.
