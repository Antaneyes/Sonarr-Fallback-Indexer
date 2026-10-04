# Sonarr - Custom Power Edition 🚀

Este repositorio es un fork de **Sonarr (v4)** que incluye mejoras críticas para la automatización y gestión de indexadores.

## 🛠️ Modificaciones Incluidas

### 1. Fix de Importación Automática (ID Match)
Se ha modificado el motor de importación para permitir que Sonarr procese descargas automáticamente incluso cuando la serie ha sido emparejada mediante su ID (en lugar de por el título exacto).
- **Problema original:** Sonarr bloqueaba la importación con el error "Series title mismatch" si el nombre del release no coincidía perfectamente, aunque el histórico confirmara que era la serie correcta.
- **Solución:** Se ha habilitado la importación automática en estos casos, reduciendo drásticamente la intervención manual necesaria.

---

### 2. Fallback Indexers (Indexadores de Respaldo)
Nueva lógica de búsqueda secuencial para optimizar el uso de tus indexadores.
- **Búsqueda Automática Inteligente:** Solo consulta los indexadores de "Fallback" si los indexadores principales no devuelven resultados aprobados. Ideal para ahorrar API y tiempo en indexadores lentos o con límites bajos.
- **Búsqueda Interactiva bajo demanda:** Los indexadores de fallback se ocultan de la búsqueda interactiva inicial. Se ha añadido un botón **"Buscar en Fallback"** para consultarlos solo cuando el usuario lo decida.
- **Interfaz UI:** Incluye checkboxes de configuración y etiquetas visuales en el listado de indexadores.

---

### 3. Soporte MicroHD (mHD)
Reconocimiento y gestión nativa de formatos mHD para una biblioteca más eficiente.
- **Detección Inteligente:** Identifica tags como `mHD`, `microHD`, `m1080`, `m720`, `m4k` y `muhd`, asignándoles su propia categoría de calidad.
- **Sincronización Automática:** Al arrancar, Sonarr integra automáticamente estas nuevas variantes en tus perfiles de calidad existentes.
- **Control de Precisión:** Permite priorizar versiones de alta calidad y bajo peso (mHD) frente a versiones estándar o pesadas (Remux) directamente desde los ajustes de perfil.

---

### 4. Títulos de Series en Español
La UI puede mostrar títulos localizados al español sin cambiar la lógica de búsqueda de Sonarr.
- **Título visible:** Las respuestas de la API incluyen `displayTitle`, usado por la UI para mostrar el título español cuando existe.
- **Nuevas carpetas:** Al añadir series nuevas, Sonarr puede usar el título traducido para la carpeta inicial si la traducción ya está disponible.
- **Fuente TMDb:** Las traducciones se obtienen desde TMDb usando `es-ES`, primero desde la ficha localizada y después desde el endpoint de traducciones como respaldo.
- **Caché local:** Los títulos se guardan en la tabla `SeriesTranslations` para no consultar TMDb en cada renderizado.
- **Protección de metadatos:** Si SkyHook devuelve un `TitleSlug` que ya pertenece a otra serie, se omite esa actualización para evitar corromper la serie existente.

Para activar esta función, configura una API key de TMDb en los ajustes generales.

---

## 🚀 Despliegue con Docker

El `Dockerfile` de este repo compila Sonarr completo (backend y UI) y genera una imagen basada en la de LinuxServer, con el programa original sustituido entero por esta versión:

```bash
docker build -t local/sonarr-fallback:<versión> .
```

Después basta con usar esa imagen en el compose (`image: local/sonarr-fallback:<versión>`), sin montar binarios. El `docker-compose.yml` del repo es un ejemplo que la construye directamente (`docker compose up -d --build`, o `launch_sonarr.bat` en Windows). La interfaz estará en `http://localhost:8989`.

> [!IMPORTANT]
> El backend se compila con el SDK exacto que fija `global.json` (6.0.405). Con otro SDK 6.0 se empaquetan versiones distintas de algunas DLL y Sonarr no arranca. `LSIO_TAG` en el `Dockerfile` debe ser la versión de upstream en la que se basa el fork (p. ej. `4.0.20.3014-ls326`); actualízala cada vez que se fusione una versión nueva de Sonarr.

## 📝 Detalles Técnicos
- **Base:** Sonarr v4 (v4.0.13+).
- **Backend:** Cambios en `NzbDrone.Core` (ReleaseSearchService, CompletedDownloadService, Migraciones, traducciones TMDb).
- **Idioma:** Localización completa al Español (corregida) e Inglés.
- **Frontend:** React + Redux con nuevos componentes en InteractiveSearch, Indexer Settings y visualización de `displayTitle`.

---
*Desarrollado para coleccionistas que buscan el máximo nivel de automatización.*
