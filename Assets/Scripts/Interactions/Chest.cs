using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Transform hinge; 
    [SerializeField]
    private Sprite icon;

    private bool open;
    private bool moving;

    private IEnumerator OpenClose(bool opening)
    {
        moving = true;

        // Open the lid over time
        float start = Time.time;
        float period = 1f;
        while(true)
        {
            float progress = (Time.time - start) / period;
            if (progress > 1f)
                progress = 1f;

            float angle = progress * 90f;
            if (!opening)
                angle = 90f - angle;

            hinge.localRotation = 
                Quaternion.Euler(new(angle, 0, 0));
            
            if (progress == 1f)
                break;

            yield return null;
        }
        open = opening;
        moving = false;
    }

    public void Interact()
    {
        if(!moving)
            StartCoroutine(OpenClose(!open));
    }

    public string GetName()
    {
        return "Chest";
    }

    public string GetDescription()
    {
        return "It's full of gold!";
    }

    public Sprite GetIcon()
    {
        return icon;
    }
}
