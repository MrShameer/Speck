using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public string baseURL;
    public string fetchLoginURL;
    public string fetchValidateURL;
    public string fetchInfoURL;
    public string fetchSimsURL;
    public string deleteSimURL;


    public GameObject Menu;
    public GameObject LoginPanel;
    public GameObject MainPanel;
    public GameObject Simulation;
    public GameObject SimulationInformation;
    GameObject info;
    public GameObject Text;
    public GameObject simulationBlock;
    GameObject simulationPanel;
    public InputField email;
    public InputField password;
    public Text validate;
    public GameObject alert;

    bool panelOpen = false;

    [System.Serializable]
    public class Sims
    {
        public SimulationInfo[] sims;
    }

    [System.Serializable]
    public class SimulationInfo
    {
        [JsonProperty("id")]
        public string Simulation_Id;
        [JsonProperty("name")]
        public string Simulation_Name;
        [JsonProperty("total_npc")]
        public string Total_Npc;
        [JsonProperty("duration")]
        public TimeSpan Simulation_Duration;
        [JsonProperty("with_mask")]
        public string Npc_With_Mask;
        [JsonProperty("created_at")]
        public DateTime Simulation_Created_At;
        [JsonProperty("updated_at")]
        public DateTime Simulation_Updated_At;
    }

    [System.Serializable]
    public class Log
    {
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("token")]
        public string Token;
    }

    [System.Serializable]
    public class Delete
    {
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("delete")]
        public bool delete;
    }

    string[] simulationNames = {"Train Station"};

    SimulationInfo simInfo;

    private void Awake()
    {
        if (instance == null){
            instance = this;
        }
        else if (instance != this){
            Destroy(this);
        }
        StartCoroutine(fetchValidate());
    }


    void Start()
    {
        info = SimulationInformation.transform.Find("InfoPanel/Infos").gameObject;
        simulationPanel = Simulation.transform.Find("SimPanel/Sims").gameObject;
        alert.SetActive(false);
    }

    public void quit(){
        Application.Quit();
    }

    public void Login()
    {
        if(string.IsNullOrWhiteSpace(email.text) || string.IsNullOrWhiteSpace(password.text)){
            validate.text = "Sila Isi Semua Ruangan";
            return;
        }
        StartCoroutine(fetchLogin(email.text, password.text));
    }

    public void LogOut(){
        PlayerPrefs.DeleteKey("Token");
        LoginPanel.SetActive(true);
        MainPanel.SetActive(false);  
    }

    public void newSimulation(){
        Menu.SetActive(false);
        Simulation.SetActive(true);

        foreach (Transform child in simulationPanel.transform) {
            Destroy(child.gameObject);
        }

        foreach(var i in simulationNames){
            GameObject SimPanel = Instantiate(simulationBlock);
            SimPanel.transform.SetParent(simulationPanel.transform,false);
            //UBAH GAMBAR SEKALI
            SimPanel.transform.Find("Text").gameObject.GetComponent<Text>().text = i;
            SimPanel.GetComponent<Button>().onClick.AddListener(delegate{openScene(i);});
        }
    }

    public void previousSimulation(){
        Menu.SetActive(false);
        Simulation.SetActive(true);
        StartCoroutine(fetchSims());
    }

    public void back(){
        Menu.SetActive(true);
        Simulation.SetActive(false);
        alert.SetActive(false);
        foreach (Transform child in simulationPanel.transform) {
            Destroy(child.gameObject);
        }
        closeInfo();
    }

    public void closeInfo(){
        foreach (Transform child in info.transform) {
            Destroy(child.gameObject);
        }
        SimulationInformation.SetActive(false);
        alert.SetActive(false);
        panelOpen = false;
    }

    public void openAlert(){
        alert.SetActive(true);
    }

    public void closeAlert(){
        alert.SetActive(false);
    }


    IEnumerator fetchLogin(string email, string password){
        Log data = new Log();
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        UnityWebRequest www = UnityWebRequest.Post(baseURL+fetchLoginURL,form);
        // http://speck-api.test/api/login
        yield return www.Send();
        
        data =JsonConvert.DeserializeObject<Log>(www.downloadHandler.text);
        // data = JsonUtility.FromJson<Log>(www.downloadHandler.text);

        if(!www.isNetworkError) {
            if(data.Token != null){
                PlayerPrefs.SetString("Token", data.Token);
                LoginPanel.SetActive(false);
                MainPanel.SetActive(true);  
            }else if(data.Message != null){
                validate.text = data.Message;
            }
        }
    }


    IEnumerator fetchValidate(){
        Log data = new Log();
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(baseURL+fetchValidateURL,form);
        // http://speck-api.test/api/user
        www.SetRequestHeader("Authorization","Bearer "+ PlayerPrefs.GetString("Token"));
        www.SetRequestHeader("Accept", "application/json");
        yield return www.Send();
        data =JsonConvert.DeserializeObject<Log>(www.downloadHandler.text);
        if(data.Message != null){
            if(data.Message == "Authenticated"){
                LoginPanel.SetActive(false);
                MainPanel.SetActive(true);
            }
        }
    }

    IEnumerator fetchInfo(string name, string id) {
        if(!panelOpen){
            WWWForm form = new WWWForm();
            form.AddField("id", id);
            UnityWebRequest www = UnityWebRequest.Post(baseURL+fetchInfoURL,form);
            // http://speck-api.test/api/info
            www.SetRequestHeader("Authorization", "Bearer "+ PlayerPrefs.GetString("Token"));
            www.SetRequestHeader("Accept", "application/json");
            yield return www.Send();
            // SimulationInfo data =JsonConvert.DeserializeObject<SimulationInfo>(www.downloadHandler.text);
            simInfo =JsonConvert.DeserializeObject<SimulationInfo>(www.downloadHandler.text);

            if(!www.isNetworkError) {
                SimulationInformation.SetActive(true);
                foreach(var property in simInfo.GetType().GetFields()) 
                {
                    if(property.Name != "Simulation_Id"){
                        GameObject InfoText = Instantiate(Text);
                        InfoText.transform.SetParent(info.transform,false);
                        InfoText.GetComponent<Text>().text = property.Name.Replace('_',' ') + " : " + property.GetValue(simInfo);
                    }
                    
                }
            }
            panelOpen = true;
        }
    }

    IEnumerator fetchSims(){
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(baseURL+fetchSimsURL,form);
        // http://speck-api.test/api/list
        www.SetRequestHeader("Authorization", "Bearer "+ PlayerPrefs.GetString("Token"));
        www.SetRequestHeader("Accept", "application/json");
        yield return www.Send();
        Sims data = JsonConvert.DeserializeObject<Sims>(www.downloadHandler.text);
        if(!www.isNetworkError) {
            foreach(var i in data.sims){
                GameObject SimPanel = Instantiate(simulationBlock);
                SimPanel.name = i.Simulation_Id;
                SimPanel.transform.SetParent(simulationPanel.transform,false);
                //UBAH GAMBAR SEKALI
                SimPanel.transform.Find("Text").gameObject.GetComponent<Text>().text = i.Simulation_Name;
                SimPanel.GetComponent<Button>().onClick.AddListener(delegate{StartCoroutine(fetchInfo(i.Simulation_Name,i.Simulation_Id));});
            }
        }
    }

    void openScene(string name){
        SceneManager.LoadScene(name);
    }

    public void deleteSim(){
        alert.SetActive(false);
        StartCoroutine(fetchDelete());
    }

    IEnumerator fetchDelete(){
        WWWForm form = new WWWForm();
        form.AddField("id", simInfo.Simulation_Id);
        UnityWebRequest www = UnityWebRequest.Post(baseURL+deleteSimURL,form);
        // http://speck-api.test/api/delete
        www.SetRequestHeader("Authorization", "Bearer "+ PlayerPrefs.GetString("Token"));
        www.SetRequestHeader("Accept", "application/json");
        yield return www.Send();
        Delete data =JsonConvert.DeserializeObject<Delete>(www.downloadHandler.text);
        if(!www.isNetworkError) {
            if(data.delete){
                closeInfo();
                Destroy(simulationPanel.transform.Find(simInfo.Simulation_Id).gameObject);
                
            }
        }
    }
}
