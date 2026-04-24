# Proyecto Intermodular — Juego narrativo 3D (Unity)

**ProyectoIntermodular** es un juego narrativo 3D creado en **Unity** con estética **retro / low poly**. El protagonista debe **encontrar al alumno que está molestando al resto** a través de **preguntas** para finalmente **informar al profesor**.

## 🎮 Descripción del juego
En un entorno escolar, el jugador explora, interactúa con personajes y reúne pistas mediante diálogos/preguntas. El objetivo es identificar al responsable de los problemas en clase tomando decisiones y avanzando por la narrativa.

## 🧰 Tecnologías
- **Motor:** Unity
- **Lenguajes principales del repositorio:**
  - C# (scripts de gameplay)
  - ShaderLab / HLSL (shaders y efectos)
  - HTML (archivos auxiliares como logs o documentación exportada)

## 📦 Paquetes / dependencias (Unity)
El proyecto usa, entre otros, paquetes como:
- **Input System**
- **Cinemachine**
- **AI Navigation**
- **URP/HDRP Core + ShaderGraph** (según configuración del proyecto)
- **ProBuilder**
- **Timeline**
- **Visual Scripting** (habilitado en dependencias)

> Nota: las versiones exactas están definidas en `Packages/manifest.json`.

## 📁 Estructura del repositorio (resumen)
- `Assets/` — Contenido principal del juego (escenas, scripts, materiales, modelos, etc.)
- `Packages/` — Manifiesto y configuración de paquetes de Unity
- `ProjectSettings/` — Configuración del proyecto Unity
- `LiarGame/` — Carpeta adicional del proyecto/solución (contiene también `Packages/`, etc.)
- `Download.mp4` — Vídeo (probablemente demo/gameplay)
- `UpgradeLog.htm` — Log de actualización de Unity

## ▶️ Cómo abrir el proyecto
1. Clona el repositorio:
   ```bash
   git clone https://github.com/EdisJava/ProyectoIntermodular.git
   ```
2. Abre el proyecto con **Unity Hub**:
   - **Add / Añadir** → selecciona la carpeta del proyecto (la raíz que contenga `Assets`, `Packages` y `ProjectSettings`).
3. Espera a que Unity importe los assets y paquetes.

## 🕹️ Controles (por completar)
- Movimiento: *(por definir)*
- Interacción / hablar: *(por definir)*
- Menú / pausa: *(por definir)*

> Si me dices qué teclas/usas en el Input System, te lo dejo documentado aquí.

## 📽️ Demo
Revisa el archivo:
- `Download.mp4`

## ✅ Estado del proyecto
Proyecto académico / intermodular en desarrollo.

## 📄 Licencia
Este repositorio no especifica licencia todavía.  
Si quieres, puedo ayudarte a añadir una (MIT, Apache 2.0, GPL, etc.) según lo que necesites.

---
**Autor:** @EdisJava
