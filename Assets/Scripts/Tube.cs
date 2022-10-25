using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour
{
    [SerializeField]
    GameObject bottomPrefab;

    [SerializeField]
    GameObject topPrefab;

    public void CreateTube(Vector2 newSize, Vector2 newPosition, bool topDirection = true)
    {
        Vector2 roofSize = new Vector2(newSize.x, 50);
        Vector2 tubeSize = new Vector2(newSize.x, newSize.y - 50);
        Vector2 roofPosition, tubePosition;

        if (topDirection)
        {
            roofPosition = new Vector2(newPosition.x, newPosition.y + tubeSize.y);
            tubePosition = new Vector2(newPosition.x, newPosition.y);
        } 
        else
        {
            roofPosition = new Vector2(newPosition.x, newPosition.y);
            tubePosition = new Vector2(newPosition.x, newPosition.y + roofSize.y);
        }

        GameObject tubePartGo = Instantiate(bottomPrefab, tubePosition, Quaternion.identity);
        tubePartGo.transform.SetParent(transform);
        TubePart tubePart = tubePartGo.GetComponent<TubePart>();
        tubePart.CreateTubePart(tubeSize, topDirection);

        GameObject roofPartGo = Instantiate(topPrefab, roofPosition, Quaternion.identity);
        roofPartGo.transform.SetParent(transform);
        TubePart roofPart = roofPartGo.GetComponent<TubePart>();
        roofPart.CreateTubePart(roofSize, topDirection);
    }
}
