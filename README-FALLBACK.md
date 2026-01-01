# Sonarr - Fallback Indexers Edition

Este repositorio es un fork de la versión oficial de Sonarr (v4) que incluye la funcionalidad de **Fallback Indexers** (Indexadores de Respaldo).

## Funcionalidad: Fallback Indexers

Esta modificación permite designar ciertos indexadores como "Fallback". El comportamiento de búsqueda cambia de la siguiente manera:

1.  **Búsqueda Automática:** Sonarr primero consulta todos los indexadores normales. Si estos no devuelven ningún resultado aprobado, entonces (y solo entonces) consulta los indexadores marcados como Fallback de forma secuencial.
2.  **Búsqueda Interactiva:** Se ha añadido un botón específico **"Buscar en Fallback"** en la interfaz de búsqueda interactiva. Los indexadores de fallback no se consultan por defecto para ahorrar tiempo y recursos, a menos que el usuario lo solicite expresamente.

### Ventajas
- Ahorro de tiempo en búsquedas sobre indexadores lentos que solo sirven como respaldo.
- Evita el spam de peticiones a indexadores que tenemos con límites de API bajos.
- Priorización inteligente de tus indexadores principales.

## Despliegue con Docker

Se han incluido archivos para facilitar el despliegue rápido con tus cambios personalizados:

- `docker-compose.yml`: Configurado para usar la imagen de `linuxserver/sonarr` pero inyectando tus binarios compilados y traducciones.
- `launch_sonarr.bat`: Script para Windows que arranca el entorno de Docker con un solo clic.

### Pasos para ejecutar:
1. Asegúrate de tener Docker Desktop instalado.
2. Ejecuta `launch_sonarr.bat`.
3. Accede a `http://localhost:6989`.

## Cambios Realizados

### Backend (NzbDrone.Core)
- Nueva propiedad `IsFallback` en la definición de indexadores.
- Lógica de búsqueda secuencial en `ReleaseSearchService`.
- Migración de base de datos para soportar el nuevo campo.

### API (Sonarr.Api.V3)
- Soporte para la propiedad `IsFallback` en los recursos de indexadores.

### Frontend
- Checkbox "Fallback" en el modal de edición de indexadores.
- Etiqueta visual "Fallback" en el listado de indexadores activos.
- Botón "Buscar en Fallback" en la búsqueda interactiva.
- Localización completa al Español y Inglés para estas nuevas funciones.

---
*Desarrollado para mejorar la gestión de indexadores en entornos con múltiples fuentes de contenido.*
