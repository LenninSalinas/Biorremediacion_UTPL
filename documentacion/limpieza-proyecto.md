# Limpieza del proyecto

**Fecha:** 10 de noviembre de 2026
**Tarea:** Eliminar escenas duplicadas, scripts sin uso, paquetes no utilizados, la carpeta de dependencias web y las versiones compiladas, comprobando antes que ninguna escena los usa.

## Resumen

| Categoría | Elementos eliminados |
|---|---|
| Escenas duplicadas | 3 escenas (7 archivos con sus `.meta`) |
| Scripts sin uso | 3 scripts (6 archivos con sus `.meta`) |
| Paquetes de Unity | 6 paquetes |
| Versión compilada | 17 archivos (92,2 MB) |
| Dependencias web | Ninguno (no estaban en el repositorio) |
| Manuales PDF de paquetes | 2 archivos (con sus `.meta` y una carpeta) |

## Cómo se comprobó que nada los usa

- **Escenas:** se revisó `ProjectSettings/EditorBuildSettings.asset` (escenas incluidas en la compilación), se buscó el GUID de cada escena en todas las escenas, prefabs y assets, y se revisaron las llamadas a `SceneManager.LoadScene` en los scripts.
- **Scripts:** se buscó el GUID de cada script en escenas, prefabs, assets, controladores y animaciones, y el nombre de su clase en el resto de scripts.
- **Paquetes:** se buscaron los GUID de los scripts de cada paquete en los assets del proyecto, su uso en el código (`using`) y si otro paquete depende de él (`Packages/packages-lock.json`).
- **Versión compilada:** se comprobó que ninguna página ni script de la web la enlaza.

---

## 1. Escenas duplicadas

Carpeta eliminada: `simulador/Biorremediacion_UTPL/Assets/Prefabs(Not Available)/Scenes/`

| Archivo eliminado | Tamaño | Escena original que se conserva |
|---|---|---|
| `DemoScene.unity` + `.meta` | 126 KB | `Assets/Scenes/DemoScene.unity` |
| `MenuPrincipal.unity` + `.meta` | 16 KB | `Assets/Scenes/MenuPrincipal.unity` |
| `SampleScene 1.unity` + `.meta` | 55 KB | `Assets/Scenes/SampleScene.unity` |
| `Scenes.meta` (de la carpeta) | — | — |

**Motivo:** no están en Build Settings, ningún archivo las referencia y ningún script las carga.

> **Importante:** el resto de la carpeta `Prefabs(Not Available)` **se conserva**. A pesar de su nombre, 12 de sus archivos se usan en el proyecto (entre ellos `Player.prefab`, los modelos de botellas, GPS y multiparámetro, y `Global Volume.prefab`).

## 2. Scripts sin uso

Carpeta: `simulador/Biorremediacion_UTPL/Assets/Scripts/`

| Archivo eliminado | Motivo |
|---|---|
| `FadeInOut.cs` + `.meta` | No está asignado en ninguna escena ni prefab, y ningún script lo usa. |
| `SimpleCameraController.cs` + `.meta` | No está asignado en ninguna escena ni prefab, y ningún script lo usa. |
| `WaterfallAudio.cs` + `.meta` | No está asignado en ninguna escena ni prefab, y ningún script lo usa. |

Los otros 14 scripts de la carpeta sí se usan y se conservan.

## 3. Paquetes de Unity

Quitados de `simulador/Biorremediacion_UTPL/Packages/manifest.json`:

| Paquete | Versión | Motivo |
|---|---|---|
| `com.unity.collab-proxy` (Unity VCS) | 2.12.4 | El proyecto usa Git, no Unity Version Control. |
| `com.unity.ide.rider` | 3.0.31 | El equipo no usa el editor Rider (se usa Visual Studio / VS Code). |
| `com.unity.multiplayer.center` | 1.0.0 | El simulador no es multijugador. |
| `com.unity.recorder` | 5.1.2 | Ningún asset ni script lo usa. |
| `com.unity.timeline` | 1.8.7 | Sin uso; solo lo requería `com.unity.recorder`. |
| `com.unity.visualscripting` | 1.9.5 | No hay grafos de Visual Scripting ni código que lo use. |

**Paquetes que se conservan** porque se usan: AI Navigation, Animation Rigging, Input System, Universal Render Pipeline, Terrain Tools, UGUI y Visual Studio Editor. `com.unity.test-framework` también se conserva porque lo requieren Animation Rigging y Visual Studio Editor.

## 4. Versión compilada

Carpeta eliminada: `web/simulador/` (17 archivos, 92,2 MB)

| Ubicación | Archivos |
|---|---|
| `web/simulador/` | `index.html` |
| `web/simulador/Build/` | `simulador.data.unityweb` (88 MB), `simulador.framework.js.unityweb`, `simulador.loader.js`, `simulador.wasm.unityweb` |
| `web/simulador/TemplateData/` | `favicon.ico`, `fullscreen-button.png`, `MemoryProfiler.png`, `progress-bar-empty-dark.png`, `progress-bar-empty-light.png`, `progress-bar-full-dark.png`, `progress-bar-full-light.png`, `style.css`, `unity-logo-dark.png`, `unity-logo-light.png`, `unity-logo-title-footer.png`, `webmemd-icon.png` |

**Motivo:** era una compilación antigua del simulador. La web no la usa, porque después del inicio de sesión redirige a la página de itch.io, donde se publica la versión actual. Las nuevas compilaciones se generan fuera del repositorio.

## 5. Carpeta de dependencias web

No se eliminó nada: la carpeta `web/node_modules/` no estaba en el repositorio y ya está excluida en `.gitignore`. Las dependencias se instalan con `npm install` dentro de `web/`.

## 6. Manuales PDF de paquetes

| Archivo eliminado | Paquete |
|---|---|
| `Assets/BOKI/LowPolyNature/Documentation/BOKI - Low Poly Nature.pdf` + `.meta` (y la carpeta `Documentation`) | BOKI Low Poly Nature |
| `Assets/Construction Vehicles 1.1/CONSTRUCTION VEHICLE.pdf` + `.meta` | Construction Vehicles 1.1 |

**Motivo:** son los manuales que el autor de cada paquete de la Asset Store incluye. Ningún asset los referencia y no forman parte de la build.

---

## Cambios relacionados

| Archivo | Cambio |
|---|---|
| `.gitattributes` | Se reemplazaron `web/** -filter -diff -merge` y `web/** binary` por reglas que solo marcan las imágenes como binarias, para que los cambios del código web se vean en los pull requests. |
| `web/js/apiService.js` | Se dejó de guardar el género y la cédula del usuario en `localStorage`. |
| `web/server.js` | Se reemplazó `express.static(__dirname)`, que publicaba toda la carpeta (incluidos `server.js` y `package.json`), por la publicación solo de `index.html`, `css/`, `js/` e `imgs/`. |
| `simulador/.../Assets/Scripts/Change_Scenes.cs` | Se actualizó el nombre de la escena cargada de `"SampleScene 1"` a `"SampleScene"` tras su renombre. |

## Cómo recuperar un elemento

Mientras los cambios no estén confirmados (commit), cualquier archivo se recupera con:

```bash
git restore "<ruta del archivo>"
```

Después del commit, se puede recuperar desde el historial de Git.
