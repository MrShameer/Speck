using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject LoginPanel;
    public GameObject MainPanel;

    public GameObject SimulationInformation;
    GameObject info;
    public GameObject Text;

    [System.Serializable]
    public class SimulationInfo
    {
        [JsonProperty("name")]
        public string Simulation_Name;
        [JsonProperty("total_npc")]
        public string Total_Npc;
        [JsonProperty("total_infected")]
        public string Total_Infected_Npc;
        [JsonProperty("duration")]
        public string Simulation_Duration;
        [JsonProperty("with_mask")]
        public string Npc_With_Mask;
        [JsonProperty("created")]
        public string Simulation_Created_At;
        [JsonProperty("updated")]
        public string Simulation_Updated_At;
    }

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
        MainPanel.SetActive(false);
        info = SimulationInformation.transform.Find("InfoPanel/Infos").gameObject;

        StartCoroutine(GetText());
    }

    IEnumerator GetText() {
        WWWForm form = new WWWForm();
        form.AddField("name", "Train 1");
        UnityWebRequest www = UnityWebRequest.Post("http://speck-api.test/api/info",form);
        www.SetRequestHeader("Authorization", "Bearer pXlrzYViE9rBIEYVl9TliWuGScXjpjWsztLCu0FI");
        yield return www.Send();
        SimulationInfo data =JsonConvert.DeserializeObject<SimulationInfo>(www.downloadHandler.text);
        Debug.Log(www.downloadHandler.text);

        if(!www.isNetworkError) {
            Debug.Log(data.Simulation_Name);
            foreach(var property in data.GetType().GetFields()) 
            {
                GameObject InfoText = Instantiate(Text);
                InfoText.transform.SetParent(info.transform);
                InfoText.GetComponent<Text>().text = property.Name.Replace('_',' ') + " : " + property.GetValue(data);
            }

            }   
        }

    void Update()
    {
        
    }

    public void ShowMainPanel()
    {
        LoginPanel.SetActive(false);
        MainPanel.SetActive(true);
    }
}
