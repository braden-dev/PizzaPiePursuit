using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    public float inputAlpha = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // https://forum.unity.com/threads/making-material-transparant-in-universal-rp.1216053/
        var mats = GetComponent<Renderer>().materials;
        var newMats = new Material[mats.Length];

        for (var i = 0; i < mats.Length; i++)
        {
            var mat = new Material(mats[i]);

            // Set surface type to Transparent
            mat.SetFloat("_Surface", 1.0f);

            // Set Blending Mode to Alpha
            mat.SetFloat("_Blend", 0.0f);    

            // Set alpha
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, inputAlpha);  
            
            newMats[i] = mat;
        }
        GetComponent<Renderer>().materials = newMats;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
