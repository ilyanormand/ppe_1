using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    // Start is called before the first frame update
    float index; 
    ParticleSystem ps;
    public GameObject attached;
    void Start()
    {

    }
    public void SetColor(int index_) 
    {
        ps = GetComponent<ParticleSystem>();
        index = index_ + 1;
        var textSheet = ps.textureSheetAnimation;
        textSheet.startFrame = index / 6f;
        ps.Play();
    }
    // Update is called once per frame
    void Update()
    {
        if (attached != null) 
        {
            transform.position = attached.transform.position; // set the effect in the same position as the gameobject
        } 
    }
}
