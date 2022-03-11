using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{
    public Image flashImage;
    private float flashSpeed = 5f;
    //Time the flash lasts for
    private Color flashColor;
    // The values above correspond to: R - G - B - Alpha these values would produce an opaque white flash.
    private bool activate = false;
 

    void Update()
    {
        if (activate)
        {
            flashImage.color = flashColor;
        }
        else
        {
            flashImage.color = Color.Lerp(flashImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        activate = false;
    }

    public void StartFlash(Color color, float speed)
    {
        flashColor = color;
        flashSpeed = speed;
        activate = true;
    }
}

