require('dotenv').config();
const express = require('express');
const path = require('path');
const app = express();

// Vercel y otros proxies envían la IP real del usuario en X-Forwarded-For
app.set('trust proxy', 1);
app.disable('x-powered-by');

// Cabeceras de seguridad básicas
app.use((req, res, next) => {
    res.setHeader('X-Content-Type-Options', 'nosniff');
    res.setHeader('X-Frame-Options', 'DENY');
    res.setHeader('Referrer-Policy', 'strict-origin-when-cross-origin');
    next();
});

app.use(express.json({ limit: '10kb' })); // Permite leer los datos JSON del frontend

// El frontend vive en public/. Solo se publica esa carpeta: server.js, package.json y .env NO se sirven.
// En Vercel esta línea se ignora: la CDN de Vercel sirve public/ directamente.
// Se mantiene para que la web también funcione al ejecutarla en local con `node server.js`.
app.use(express.static(path.join(__dirname, 'public')));

// Límite de intentos de inicio de sesión por IP (evita ataques de fuerza bruta)
const MAX_INTENTOS = 10;
const VENTANA_MS = 15 * 60 * 1000; // 15 minutos
const intentos = new Map();

function superaLimite(ip) {
    const ahora = Date.now();
    const registro = intentos.get(ip);
    if (!registro || ahora - registro.inicio > VENTANA_MS) {
        intentos.set(ip, { inicio: ahora, cantidad: 1 });
        return false;
    }
    registro.cantidad++;
    return registro.cantidad > MAX_INTENTOS;
}

// Limpia registros viejos para no acumular memoria
setInterval(() => {
    const ahora = Date.now();
    for (const [ip, registro] of intentos) {
        if (ahora - registro.inicio > VENTANA_MS) intentos.delete(ip);
    }
}, VENTANA_MS).unref();

// Este es el endpoint que el frontend está llamando
app.post('/api/login-proxy', async (req, res) => {
    if (superaLimite(req.ip)) {
        return res.status(429).json({ errorMessage: "Demasiados intentos. Espera unos minutos e inténtalo de nuevo." });
    }

    const { username, password } = req.body || {};

    // Validación en el servidor (no se confía solo en la del navegador)
    if (typeof username !== 'string' || !/^[A-Za-z0-9._-]{1,64}$/.test(username) ||
        typeof password !== 'string' || password.length < 1 || password.length > 128) {
        return res.status(400).json({ errorMessage: "Datos de inicio de sesión inválidos." });
    }

    const API_URL = process.env.API_URL;
    const AUTH_TOKEN = process.env.AUTH_TOKEN;
    if (!API_URL || !AUTH_TOKEN) {
        console.error("Faltan las variables de entorno API_URL o AUTH_TOKEN.");
        return res.status(500).json({ errorMessage: "Error de configuración del servidor." });
    }

    try {
        // El servidor hace la petición real con el token oculto
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
                'Authorization': AUTH_TOKEN
            },
            body: JSON.stringify({ username, password })
        });

        const data = await response.json().catch(() => ({}));

        if (!response.ok || data.errorMessage) {
            return res.status(401).json({ errorMessage: "Correo o contraseña incorrectos." });
        }

        // Solo se devuelven los datos que usa el frontend (no cédula ni otros datos personales)
        res.json({
            firstName: data.firstName || "",
            lastName: data.lastName || "",
            role: { code: data.role && data.role.code ? data.role.code : "" },
            permissions: Array.isArray(data.permissions)
                ? data.permissions.map(p => ({ code: p.code }))
                : []
        });
    } catch (error) {
        console.error("Error en el servidor intermediario:", error.message);
        res.status(502).json({ errorMessage: "Error de conexión interna." });
    }
});

// Solo iniciar el servidor de escucha si estamos en entorno local
if (process.env.NODE_ENV !== 'production') {
    const PORT = process.env.PORT || 3000;
    app.listen(PORT, () => {
        console.log(`Servidor local ejecutándose en el puerto ${PORT}`);
    });
}

// Exportar la app para que Vercel la pueda procesar como Serverless Function
module.exports = app;
