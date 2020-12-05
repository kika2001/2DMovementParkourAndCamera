using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject loadingScreen;
    //public Camera loadingCam;
    public Slider bar;
    private void Awake()
    {
        instance = this;
        SceneManager.LoadSceneAsync((int) SceneIndexes.TITLE_SCREEN, LoadSceneMode.Single);
        DontDestroyOnLoad(this);
    }

    List<AsyncOperation> scenesLoading= new List<AsyncOperation>();

    public void LoadLevel(string sceneName,SceneIndexes sceneindex)
    {
        loadingScreen.gameObject.SetActive(true);
        //loadingCam.gameObject.SetActive(true);
        //scenesLoading.Add(SceneManager.UnloadSceneAsync((int) SceneIndexes.TITLE_SCREEN));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int) sceneindex, LoadSceneMode.Single));
        
        StartCoroutine(GetSceneLoadProgress(sceneName));
    }
    /*
    public void LoadGame()
    {
        loadingScreen.gameObject.SetActive(true);
        //loadingCam.gameObject.SetActive(true);
        //scenesLoading.Add(SceneManager.UnloadSceneAsync((int) SceneIndexes.TITLE_SCREEN));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int) SceneIndexes.GAME, LoadSceneMode.Single));
        
        StartCoroutine(GetSceneLoadProgress("Game"));
    }
    public void Tutorial()
    {
        loadingScreen.gameObject.SetActive(true);
        //loadingCam.gameObject.SetActive(true);
        //scenesLoading.Add(SceneManager.UnloadSceneAsync((int) SceneIndexes.TITLE_SCREEN));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int) SceneIndexes.TUTORIAL, LoadSceneMode.Single));
        
        StartCoroutine(GetSceneLoadProgress("Tutorial"));
    }
    */

    private float totalSceneProgress;

    public IEnumerator GetSceneLoadProgress(string name)
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;
                foreach (AsyncOperation operation in scenesLoading)
                {
                    totalSceneProgress += operation.progress;
                }

                totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100f;
                bar.value = Mathf.RoundToInt(totalSceneProgress);
                yield return null;
            }
            
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
        
        loadingScreen.gameObject.SetActive(false);
        //loadingCam.gameObject.SetActive(false);
    }
}