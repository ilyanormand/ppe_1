using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBombEffect : MonoBehaviour
{
    public GameObject colorBombEffect;
    public float smoothing = .5f;
    public SoundManager soundManager;
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

        StartCoroutine(GeneratingParticles(matches, positionOfCreation));
    }

    public IEnumerator GeneratingParticles(List<GameObject> ListOfMatches, Vector3 positionToGenerate) 
    { 
        foreach (GameObject i in ListOfMatches)
        {
            Debug.Log("Instantiate effect color bomb");
            GameObject effect = Instantiate(colorBombEffect, positionToGenerate, Quaternion.identity);
            soundManager.playColorSound();
            Debug.Log("effect = " + effect);
            Vector2 positionOfMatch = new Vector2(i.transform.position.x, i.transform.position.y);
            Debug.Log("positionOfMatch = " + positionOfMatch);
            //effect.transform.position = Vector2.Lerp(effect.transform.position, positionOfMatch, smoothing * Time.deltaTime);
            effect.transform.position = new Vector2(positionOfMatch.x, positionOfMatch.y);
            Debug.Log("startPosition = " + effect.transform);
            float randomTime = Random.Range(0.05f, 0.1f);
            yield return new WaitForSeconds(randomTime);
            Destroy(effect, 1f);
        }
        
    }
}
