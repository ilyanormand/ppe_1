using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyPieceSound;

    public void playDestroyNoise() 
    {
        if (PlayerPrefs.HasKey("Sound")) // проверка на наличие ключа sound
        {
            if (PlayerPrefs.GetInt("Sound") == 1) // проверка на то что значение ключа равно 1
            {
                //choose a random number
                int clipToPlay = Random.Range(0, destroyPieceSound.Length);
                // play that clip
                destroyPieceSound[clipToPlay].Play();
            }
        }
        else 
        {
            //choose a random number
            int clipToPlay = Random.Range(0, destroyPieceSound.Length);
            // play that clip
            destroyPieceSound[clipToPlay].Play();
        }
        
    }
}
