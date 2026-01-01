@echo off
echo Levantando Sonarr con tus cambios personalizados (Fallback Indexers)...
docker-compose up -d
echo Sonarr deberia estar disponible en http://localhost:6989 en unos segundos.
pause
