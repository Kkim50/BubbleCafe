using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
    public SaveData saveData;
    private string savePath;

    void Start()
    {
        savePath = Application.persistentDataPath + "/playerdata.save";
        if (File.Exists(savePath)) {
            LoadGame(savePath);
            Debug.Log("Loaded data!");
        } else {
            Debug.Log("Save data not found!");
            saveData = CreateNewSaveData();
        }
    }

    void Update() {
        if (Input.GetKeyDown("s")) {
            SaveGame(savePath);
            Debug.Log("Saved data to file.");
        }
    }

    private SaveData CreateNewSaveData() {
        SaveData save = new SaveData();
        save.availableSizes.Add("m");
        save.availableTeas.Add("black");
        return save;
    }

    public void SaveGame(string path) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        bf.Serialize(file, saveData);
        file.Close();
    }

    public void LoadGame(string path) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);
        saveData = (SaveData) bf.Deserialize(file);
        file.Close();
    }
}
