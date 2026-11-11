# Documentación — Biorremediación UTPL

Esta carpeta reúne la documentación técnica del proyecto.

## Estructura del repositorio

| Carpeta | Contenido |
|---|---|
| `simulador/` | Proyecto de Unity 6 (`simulador/Biorremediacion_UTPL`). |
| `web/` | Página de inicio de sesión (Express) desplegada en Vercel. |
| `documentacion/` | Documentación técnica, manuales y decisiones del proyecto. |

## Documentos

- [Limpieza del proyecto](limpieza-proyecto.md): elementos eliminados (escenas, scripts, paquetes y versiones compiladas) y cómo se comprobó que no se usaban.

## Requisitos para trabajar en el proyecto

- **Git LFS** instalado en cada equipo (`git lfs install`, una sola vez). Sin él, los modelos, texturas, audio, terrenos e iluminación se descargan como punteros y Unity no puede leerlos.
- **Unity 6** (6000.0.38f1).
- **Node.js** para ejecutar la web en local (`npm install` dentro de `web/`).

## Variables de entorno de la web

La web necesita estas variables. En local van en `web/.env` (ignorado por Git; ver `web/.env.example`) y en producción en *Vercel → Settings → Environment Variables*:

| Variable | Descripción |
|---|---|
| `API_URL` | URL del servicio de autenticación. |
| `AUTH_TOKEN` | Token de autorización del servicio. **Nunca** se escribe en el código. |

## Flujo de trabajo

- La rama `main` está protegida: todo cambio entra por *pull request* con al menos una aprobación.
- No se suben versiones compiladas del simulador al repositorio; la build web se publica en itch.io.
