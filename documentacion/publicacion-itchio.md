# Publicación del simulador en itch.io

El simulador se publica como juego HTML5 (WebGL) en una página de itch.io de la cuenta institucional UTPL. La web de inicio de sesión (`web/`) redirige a esa página cuando el usuario entra correctamente.

## 1. Requisitos (una sola vez)

1. **Módulo WebGL de Unity.** En *Unity Hub → Installs → 6000.0.38f1 → ⚙ → Add modules* marcar **WebGL Build Support** e instalar (unos 860 MB de descarga y 4 GB en disco).
2. **Cuenta de itch.io institucional.** Crearla en <https://itch.io/register> con el correo institucional UTPL. El nombre de usuario define la URL pública (`<usuario>.itch.io/biorremediacion`), así que conviene uno estable, por ejemplo `utpl-biorremediacion`.
3. En *Settings → Profile* de itch.io, completar el perfil (nombre visible: *UTPL*) para que el proyecto aparezca como institucional.

## 2. Configuración de la build web

Ya está guardada en `ProjectSettings` y no hay que tocarla:

| Ajuste (*Player Settings → Web → Publishing Settings*) | Valor | Motivo |
|---|---|---|
| Compression Format | **Gzip** | itch.io no envía las cabeceras `Content-Encoding` que necesita Brotli. |
| Decompression Fallback | **Activado** | El navegador descomprime los archivos aunque el servidor no envíe cabeceras. |
| Resolución por defecto | 960 × 600 | Debe coincidir con el tamaño del visor en itch.io. |

## 3. Generar la build

1. Abrir el proyecto `simulador/Biorremediacion_UTPL` en Unity 6000.0.38f1.
2. *File → Build Profiles → Web → Switch Platform* (la primera vez tarda porque reimporta los assets).
3. Comprobar que las escenas incluidas son, en este orden: `MenuPrincipal`, `SampleScene`, `DemoScene`.
4. *Build* → elegir la carpeta `simulador/Biorremediacion_UTPL/Builds/WebGL` (está en `.gitignore`, no se sube al repositorio).

El resultado debe tener `index.html`, `Build/` y `TemplateData/`.

## 4. Empaquetar en .zip

itch.io exige que `index.html` esté en la **raíz** del zip, no dentro de una carpeta. Desde la carpeta del proyecto de Unity:

```bash
cd Builds/WebGL && tar -a -c -f ../biorremediacion-webgl.zip *
```

> No usar *Enviar a → Carpeta comprimida* ni `Compress-Archive` de PowerShell 5.1: guardan las rutas con `\` y itch.io puede no encontrar los archivos.

Alternativa sin zip: [butler](https://itch.io/docs/butler/), la herramienta oficial de itch.io, sube la carpeta directamente y solo envía lo que cambió:

```bash
butler login
butler push Builds/WebGL <usuario>/biorremediacion:html5 --userversion 0.1.0
```

## 5. Crear la página en itch.io

*Dashboard → Create new project*:

| Campo | Valor |
|---|---|
| Title | Biorremediación UTPL |
| Project URL | `biorremediacion` |
| Short description | Simulador 3D de muestreo de agua para prácticas de biorremediación. |
| Classification | Games → *Kind of project:* **HTML** |
| Release status | Released |
| Pricing | No payments |
| Uploads | Subir `biorremediacion-webgl.zip` y marcar **This file will be played in the browser** |
| Embed options | Viewport **960 × 600**, activar **Fullscreen button**; *Mobile friendly* desactivado |
| Genre | Simulation / Educational |
| Tags | `simulation`, `educational`, `3d`, `first-person`, `science` |
| Language | Spanish |
| Visibility | Ver la nota de abajo |

### Descripción sugerida

```text
Biorremediación UTPL es un simulador 3D en primera persona desarrollado en la
Universidad Técnica Particular de Loja para practicar el muestreo de agua en
proyectos de biorremediación.

El estudiante recorre los puntos de muestreo, toma muestras con el frasco y el
agarrador telescópico, mide parámetros con el equipo multiparámetro y registra
los datos en el formulario de campo.

CONTROLES
- W A S D / flechas: moverse
- Ratón: mirar alrededor
- Espacio: saltar
- Clic izquierdo: tomar un objeto
- Clic derecho: usar el agarrador telescópico
- I: ver información del objeto
- F: abrir el formulario (dentro de la zona de muestreo)
- Esc: cerrar ventanas

Se recomienda un navegador de escritorio (Chrome, Edge o Firefox).
```

Agregar al menos 3 capturas de pantalla (630 × 500 o mayores) y una imagen de portada de 630 × 500.

### Visibilidad

- **Public:** cualquiera con el enlace puede jugar, aunque no pase por el inicio de sesión de la web.
- **Restricted** con contraseña o *download keys*: solo entra quien la tenga, pero itch.io pedirá esa contraseña después del inicio de sesión de la web.

Elegir según si el inicio de sesión debe ser obligatorio o solo un punto de entrada.

## 6. Después de publicar

1. Actualizar la redirección de la web en `web/js/loginController.js` con la nueva URL `https://<usuario>.itch.io/biorremediacion` y desplegar en Vercel.
2. Despublicar o eliminar la página anterior (`jefferson-999.itch.io/biorremediacion`) para que no queden dos versiones.

## Nuevas versiones

Repetir los pasos 3 y 4 y reemplazar el archivo en *Edit game → Uploads* (o volver a ejecutar `butler push`, subiendo `--userversion`). La URL no cambia, así que no hay que tocar la web.
