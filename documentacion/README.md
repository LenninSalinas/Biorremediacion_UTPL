# Documentación — Biorremediación UTPL

Esta carpeta reúne la documentación técnica del proyecto.

## Estructura del repositorio

| Carpeta | Contenido |
|---|---|
| `simulador/` | Proyecto de Unity 6 (`simulador/Biorremediacion_UTPL`). |
| `web/` | Página de inicio de sesión (Express) desplegada en Vercel. El frontend está en `web/public/` y el servidor en `web/server.js`. |
| `documentacion/` | Documentación técnica, manuales y decisiones del proyecto. |

## Documentos

- [Limpieza del proyecto](limpieza-proyecto.md): elementos eliminados (escenas, scripts, paquetes y versiones compiladas) y cómo se comprobó que no se usaban.
- [Publicación en itch.io](publicacion-itchio.md): cómo generar la build web, empaquetarla y publicarla en la página institucional.

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

## Despliegue de la web en Vercel

| Ajuste del proyecto en Vercel | Valor |
|---|---|
| Root Directory | `web` |
| Framework Preset | Express |
| Rama de producción | `main` (cada rama y cada PR genera una versión de prueba *Preview*) |

Vercel ejecuta `server.js` como una función y sirve los archivos de `public/` desde su CDN. Por eso todo el frontend (HTML, CSS, JS e imágenes) **tiene que estar dentro de `public/`**: con Express, Vercel ignora `express.static()`. Las cabeceras de seguridad de los archivos estáticos se definen en `web/vercel.json`.

Para probar en local: `npm install` y `node server.js` dentro de `web/`, y abrir `http://localhost:3000`.

## Flujo de trabajo

- La rama `main` está protegida: todo cambio entra por *pull request* con al menos una aprobación.
- No se suben versiones compiladas del simulador al repositorio; la build web se publica en itch.io.
