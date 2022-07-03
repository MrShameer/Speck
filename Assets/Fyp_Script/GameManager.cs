using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string insertInfoURL;
    // "http://speck-api.test/api/insertInfo"

    [Header("Train Setting")]
    public GameObject trainPrefab;
    public Transform trainSpawner;
    public Transform trainDestination;
    public Transform trainDestroy;
    // public float TrainWaitTime;
    public Slider TrainWaitTime;
    GameObject train;
    Transform target;

    [Header("Train Boarding")]
    public Transform trainBoarding;
    
    [HideInInspector]
    public bool available = false;


    
    [System.Serializable]
    public class SpawnState {
        public Transform Location;
        public bool Burst;
    }
    [Header("Spawner Setting")]
    public List<SpawnState> spawning;
    public List<GameObject> Ai;

    public GameObject system;

    public GameObject menu;
    public InputField mapName;
    public Slider npcCount;
    public Slider npcSpawnInterval;
    public Slider totalFaceMask;
    InputField Duration;
    public Text validate;

    [Header("Options")]
    public GameObject optionPanel;
    public Button pauseButton;
    public Button playButton;
    public Button stopButton;

    [Header("Simulation Information")]
    public GameObject SimulationInformation;
    GameObject info;
    public GameObject Text;


    [Header("Alert")]
    public GameObject alert;
    public Text alertMessage;
    // TextMesh alertTitle;


    [System.Serializable]
    class SimsInfo
    {
        [JsonProperty("name")]
        public string Simulation_Name;
        [JsonProperty("total_npc")]
        public string Total_Npc;
        [JsonProperty("npc_spawn_interval")]
        public string Npc_Spawn_Interval;
        [JsonProperty("duration")]
        public string Simulation_Duration;
        [JsonProperty("with_mask")]
        public string Npc_With_Mask;
    }
    
    [System.Serializable]
    class SimInsert
    {
        public string message;
        public bool insert;
    }


    SimsInfo simsInfo = new SimsInfo();

    DateTime startTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    void Start()
    {
        Time.timeScale = 1;
        playButton.interactable = false;
        stopButton.interactable = false;
        optionPanel.SetActive(false);
        menu.SetActive(true);
        SimulationInformation.SetActive(false);
        info = SimulationInformation.transform.Find("InfoPanel/Infos").gameObject;
        alert.SetActive(false);
        validate.text = "";

        // alertTitle = alert.transform.Find("Title").GetComponent<TextMesh>();
    }

    public void pause(){
        pauseButton.interactable = false;
        playButton.interactable = true;
        stopButton.interactable = true;
        Time.timeScale = 0;
    }

    public void play(){
        pauseButton.interactable = true;
        playButton.interactable = false;
        stopButton.interactable = false;
        Time.timeScale = 1;
    }

    public void stop(){
        // TimeSpan final = DateTime.Now - startTime;
        // simsInfo.Simulation_Duration = (final.Hours+"(H) / "+final.Minutes+"(M) / "+final.Seconds+"(S)");
        simsInfo.Simulation_Duration = (DateTime.Now - startTime).ToString("hh\\:mm\\:ss");
        foreach(var property in simsInfo.GetType().GetFields()) 
        {
            GameObject InfoText = Instantiate(Text);
            InfoText.transform.SetParent(info.transform,false);
            InfoText.GetComponent<Text>().text = property.Name.Replace('_',' ') + " : " + property.GetValue(simsInfo);
        }
        SimulationInformation.SetActive(true);
        playButton.interactable = false;
        stopButton.interactable = false;
    }

    public void save(){
        StartCoroutine(sendInfo());
    }

    public void startSpawn(){
        if(string.IsNullOrWhiteSpace(mapName.text)){
            validate.text = "Nama Simulasi Diperlukan";
            Debug.Log("Map name is empty");
            return;
        }
        startTime = DateTime.Now;
        StartCoroutine(spawn());
    }

    public IEnumerator spawn(){
        InvokeRepeating("repeat", 1, TrainWaitTime.value*3);
        simsInfo.Simulation_Name = mapName.text;
        simsInfo.Total_Npc = npcCount.value.ToString();
        simsInfo.Npc_Spawn_Interval = npcSpawnInterval.value.ToString();
        simsInfo.Npc_With_Mask = totalFaceMask.value.ToString();
        int npcArea = int.Parse(simsInfo.Total_Npc)/spawning.Count;
        menu.SetActive(false);
        optionPanel.SetActive(true);
        foreach (SpawnState spawn in spawning)
        {
            GameObject newAI=null;
            if(Ai.Count > npcArea){
                for (int i = 0; i < npcArea; i++)
                {
                    newAI = Instantiate(Ai[i], spawn.Location.position, Quaternion.identity);
                    yield return new WaitForSeconds(npcSpawnInterval.value);
                }
            }
            else{
                int mod = npcArea % Ai.Count;
                int div = npcArea / Ai.Count;

                for (int i = 0; i < mod; i++)
                {
                    for (int j = 0; j < div+1; j++)
                    {
                        newAI = Instantiate(Ai[i], spawn.Location.position, Quaternion.identity);
                        yield return new WaitForSeconds(npcSpawnInterval.value);
                    }
                }

                for (int i = mod; i < Ai.Count; i++)
                {
                    for (int j = 0; j < div; j++)
                    {
                        newAI = Instantiate(Ai[i], spawn.Location.position, Quaternion.identity);
                        yield return new WaitForSeconds(npcSpawnInterval.value);
                    }
                }
            }
        }
    }

    void Update()
    {
        if(train){
            train.transform.position = Vector3.MoveTowards(train.transform.position, target.position,  Time.deltaTime * 10);
            if(Vector3.Distance(trainDestroy.position, train.transform.position) < 0.1f){
                Destroy(train);
            }else if(Vector3.Distance(trainDestination.position, train.transform.position) < 0.1f){
                available = true;
                StartCoroutine(timer());
            }
        }
    }

    void repeat(){
        train = Instantiate(trainPrefab,trainSpawner.position, Quaternion.Euler(0,0,0)) as GameObject;
        target = trainDestination;
    }

    IEnumerator timer(){
        yield return new WaitForSecondsRealtime(TrainWaitTime.value);
        target = trainDestroy;
        available = false;
    }

    public void returnScene(){
        SceneManager.LoadScene("MainUI");
    }


    IEnumerator sendInfo() {
        WWWForm form = new WWWForm();
        form.AddField("name", simsInfo.Simulation_Name);
        form.AddField("total_npc", simsInfo.Total_Npc);
        form.AddField("npc_spawn_interval", simsInfo.Npc_Spawn_Interval);
        form.AddField("duration", simsInfo.Simulation_Duration);
        form.AddField("with_mask", simsInfo.Npc_With_Mask);
        UnityWebRequest www = UnityWebRequest.Post(insertInfoURL,form);
        www.SetRequestHeader("Authorization", "Bearer "+ PlayerPrefs.GetString("Token"));
        www.SetRequestHeader("Accept", "application/json");
        yield return www.Send();
        SimInsert data =JsonConvert.DeserializeObject<SimInsert>(www.downloadHandler.text);
        if(!www.isNetworkError) {
            alert.SetActive(true);
            
            alertMessage.text = data.message;
            StartCoroutine(fadeOut());
            if(data.insert){
                SimulationInformation.transform.Find("SaveButton").GetComponent<Button>().interactable = false;
            }else{
                //UNHIDE SAVE BUTTON
            }
        }
    }

    IEnumerator fadeOut(){
        yield return new WaitForSecondsRealtime(2);
        alert.SetActive(false);
    }
}
