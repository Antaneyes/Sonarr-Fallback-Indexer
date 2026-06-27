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

## 🚀 Despliegue con Docker (Listo para usar)

Este repo incluye todo lo necesario para correr Sonarr con estos cambios en segundos usando Docker:

1. **Configurar:** Ajusta las rutas en `docker-compose.yml`.
2. **Lanzar:** Ejecuta `launch_sonarr.bat` (en Windows) o `docker-compose up -d`.
3. **Acceso:** Entra en `http://localhost:8989`.

> [!NOTE]
> El despliegue de Docker monta automáticamente los binarios compilados y las traducciones corregidas en la imagen oficial de LinuxServer.

> [!IMPORTANT]
> La interfaz compilada debe montarse en `/app/sonarr/bin/UI`. Sonarr sirve los assets desde el content root `/app/sonarr/bin`; si se monta en `/app/sonarr/UI`, el backend puede estar parcheado pero la UI seguirá mostrando la versión original de la imagen.

> [!IMPORTANT]
> Si montas el directorio completo de salida sobre `/app/sonarr/bin`, conserva el apphost `Sonarr` y las librerías nativas (`*.so`) extraídas desde la imagen `linuxserver/sonarr:latest`. Esto evita mezclar binarios locales glibc con la base Alpine/musl de LinuxServer.

## 📝 Detalles Técnicos
- **Base:** Sonarr v4 (v4.0.13+).
- **Backend:** Cambios en `NzbDrone.Core` (ReleaseSearchService, CompletedDownloadService, Migraciones, traducciones TMDb).
- **Idioma:** Localización completa al Español (corregida) e Inglés.
- **Frontend:** React + Redux con nuevos componentes en InteractiveSearch, Indexer Settings y visualización de `displayTitle`.

---
*Desarrollado para coleccionistas que buscan el máximo nivel de automatización.*
