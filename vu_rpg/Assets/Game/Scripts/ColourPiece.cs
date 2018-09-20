using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourPiece : MonoBehaviour
{

    public enum ColourType
    {
        
        BLUE,
        GREEN,
        ORANGE,
        PINK,
        PURPLE,
        RED,
        YELLOW,
        ANY,
        COUNT
    };

    [System.Serializable]
    public struct ColourObject
    {
        public ColourType colour;
        public Material backMaterial;
        // public Material imageMaterial;
    }

    public ColourObject[] colourObjects;

    private ColourType colour;

    public ColourType Colour {
        get { return colour; }
        set { SetColour(value); }
    }

    public int NumColours
    {
        get { return colourObjects.Length; }
    }

    private MeshRenderer backRenderer, imageRenderer;
    private Dictionary<ColourType, Material[]> colourObjectDict;

    void Awake()
    {
        backRenderer = transform.Find("back").GetComponent<MeshRenderer>();
//        imageRenderer = transform.Find("image").GetComponent<MeshRenderer>();
        colourObjectDict = new Dictionary<ColourType, Material[]>();
        for (int i = 0; i < colourObjects.Length; i++)
        {
            if (!colourObjectDict.ContainsKey(colourObjects[i].colour))
            {
                Material[] materials = { colourObjects[i].backMaterial };
                colourObjectDict.Add(colourObjects[i].colour, materials);
            }
        }
    }


    public void SetColour(ColourType newColour) {
        colour = newColour;
        if (colourObjectDict.ContainsKey(newColour)) {
            backRenderer.material = colourObjectDict[newColour][0];
            // imageRenderer.material = colourObjectDict[newColour][1];
        }
    }
}
