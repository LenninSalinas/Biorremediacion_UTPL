# Errores identificados

> Simulador de Biorremediación — Versión WebGL
> Cada error incluye: pasos para reproducirlo, resultado esperado, resultado obtenido, gravedad y captura.

---

## 1. Se quitan los objetos agarrados al pulsar [ESC] para salir del formulario

**Scripts probablemente involucrados:** `FormularioUIToolkitController.cs`, `Grab.cs`

### Pasos para reproducirlo

1. Ponerse los guantes (ambos).
2. Clic izquierdo en el agarrador telescópico para equiparlo.
3. Clic izquierdo en el multiparámetro para equiparlo.
4. Clic derecho sobre una sonda para acoplarla al agarrador.
5. Ir a una zona de agua, en el punto correcto donde se activa el formulario.
6. Pulsar **[F]** para abrir el formulario.
7. Pulsar **[ESC]** para salir del formulario.

### Resultado esperado

Al cerrar el formulario con [ESC], el juego debe reanudarse (`Time.timeScale = 1`), el cursor debe bloquearse de nuevo, y los objetos que el jugador tenía equipados (multiparámetro en mano izquierda, agarrador con sonda en mano derecha) deben seguir exactamente donde estaban, sin verse afectados por cerrar el formulario.

### Resultado obtenido

Los objetos equipados (multiparámetro y/o agarrador con la sonda) desaparecen de las manos del jugador al salir del formulario con [ESC].

### Gravedad

**Alta** — el jugador pierde herramientas/objetos centrales para la mecánica de medición, obligando a volver a recogerlos (y si no reaparecen en el mundo, puede bloquear el progreso).

### Captura

![Evidencia](./images/Error1.png)

---

## 2. Se borran todos los valores de la pantalla de multiparámetro al sacar una sola sonda

**Scripts involucrados:** `WaterProbe.cs`, `MultiparameterScreen.cs`

### Pasos para reproducirlo

1. Agarrar dos o más sondas (de distinto tipo: pH, O2, Conductividad) y dejarlas dentro de la zona de agua para que las tres queden leyendo al mismo tiempo.
2. Sacar del agua una sola de las sondas que quedaron puestas.

### Resultado esperado

Al retirar del agua una sonda, solo debería borrarse (volver a `--`) el valor correspondiente a **esa** sonda. Las lecturas de las otras sondas que siguen en el agua deberían permanecer visibles en la pantalla.

### Resultado obtenido

Al sacar una sola sonda del agua, la pantalla borra **las tres** lecturas (pH, O2 y Conductividad), aunque las otras dos sondas sigan dentro del agua.

### Gravedad

**Alta** — se pierde información de medición válida; el usuario cree que las otras sondas dejaron de funcionar cuando en realidad siguen activas.

### Captura

![Evidencia](./images/Error2.png)

---

## 3. El valor de una sonda se queda congelado si se suelta el agarrador con la sonda dentro del agua

**Scripts involucrados:** `AgarradorTelescopico.cs`, `WaterProbe.cs`, `MultiparameterScreen.cs`

### Pasos para reproducirlo

1. Agarrar el agarrador telescópico.
2. Agarrar el multiparámetro.
3. Agarrar una sonda (cualquiera de las tres) con el agarrador.
4. Ir a una zona de agua.
5. Clic izquierdo para soltar el objeto del agarrador telescópico (quedando la sonda dentro del agua).
6. Volver a agarrar el objeto del agarrador telescópico.

### Resultado esperado

El valor mostrado en la pantalla multiparámetro debería seguir actualizándose (con su variación aleatoria normal) mientras la sonda permanezca dentro del agua, sin importar si el agarrador está equipado o no en la mano del jugador.

### Resultado obtenido

El número mostrado se queda fijo/pegado (deja de actualizarse) mientras el agarrador está soltado con la sonda dentro del agua, aunque la sonda siga físicamente en contacto con la zona de agua.

### Gravedad

**Media** — el dato mostrado deja de ser confiable (no refleja el estado real del agua), pero no bloquea el flujo del juego ni rompe otras mecánicas.

### Captura

![Evidencia](./images/Error3.png)

---

## 4. El agarrador telescópico atraviesa la zona de agua al soltarlo desde una altura media-alta

**Scripts involucrados:** `Grab.cs` (física al soltar: `SoltarDerecha()`), colisiones de `WaterZone`

### Pasos para reproducirlo

1. Agarrar el agarrador telescópico.
2. Ir a una zona de agua.
3. Alzar un poco la cámara del personaje hacia arriba (para soltar el objeto desde más alto sobre el agua).
4. Pulsar clic izquierdo para soltar el agarrador telescópico.

### Resultado esperado

Al soltarlo, el agarrador telescópico debería caer por gravedad y quedar flotando o detenido sobre/en la superficie del agua (o hundirse de forma controlada), sin atravesar el collider de la zona de agua.

