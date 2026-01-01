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

## 🚀 Despliegue con Docker (Listo para usar)

Este repo incluye todo lo necesario para correr Sonarr con estos cambios en segundos usando Docker:

1. **Configurar:** Ajusta las rutas en `docker-compose.yml`.
2. **Lanzar:** Ejecuta `launch_sonarr.bat` (en Windows) o `docker-compose up -d`.
3. **Acceso:** Entra en `http://localhost:8989`.

> [!NOTE]
> El despliegue de Docker monta automáticamente los binarios compilados y las traducciones corregidas en la imagen oficial de LinuxServer.

## 📝 Detalles Técnicos
- **Base:** Sonarr v4 (v4.0.13+).
- **Backend:** Cambios en `NzbDrone.Core` (ReleaseSearchService, CompletedDownloadService, Migraciones).
- **Idioma:** Localización completa al Español (corregida) e Inglés.
- **Frontend:** React + Redux con nuevos componentes en InteractiveSearch y Indexer Settings.

---
*Desarrollado para coleccionistas que buscan el máximo nivel de automatización.*
