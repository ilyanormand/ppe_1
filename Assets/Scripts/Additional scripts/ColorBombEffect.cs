using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBombEffect : MonoBehaviour
{
    public GameObject colorBombEffect;
    public float smoothing = .5f;
    // Start is called before the first frame update
    public void StartEffect(List<GameObject> matches, Vector3 positionOfCreation) 
    {
        /*for (int i = 0; i < matches.Count; i++)
        {
            Debug.Log("Instantiate effect color bomb");
            GameObject effect = Instantiate(colorBombEffect, positionOfCreation, Quaternion.identity);
            Debug.Log("effect = " + effect);
            Vector2 positionOfMatch = new Vector2(matches[i].transform.position.x, matches[i].transform.position.y);
            Debug.Log("positionOfMatch = " + positionOfMatch);
            effect.transform.position = Vector2.Lerp(effect.transform.position, positionOfMatch, smoothing * Time.deltaTime);
            Debug.Log("startPosition = " + effect.transform);
        }*/
        Debug.Log(matches.Count);

        foreach (GameObject i in matches) 
        {
            Debug.Log("Instantiate effect color bomb");
            GameObject effect = Instantiate(colorBombEffect, positionOfCreation, Quaternion.identity);
            GameObject effect2 = Instantiate(colorBombEffect, positionOfCreation, Quaternion.identity);
            Debug.Log("effect = " + effect);
            Vector2 positionOfMatch = new Vector2(i.transform.position.x, i.transform.position.y);
            Debug.Log("positionOfMatch = " + positionOfMatch);
            //effect.transform.position = Vector2.Lerp(effect.transform.position, positionOfMatch, smoothing * Time.deltaTime);
            effect.transform.position = new Vector2(positionOfMatch.x, positionOfMatch.y);
            effect2.transform.position = new Vector2(positionOfMatch.x, positionOfMatch.y);
            Debug.Log("startPosition = " + effect.transform);
        }
    }
}
