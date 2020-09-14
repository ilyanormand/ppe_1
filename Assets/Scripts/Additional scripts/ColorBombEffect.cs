using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBombEffect : MonoBehaviour
{
    public GameObject colorBombEffect;
    public LightningBoltScript lightEffectPrefab;
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
        //Debug.Log(matches.Count);

        StartCoroutine(GeneratingParticles(matches, positionOfCreation));
    }

    public IEnumerator GeneratingParticles(List<GameObject> ListOfMatches, Vector3 positionToGenerate) 
    {
        
        foreach (GameObject i in ListOfMatches)
        {

            if (i != null) 
            {
                //Debug.Log("Instantiate effect color bomb");
                LightningBoltScript lightEffect = Instantiate(lightEffectPrefab, positionToGenerate, Quaternion.identity);
                //Debug.Log("lightEffect = " + lightEffect);
                lightEffect.transform.parent = gameObject.transform;
                lightEffect.StartPosition = positionToGenerate;
                //Debug.Log("StartPosition = " + lightEffect.StartPosition);
                //Debug.Log("Position to generate = " + positionToGenerate);
                Vector3 positionOfMatch = new Vector3(i.transform.position.x, i.transform.position.y, 0);
                lightEffect.EndPosition = positionOfMatch;
                //soundManager.playLightSound();
                //Debug.Log("EndPosition = " + lightEffect.EndPosition);
                //Debug.Log("Position of match = " + positionOfMatch);
                yield return new WaitForSeconds(0.03f);
                Destroy(lightEffect.gameObject);
                GameObject effect = Instantiate(colorBombEffect, positionToGenerate, Quaternion.identity);
                soundManager.playColorSound();
                effect.transform.position = new Vector2(positionOfMatch.x, positionOfMatch.y);
                float randomTime = Random.Range(0.03f, 0.07f);
                yield return new WaitForSeconds(randomTime);
                Destroy(effect, 1f);
            }
            
        }
        
    }
}
