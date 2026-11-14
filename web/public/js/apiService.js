async function loginWeb(username, password) {
    const url = "/api/login-proxy";

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
            },
            body: JSON.stringify({
                username: username,
                password: password
            })
        });

        const data = await response.json();

        // Si el servidor devuelve un mensaje de error, lo mostramos en la consola
        if (data.errorMessage) {
            console.error("Error del servidor:", data.errorMessage);
            return false;
        }

        // login exitoso
        const primerNombre = data.firstName.split(' ')[0];
        const primerApellido = data.lastName.split(' ')[0];
        const nombreCompleto = `${primerNombre} ${primerApellido}`;

        // convertirmos los permisos a texto
        let permissions = "";
        if (data.permissions && data.permissions.length > 0) {
            permissions = data.permissions.map(p => p.code).join(',');
        }

        // Guardamos los datos en el navegador para mantener la sesión
        localStorage.setItem("userName", username);
        localStorage.setItem("userFirstName", nombreCompleto);
        localStorage.setItem("userRole", data.role.code);
        localStorage.setItem("userPermissions", permissions);
        // Ya no se guardan la cédula ni el género: son datos personales que la web no necesita.
        localStorage.removeItem("userGenere");
        localStorage.removeItem("userIdentification");

        console.log("!Login exitoso! Datos guardados.");
        return true;
    } catch (error) {
        console.error("Error de red conectando al servidor:", error);
        return false;
    }
}
