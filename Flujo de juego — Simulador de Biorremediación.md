# Flujo de juego — Simulador de Biorremediación

```mermaid
flowchart TD
    A([Menú principal]) -->|Clic en botón de inicio| B[Carga DemoScene + SampleScene 1]
    B --> C([Jugador en el mundo 3D])

    C --> D{Movimiento y cámara}
    D -->|W A S D| D
    D -->|Espacio si isGrounded| D
    D -->|Mouse: girar / inclinar cámara| D

    C --> E[Buscar y agarrar guantes]
    E -->|Clic izq. sobre Guante_Item| F{¿Ambos guantes equipados?}
    F -->|No| E
    F -->|Sí| G[Puede agarrar objetos]

    G --> H[Agarrar Agarrador telescópico]
    H -->|Clic izq. mirando Agarrador, mano derecha libre| I[Agarrador equipado]

    I --> J[Acoplar sonda pH / O2 / Conductividad]
    J -->|Clic der. a ≤4m sin sonda acoplada| K[Sonda acoplada en punta del agarrador]

    K --> L[Entrar a zona de agua con sonda acoplada]
    L -->|OnTriggerEnter Water| M[Lectura automática + variación aleatoria]
    M --> N[Pantalla multiparámetro actualizada]
    L -->|OnTriggerExit Water| O[ResetearPantalla: borra las 3 lecturas]

    K --> P[Soltar sonda]
    P -->|Clic der. en cualquier dirección| Q[Sonda cae: recupera física y gravedad]

    I --> R[Soltar el agarrador]
    R -->|Clic izq. mirando al vacío| G

    G --> S[Agarrar botella con mano izquierda]
    S --> T[Entrar a zona de agua con la botella]
    T -->|OnTriggerStay / OverlapSphere| U[Botella se llena isFilled=true]

    C --> V{Dentro de zona de agua}
    V -->|Tecla F| W[Abrir formulario: pausa juego, cursor visible]
    W -->|Esc o F de nuevo, foco no en TextField| X[Cerrar formulario: reanuda juego, cursor oculto]
    V -->|Salir de la zona con formulario abierto| X

    C --> Y[GPS simulado]
    Y -.->|Pasivo, se actualiza cada frame| Y

    C --> Z{Mirar objeto con InformationObject}
    Z -->|Tecla I| AA[Abrir panel de info: pausa juego, cursor visible, oculta mira]
    AA -->|Tecla I de nuevo o Esc| AB[Cerrar panel: reanuda juego, cursor oculto, muestra mira]

    style A fill:#e8f5e9
    style C fill:#e3f2fd
    style N fill:#fff3e0
    style U fill:#fff3e0
    style W fill:#fce4ec
    style AA fill:#fce4ec
```

## Notas del diagrama

- Las ramas de **formulario (F)** y **panel de información (I)** manejan `Time.timeScale` y el cursor de forma independiente, por lo que si se activan casi al mismo tiempo pueden pisarse entre sí (ver sección 8 y 10 del mapa funcional).
- El reseteo de pantalla multiparámetro al salir del agua borra **las tres lecturas**, no solo la de la sonda que salió.
- Agarrar objetos (sondas, botella, agarrador) requiere siempre tener **ambos guantes equipados** primero.