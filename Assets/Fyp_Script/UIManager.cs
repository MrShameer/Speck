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

    bool panelOpen = false;

    [System.Serializable]
    public class Sims
    {
        public SimulationInfo[] sims;
    }

    [System.Serializable]
    public class SimulationInfo
    {
        [JsonProperty("name")]
        public string Simulation_Name;
        [JsonProperty("total_npc")]
        public string Total_Npc;
        [JsonProperty("duration")]
        public TimeSpan Simulation_Duration;
        [JsonProperty("with_mask")]
        public string Npc_With_Mask;
        [JsonProperty("created_at")]
        public string Simulation_Created_At;
        [JsonProperty("updated_at")]
        public string Simulation_Updated_At;
    }

    [System.Serializable]
    public class Log
    {
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("token")]
        public string Token;
    }

    string[] simulationNames = {"Train Station"};

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
    }

    public void Login()
    {
        if(string.IsNullOrWhiteSpace(email.text) || string.IsNullOrWhiteSpace(password.text)){
            validate.text = "Please fill in all fields";
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
        panelOpen = false;
    }


    IEnumerator fetchLogin(string email, string password){
        Log data = new Log();
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        UnityWebRequest www = UnityWebRequest.Post("http://speck-api.test/api/login",form);
        yield return www.Send();
        
        data =JsonConvert.DeserializeObject<Log>(www.downloadHandler.text);

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
        UnityWebRequest www = UnityWebRequest.Post("http://speck-api.test/api/user",form);
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

    IEnumerator fetchInfo(string name) {
        if(!panelOpen){
            WWWForm form = new WWWForm();
            form.AddField("name", name);
            UnityWebRequest www = UnityWebRequest.Post("http://speck-api.test/api/info",form);
            www.SetRequestHeader("Authorization", "Bearer "+ PlayerPrefs.GetString("Token"));
            www.SetRequestHeader("Accept", "application/json");
            yield return www.Send();
            SimulationInfo data =JsonConvert.DeserializeObject<SimulationInfo>(www.downloadHandler.text);

            if(!www.isNetworkError) {
                SimulationInformation.SetActive(true);
                foreach(var property in data.GetType().GetFields()) 
                {
                    GameObject InfoText = Instantiate(Text);
                    InfoText.transform.SetParent(info.transform,false);
                    InfoText.GetComponent<Text>().text = property.Name.Replace('_',' ') + " : " + property.GetValue(data);
                }
            }
            panelOpen = true;
        }
    }

    IEnumerator fetchSims(){
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post("http://speck-api.test/api/list",form);
        www.SetRequestHeader("Authorization", "Bearer "+ PlayerPrefs.GetString("Token"));
        www.SetRequestHeader("Accept", "application/json");
        yield return www.Send();
        Sims data = JsonConvert.DeserializeObject<Sims>(www.downloadHandler.text);
        Debug.Log(www.downloadHandler.text);
        if(!www.isNetworkError) {
            foreach(var i in data.sims){
                GameObject SimPanel = Instantiate(simulationBlock);
                SimPanel.transform.SetParent(simulationPanel.transform,false);
                //UBAH GAMBAR SEKALI
                SimPanel.transform.Find("Text").gameObject.GetComponent<Text>().text = i.Simulation_Name;
                SimPanel.GetComponent<Button>().onClick.AddListener(delegate{StartCoroutine(fetchInfo(i.Simulation_Name));});
            }
        }
    }

    void openScene(string name){
        SceneManager.LoadScene(name);
    }
}
