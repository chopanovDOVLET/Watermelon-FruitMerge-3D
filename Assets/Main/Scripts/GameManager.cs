using System;
using CrazyGames;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    List<DragonData> drData = new List<DragonData>();
    List<Dragon> dr = new List<Dragon>();
    Transform drParent;
    
    public bool isGamePlayOn = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        drData = MoveController.Instance.dragonData;
        drParent = MoveController.Instance.dragonsParent;
        GetDr();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("Size", 0);
        PlayerPrefs.SetInt("CurrentDragon", 0);
        SaveNubers();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GetDr()
    {
        int Size = PlayerPrefs.GetInt("Size", 0);
      
        Vector3 InsPos = Vector3.zero;
        Quaternion InsRot = Quaternion.Euler(Vector3.zero);

        for (int i = 0; i < Size; i++)
        {
            InsPos.x = PlayerPrefs.GetFloat("PosX" + i);
            InsPos.y = PlayerPrefs.GetFloat("PosY" + i);
            InsPos.z = PlayerPrefs.GetFloat("PosZ" + i);

            InsRot.x = PlayerPrefs.GetFloat("RotX" + i);
            InsRot.y = PlayerPrefs.GetFloat("RotY" + i);
            InsRot.z = PlayerPrefs.GetFloat("RotZ" + i);

            Dragon newDr = Instantiate(drData[PlayerPrefs.GetInt("dr" + i)].DragonPrefap, InsPos, InsRot, drParent).GetComponent<Dragon>();
            dr.Add(newDr);
            newDr.id = MoveController.Instance.dragonData[PlayerPrefs.GetInt("dr" + i)].id;
            newDr.hashId = MoveController.Instance.hashId;
            MoveController.Instance.hashId++;
            newDr.mesh.material = DragonSpawner.Instance.drHighF;
        }

        for (int i = 0; i < dr.Count; i++)
        {
            dr[i].drRigidbody.isKinematic = false; 
        }
    }

    void SaveNubers()
    {
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("HighScore", ScoreCounter.Instance.highscore);

        PlayerPrefs.SetInt("Bomb", SpecialDragons.Instance.bombSize);
        PlayerPrefs.SetInt("Unicorn", SpecialDragons.Instance.unicornSize);

        if (DragonIndicator.Instance.Sp)
            PlayerPrefs.SetInt("Sp", 1);
        else
            PlayerPrefs.SetInt("Sp", 0);
    }

    void Save() 
    {
        dr = new List<Dragon>();
        PlayerPrefs.SetInt("Score", ScoreCounter.Instance.score); 
        PlayerPrefs.SetInt("HighScore", ScoreCounter.Instance.highscore);
     
        PlayerPrefs.SetInt("Bomb", SpecialDragons.Instance.bombSize);
        PlayerPrefs.SetInt("Unicorn", SpecialDragons.Instance.unicornSize);
        PlayerPrefs.SetInt("CurrentDragon", MoveController.Instance.currentdr);

        if (DragonIndicator.Instance.Sp)
            PlayerPrefs.SetInt("Sp", 1);
        else
            PlayerPrefs.SetInt("Sp", 0);

        drParent = MoveController.Instance.dragonsParent;
        if (drParent.childCount != 0)
        {
            for (int i = 0; i < drParent.childCount; i++)
            {
                dr.Add(drParent.GetChild(i).GetComponent<Dragon>());
            }

            PlayerPrefs.SetInt("Size", dr.Count);

            for (int i = 0; i < dr.Count; i++)
            {
                PlayerPrefs.SetInt("dr" + i, dr[i].id);

                PlayerPrefs.SetFloat("PosX" + i, dr[i].transform.position.x);
                PlayerPrefs.SetFloat("PosY" + i, dr[i].transform.position.y);
                PlayerPrefs.SetFloat("PosZ" + i, dr[i].transform.position.z);

                PlayerPrefs.SetFloat("RotX" + i, dr[i].transform.rotation.x);
                PlayerPrefs.SetFloat("RotY" + i, dr[i].transform.rotation.y);
                PlayerPrefs.SetFloat("RotZ" + i, dr[i].transform.rotation.z);
            }
        }
    }
    
    public void StartGamePlayEventCrazy()
    {
        if (!isGamePlayOn)
        {
            isGamePlayOn = true;
            CrazyGames.CrazyEvents.Instance.GameplayStart();
        }
    }
    public void StopGamePlayEventCrazy()
    {
        if (isGamePlayOn)
        {
            isGamePlayOn = false;
            CrazyGames.CrazyEvents.Instance.GameplayStop();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if(ScoreCounter.Instance.highscore != 0)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}