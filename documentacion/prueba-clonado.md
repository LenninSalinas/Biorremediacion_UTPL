# Prueba de clonado desde cero

**Fecha:** 16 de noviembre de 2026
**Objetivo:** comprobar que cualquier integrante puede descargar el repositorio, abrirlo en Unity y compilarlo sin pasos manuales ni archivos que falten.
**Resultado:** ✅ correcta.

## Entorno

| Elemento | Versión |
|---|---|
| Sistema operativo | Windows 11 Pro |
| Git | 2.56.0 |
| Git LFS | 3.8.1 |
| Unity | 6000.0.38f1, con el módulo *WebGL Build Support* |
| Repositorio | `LenninSalinas/Biorremediacion_UTPL`, rama `main`, commit `b729bba` (fusión del PR #4) |

Se clonó en una carpeta vacía y corta (`%TEMP%\pc`), sin carpeta `Library/` ni ningún archivo previo del proyecto.

## 1. Clonado

```bash
git clone https://github.com/LenninSalinas/Biorremediacion_UTPL.git
```

| Comprobación | Resultado |
|---|---|
| Tiempo de clonado (incluida la descarga de LFS) | 3 min 23 s |
| Archivos en el repositorio | 1307 |
| Archivos en Git LFS | 204 (836 MB) |
| Archivos LFS sin descargar | **0** |
| Archivos que quedaron como puntero LFS en vez del archivo real | **0** |
| Cambios pendientes justo después de clonar (`git status`) | **0** |
| Tamaño en disco | 853 MB de proyecto + 1,1 GB de `.git` |

## 2. Apertura y compilación en Unity

Se abrió el proyecto `simulador/Biorremediacion_UTPL` con Unity 6000.0.38f1 en modo consola y se generó la build web con las escenas de *Build Settings*.

| Comprobación | Resultado |
|---|---|
| Primera importación de assets | 1 min 50 s |
| Errores de compilación de scripts | **0** |
| Scripts o prefabs faltantes en escenas | **0** |
| Escenas incluidas | `MenuPrincipal`, `SampleScene`, `DemoScene` |
| Plantilla web / compresión | `UTPL` / Gzip con *Decompression Fallback* |
| Resultado de la build | **Succeeded**: 0 errores, 14 avisos |
| Duración de la build | 26 min 19 s (30 min en total con la importación) |
| Tamaño de la build | 76,6 MB |

Los 14 avisos son normales en una build web y no afectan al juego:
- Unity descarta shaders que WebGL no admite (HDRP de TextMeshPro y visualización de terrenos).
- IL2CPP avisa de que algunos métodos internos de Unity son grandes.

## 3. Prueba de la build

La build generada desde el clon se sirvió en local y se abrió en el navegador. Cargó hasta el **menú principal** sin errores en la consola.

La misma configuración está publicada en <https://biorremediacion-utpl.itch.io/biorremediacion>. Allí se comprobó a mano el paso del menú a la escena, el movimiento y el sonido del agua en `DemoScene`.

## Requisitos que confirma la prueba

Para que la descarga funcione a la primera, cada integrante necesita:

1. **Git LFS instalado antes de clonar** (`git lfs install`, una sola vez por equipo). Sin él, los modelos, texturas y audios bajan como punteros de texto y Unity no los puede leer.
2. **Unity 6000.0.38f1** con *WebGL Build Support*, si va a generar la build web.
3. **Clonar en una ruta corta** (por ejemplo `C:\proyectos`). Algunas rutas del proyecto tienen 160 caracteres, y Windows no admite rutas de más de 260 si no están activadas las rutas largas. Alternativa: `git config --global core.longpaths true`.
4. La primera apertura en Unity tarda unos minutos porque genera la carpeta `Library/`. Esa carpeta no se sube al repositorio.
