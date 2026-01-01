# Guía de Implementación: Indexadores Fallback (Sonarr)

Esta guía detalla los cambios necesarios para replicar la funcionalidad de "Indexador de Fallback" y "Búsqueda Interactiva en Fallback" en Sonarr, basada en la implementación exitosa realizada en Radarr.

---

## 1. Base de Datos (Migración)
Añadir la columna `IsFallback` a la tabla `Indexers`.

**Archivo sugerido:** `src/NzbDrone.Core/Datastore/Migration/[SiguienteNumero]_add_is_fallback_to_indexers.cs`

```csharp
[Migration(20251229)] // Usa la fecha actual o el siguiente número
public class add_is_fallback_to_indexers : NzbDroneMigrationBase
{
    protected override void MainDbUpgrade()
    {
        Alter.Table("Indexers").AddColumn("IsFallback").AsBoolean().WithDefaultValue(false);
    }
}
```

---

## 2. Core (Backend C#)

### A. Modelo de Datos
Añadir la propiedad al modelo base de los indexadores.

**Archivo:** `src/NzbDrone.Core/Indexers/IndexerDefinition.cs`
```csharp
public bool IsFallback { get; set; }
```

### B. Criterios de Búsqueda
Permitir que las búsquedas soliciten explícitamente incluir indexadores de fallback.

**Archivo:** `src/NzbDrone.Core/IndexerSearch/Definitions/SearchCriteriaBase.cs`
```csharp
public bool IncludeFallback { get; set; }
```

### C. Lógica de Búsqueda (El "Corazón")
Modificar el despacho de búsquedas para que sea secuencial. Primero busca en indexadores normales; si no hay resultados aprobados, busca en los de fallback.

**Archivo:** `src/NzbDrone.Core/IndexerSearch/ReleaseSearchService.cs` (Método `Dispatch`)
```csharp
// 1. Separar indexadores
var normalIndexers = indexers.Where(i => !((IndexerDefinition)i.Definition).IsFallback).ToList();
var fallbackIndexers = indexers.Where(i => ((IndexerDefinition)i.Definition).IsFallback).ToList();

// 2. Buscar en normales
var tasks = normalIndexers.Select(indexer => DispatchIndexer(searchAction, indexer, criteriaBase));
var reports = (await Task.WhenAll(tasks)).SelectMany(x => x).ToList();
var resultsFound = _makeDownloadDecision.GetSearchDecision(reports, criteriaBase).ToList();

// 3. Condición de Fallback: Si es búsqueda interactiva forzada O si no hay resultados aprobados en automática
if (fallbackIndexers.Any() && ((!criteriaBase.InteractiveSearch && !resultsFound.Any(d => d.Approved)) || criteriaBase.IncludeFallback))
{
    var fallbackTasks = fallbackIndexers.Select(indexer => DispatchIndexer(searchAction, indexer, criteriaBase));
    reports.AddRange((await Task.WhenAll(fallbackTasks)).SelectMany(x => x));
    resultsFound = _makeDownloadDecision.GetSearchDecision(reports, criteriaBase).ToList();
}
```

---

## 3. API (V3)

### A. Recursos y Mapeo
Asegurar que la propiedad viaje entre el servidor y la interfaz.

**Archivo:** `src/Sonarr.Api.V3/Indexers/IndexerResource.cs`
```csharp
public bool IsFallback { get; set; }
```
*(Y actualizar el mapper correspondiente para copiar el valor entre Definition y Resource).*

### B. Controlador de Búsqueda
Aceptar el parámetro `includeFallback` en los endpoints de búsqueda (ej. `ReleaseController`).

---

## 4. Frontend (React / Redux)

### A. Configuración de Indexadores
Añadir el checkbox en el modal de edición.

**Archivo:** `frontend/src/Settings/Indexers/Indexers/EditIndexerModalContent.tsx`
```tsx
<FormGroup>
  <FormLabel>Fallback</FormLabel>
  <FormInputGroup
    type={inputTypes.CHECK}
    name="isFallback"
    helpText="Si está activado, este indexador solo se consultará si los demás no devuelven resultados válidos."
    {...isFallback}
    onChange={handleInputChange}
  />
</FormGroup>
```

### B. Búsqueda Interactiva
Añadir el botón para forzar la búsqueda en fallback.

**Archivo:** `frontend/src/InteractiveSearch/InteractiveSearch.tsx`
```tsx
<Button
  className={styles.fallbackButton}
  onPress={() => dispatch(fetchReleases({ ...searchPayload, includeFallback: true }))}
  title="Buscar en indexadores de fallback (más lento)"
>
  <Icon name={icons.SEARCH} /> Buscar en Fallback
</Button>
```

---

## 5. Resumen de flujos (Arquitectura)
1. **Automático:** Búsqueda normal -> ¿Hay resultados? No -> Búsqueda Fallback.
2. **Interactivo:** Lista normal de resultados + Botón "Buscar en Fallback" para añadir los lentos bajo demanda.