### Resultado obtenido

El objeto atraviesa la geometría/collider de la zona de agua y cae a través de ella (posiblemente perdiéndose fuera del área jugable), en vez de quedar detenido en la superficie.

### Gravedad

**Alta** — el jugador puede perder permanentemente una herramienta necesaria para la mecánica principal (acoplar sondas), sin forma evidente de recuperarla.

### Captura

![Evidencia](./images/Error4_01.png)
![Evidencia](./images/Error4_02.png)

---

## 5. Conflicto de cursor al abrir el panel de información

**Scripts involucrados:** `Grab.cs`, `HUDController.cs`

### Pasos para reproducirlo

1. Mirar un objeto con información (componente `InformationObject`) y pulsar **[I]**.

### Resultado esperado

Al abrir el panel de información con [I], el cursor debería **liberarse y mostrarse** en pantalla (para poder leer el panel), y el juego debería pausarse (`Time.timeScale = 0`).

### Resultado obtenido

El tiempo de juego sí se detiene correctamente (`Time.timeScale = 0`), pero el cursor **no se oculta/libera**: permanece bloqueado y oculto como si el panel no estuviera abierto, impidiendo al jugador mover el mouse para interactuar con el panel de información.

### Gravedad

**Alta** — afecta directamente el control del jugador en una interacción central; el panel se abre pero el usuario no puede operarlo con el cursor.

### Captura

![Evidencia](./images/Error5.png)

---

## 6. Desincronización de interfaz y estado de juego al pulsar [I] dentro del formulario

**Scripts involucrados:** `FormularioUIToolkitController.cs`, `Grab.cs`, `HUDController.cs`

### Pasos para reproducirlo

1. Entrar a una zona de agua (aparece el aviso `[F] Formulario`).
2. Mirar un objeto interactivo en el agua y pulsar `[F]` para abrir el formulario.
3. Estando dentro del formulario, pulsar la tecla `[I]` dos veces consecutivas.
4. Mover el ratón / la cámara.

### Resultado esperado

Mientras el formulario está abierto, la tecla `[I]` no debería interferir con su estado ni con el del juego: el cursor debe mantenerse liberado sobre el formulario y el tiempo de juego debe seguir congelado hasta que el usuario cierre el formulario explícitamente (con `[ESC]` o `[F]`).

### Resultado obtenido

Al pulsar `[I]` dentro del formulario, el foco se le quita al formulario y se devuelve al juego (se reactiva el movimiento de cámara y el flujo normal), pero el formulario **sigue visible en pantalla** y queda en un estado inconsistente: no se puede interactuar con él ni cerrarlo con normalidad.

### Gravedad

**Crítica** — deja la interfaz en un estado roto que mezcla gameplay activo con una UI inoperable visible en pantalla; puede bloquear al jugador si no encuentra cómo salir de ese estado.

### Captura

![Evidencia](./images/Error6_01.png)
![Evidencia](./images/Error6_02.png)
![Evidencia](./images/Error6_03.png)
![Evidencia](./images/Error6_04.png)
![Evidencia](./images/Error6_05.png)
![Evidencia](./images/Error6_06.png)

---

## 7. Las sondas caen de forma anómala si se presiona clic derecho otra vez mientras se están usando

**Scripts involucrados:** `AgarradorTelescopico.cs` (`SoltarSonda()`)

### Pasos para reproducirlo

1. Equipar ambos guantes.
2. Clic izquierdo en el agarrador telescópico para equiparlo en la mano derecha. **[CONFIRMAR: el md original decía "clic derecho" en este paso — según el diseño del juego, equipar el agarrador se hace con clic izquierdo; corregido aquí asumiendo que fue un error de tipeo. Avísame si en realidad sí usaste clic derecho y lo dejamos así.]**
3. Con el agarrador equipado, apuntar a una sonda (cualquiera) y hacer clic derecho para acoplarla, luego sumergirla en el agua para tomar una medición.
4. Mientras la sonda sigue midiendo (dentro del agua), pulsar de nuevo clic derecho para soltarla.

### Resultado esperado

Al soltar la sonda con clic derecho, debería desacoplarse del agarrador con normalidad: reactivar su física/gravedad y caer de forma controlada hasta quedar apoyada en una superficie estable y accesible para volver a recogerla.

### Resultado obtenido

La sonda cae de forma anómala al soltarla en pleno muestreo y queda de manera **permanente inaccesible** (fuera del alcance del jugador, sin forma de volver a agarrarla ni con el agarrador telescópico ni con la mano).

### Gravedad

**Alta** — el jugador pierde de forma irrecuperable una herramienta de medición necesaria para completar las mediciones de esa sonda, sin ninguna vía dentro del juego para recuperarla.

### Captura

![Evidencia](./images/Error7.png)