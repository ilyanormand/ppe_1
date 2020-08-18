﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private HintManager hintManager;
    public bool tutorialShow;
    public GameObject BackgroundTutorial;
    public Dot dot;
    public GameObject FingerPrefab;
    void Start()
    {
        hintManager = FindObjectOfType<HintManager>();
        dot = FindObjectOfType<Dot>();
        tutorialShow = true;
        dot.notTutorial = false;
    }

    void Update()
    {
        if (tutorialShow == true)
        {
            changeElementLayer();
        }
        else if (tutorialShow == false)
        {
            GameObject finger = GameObject.Find("FingerTutorial");
            BackgroundTutorial.SetActive(false);
            Destroy(finger);
        }

    }
    
    void changeElementLayer() 
    {
        if (hintManager.tutorialElements != null)
        {
            foreach (GameObject i in hintManager.tutorialElements)
            {
                GameObject child = i.transform.GetChild(0).gameObject;
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = 6;
            }
        }
        else 
        {
            Debug.Log("fruits == null");
        }
        
    }
}
