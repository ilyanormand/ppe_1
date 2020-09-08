using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyPieceSound;
    public AudioSource stripeSoundRow;
    public AudioSource stripeSoundColumn;
    public AudioSource backcgroundMusic;
    public AudioSource adjacentSound;
    public AudioSource ColorBombSound;
    //public AudioSource LightSound;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                backcgroundMusic.Play();
                backcgroundMusic.volume = 0;
            }
            else
            {
                backcgroundMusic.Play();
                backcgroundMusic.volume = 1;
            }
        }
        else
        {
            backcgroundMusic.Play();
            backcgroundMusic.volume = 0;
        }
    }

    public void adjustVolume()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            Debug.Log(PlayerPrefs.GetInt("Sound"));
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                //Debug.Log(backcgroundMusic.volume);
                backcgroundMusic.volume = 0;
                //Debug.Log(backcgroundMusic.volume);
            }
            else
            {
                backcgroundMusic.volume = 1;
            }
        }
    }
    public void playDestroyNoise() 
    {

        /*if (PlayerPrefs.HasKey("Sound")) // проверка на наличие ключа sound
        {
            if (PlayerPrefs.GetInt("Sound") == 1) // проверка на то что значение ключа равно 1
            {
                //choose a random number
                int clipToPlay = Random.Range(0, destroyPieceSound.Length);
                // play that clip
                destroyPieceSound[clipToPlay].Play();
            }
        }*/
        /*else 
        {
            //choose a random number
            int clipToPlay = Random.Range(0, destroyPieceSound.Length);
            // play that clip
            destroyPieceSound[clipToPlay].Play();
        }*/

        //choose a random number
        int clipToPlay = Random.Range(0, destroyPieceSound.Length);
        // play that clip
        destroyPieceSound[clipToPlay].Play();
    }

    public void playStripeSound(string type) 
    {
        if (type == "row" || type == "Row")
        {
            stripeSoundRow.Play();
        }
        else if (type == "Column" || type == "column") 
        {
            stripeSoundColumn.Play();
        }

    }

    public void playAdjacentSound() 
    {
        adjacentSound.Play();
    }

    public void playColorSound() 
    {
        ColorBombSound.Play();
    }

    /*public void playLightSound()
    {
        LightSound.Play();
    }*/
}
