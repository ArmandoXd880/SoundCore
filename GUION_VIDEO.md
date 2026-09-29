# Guion de demostración — máximo 3 minutos

Antes de grabar: abrir la solución, ejecutar SoundCore.UI en Release, tener el código de `Invertir()` a mano y la cantidad de benchmark en 25,000. Completar nombres y números de control. Ensayar para no exceder tres minutos.

## 0:00–1:15 — Operaciones

“Este es SoundCore, con audio real mediante NAudio instalado por NuGet, sin Windows Media Player. Está basado en la lista simple del profesor. Los nodos guardan una Pista y una referencia Siguiente. Podemos comparar nuestra lista con LinkedList y List de .NET.”

1. Selecciona Lista simple propia y añade la demo. Pulsa Play para que se escuchen los tonos. Muestra Pausa/Continuar brevemente.
2. Selecciona un MP3/WAV propio, completa sus datos y pulsa Encolar al final.
3. Captura Track VIP y pulsa Reproducir siguiente. Señala la fila 2.
4. Pulsa Avanzar y muestra la pista que salió del grid en Sonando.
5. Pulsa Invertir; señala que los identificadores de nodo se conservan.
6. Pulsa Ordenar por curva BPM y luego Purgar. La demo incluye dos registros de Tono A; después del ordenamiento estable conserva el primero.
7. Si hay tiempo, captura otra pista y usa Insertar por BPM para mostrar el sexto método directamente.
8. Cambia a los otros dos radios para mostrar que tienen sus colas y las mismas acciones.

## 1:15–2:00 — Benchmark

Ejecuta 25,000 inserciones. Lee los números que realmente muestre tu equipo.

“Las tres pruebas utilizan las mismas pistas. El cronómetro excluye la creación de metadatos y el dibujo de la tabla. En la lista propia solo reconectamos referencias; LinkedList usa AddAfter con un nodo conocido. Ambas inserciones son O(1). List debe desplazar referencias dentro del arreglo, por eso Insert(1) cuesta O(n), y repetirlo N veces acumula O(N²). Los bytes son asignaciones durante la prueba, no toda la memoria de la aplicación.”

No afirmes que los tiempos siempre serán iguales ni que el resultado ilustrativo de la diapositiva deba reproducirse.

## 2:00–2:55 — Explicar Invertir línea por línea

Abre `SoundCore.Core/EstructurasPropias/ListaSimpleEnlazada.cs`.

“La antigua cabeza será la cola. Previo empieza en null y actual en la cabeza. Siguiente guarda el resto de la cadena para no perderla. Después hago que actual.Siguiente apunte a previo: aquí invierto el enlace. Muevo previo a actual y actual a siguiente para continuar. Cuando actual llega a null, previo es la nueva cabeza. No creo nodos, no intercambio valores y no uso listas auxiliares. Recorro n nodos: O(n) en tiempo y O(1) en espacio auxiliar.”

## 2:55–3:00 — Cierre

“La solución separa la interfaz de la lógica e incluye pruebas para las operaciones y los enlaces.”
