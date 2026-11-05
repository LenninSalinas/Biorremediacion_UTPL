require('dotenv').config();
const express = require('express');
const path = require('path');
const app = express();

app.use(express.json()); // Permite leer los datos JSON del frontend
app.use(express.static(path.join(__dirname))); // Sirve los archivos HTML, CSS y JS estáticos

// Este es el endpoint que el frontend está llamando
app.post('/api/login-proxy', async (req, res) => {
    const { username, password } = req.body;
    const API_URL = process.env.API_URL;
    const AUTH_TOKEN = process.env.AUTH_TOKEN;

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

        const data = await response.json();
        res.json(data); // Devuelve la respuesta limpia al frontend
    } catch (error) {
        console.error("Error en el servidor intermediario:", error);
        res.status(500).json({ errorMessage: "Error de conexión interna." });
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
