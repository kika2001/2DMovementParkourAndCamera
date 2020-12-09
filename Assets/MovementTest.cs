using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    [SerializeField] private List<MovementExtras> extras = new List<MovementExtras>();

    private void OnValidate()
    {
        extras.Clear();
        foreach (var component in GetComponents<Component>())
        {
            if (component.GetComponent<MovementExtras>())
            {
                extras.Add(component.GetComponent<MovementExtras>());
            }
        }
    }

    private void Start()
    {
        Debug.Log("Extras Herd");
        foreach (var extra in extras)
        {
            extra.ActionStart();
        }
    }
}
