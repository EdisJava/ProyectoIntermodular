using System.Collections.Generic;
using UnityEngine;

// Encuentra los componentes DoorButtonInteraction (o GameObjects con tag "PuertaInteractiva")
// y actualiza su overlay/estado visual según lo que DayManager publique.
// No modifica DoorButtonInteraction: busca un GameObject hijo 'disabledOverlay' y lo activa/desactiva.
public class DoorAvailabilityBridge : MonoBehaviour
{
    public DayManager dayManager;
    public string doorTag = "PuertaInteractiva";

    void Start()
    {
        if (dayManager == null) dayManager = FindObjectOfType<DayManager>();
        if (dayManager != null) dayManager.OnAvailableAreasChanged += HandleAvailableAreasChanged;

        // Inicial
        UpdateAllDoorVisuals(dayManager?.GetAvailableAreasForCurrentDay() ?? new List<string>());
    }

    void OnDestroy()
    {
        if (dayManager != null) dayManager.OnAvailableAreasChanged -= HandleAvailableAreasChanged;
    }

    void HandleAvailableAreasChanged(List<string> available)
    {
        UpdateAllDoorVisuals(available);
    }

    void UpdateAllDoorVisuals(List<string> available)
    {
        var doors = GameObject.FindGameObjectsWithTag(doorTag);
        foreach (var d in doors)
        {
            // Se asume que el GameObject de la puerta tiene un nombre o un componente con identificador.
            // Aquí intentamos usar el nombre del GameObject como identificador; si usas otro campo,
            // ajusta esta lógica (por ejemplo, un componente DoorButtonInteraction.areaName).
            string id = d.name;
            // Si DoorButtonInteraction tiene un campo público areaName, preferimos usarlo:
            var dbi = d.GetComponentInChildren<DoorButtonInteraction>();
            if (dbi != null)
            {
                var areaNameField = dbi.GetType().GetField("areaName");
                if (areaNameField != null)
                {
                    var val = areaNameField.GetValue(dbi) as string;
                    if (!string.IsNullOrEmpty(val)) id = val;
                }
            }

            bool availableFlag = available.Contains(id);
            // Buscar child llamado "disabledOverlay" y activarlo si NO disponible
            var overlay = d.transform.Find("disabledOverlay")?.gameObject;
            if (overlay != null) overlay.SetActive(!availableFlag);

            // Opcional: cambiar material o color
            var renderer = d.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = availableFlag ? Color.white : Color.gray;
            }
        }
    }
}