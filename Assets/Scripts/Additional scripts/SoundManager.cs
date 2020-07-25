using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyPieceSound;

    public void playDestroyNoise() 
    {
        //choose a random number
        int clipToPlay = Random.Range(0, destroyPieceSound.Length);
        // play that clip
        destroyPieceSound[clipToPlay].Play();
    }
}
