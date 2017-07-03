using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public GameObject fighterPrefab;
    SelectionScreen selectionScreen;

    private void Start()
    {
        selectionScreen = transform.root.GetComponent<SelectionScreen>();
    }

    public void SelectFighter()
    {
        selectionScreen.SelectedFighter(fighterPrefab);
    }


}
