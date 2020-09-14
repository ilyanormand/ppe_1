using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;

    void Awake()
    {
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject); // Для того чтобы обьект не уничтожался при переходах в другую сцену
            gameData = this;
        }
        else 
        {
            Destroy(this.gameObject);
        }
        load();
    }

    void Start()
    {
        
    }

    public void Save() 
    {
        // create a binary formatter which can read binary files
        BinaryFormatter formatter = new BinaryFormatter();
        // create a route from the programm to the file
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);
        // create a copy save data
        SaveData data = new SaveData();
        data = saveData;
        //Actually save the data into the file in the binary format
        formatter.Serialize(file, data);
        //close the data stream
        file.Close();
        Debug.Log(data);
    }

    public void load() 
    {
        // Check if the save game file exist
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            // create a binary formatter
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
            Debug.Log("saveData");
        }
        else 
        {
            saveData = new SaveData();
            saveData.isActive = new bool[100];
            saveData.stars = new int[100];
            saveData.highScores = new int[100];
            saveData.isActive[0] = true;
        }
    }
    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnDisable()
    {
        Save();
    }

    void Update()
    {

    }
}
