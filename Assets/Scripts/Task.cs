using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour {
    private const string ip = "http://www.snowcheng.com/goplace/public";
    private const string showMethod = "/task/show";
    private const string startMethod = "/task/start";
    private const string promptMethod = "/task/prompt";
    public UniWebView uwv;
    public Text text;
    public Text TaskText1;
    public Text TaskText2;
    public Text TaskText3;

    public Text HiddenText1;
    public Text HiddenText2;
    public Text HiddenText3;

    public Text ButtonText1;
    public Text ButtonText2;
    public Text ButtonText3;
    private string latitude;
    private string longitude;
    private bool error;
    private string errormessage;

    // Use this for initialization
    void Start () {
        error = false;
        errormessage = "发生未知问题";
        latitude = null;
        longitude = null;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Post错误处理
    /// </summary>
    /// <param name="www">WWW类信息</param>
    /// <returns>bool值</returns>
    private bool wwwError(WWW www)
    {
        if (www.error != null)
        {
            Debug.Log(www.error);
            return false;
        }
        return true;
    }

    private void flushGpsInformation() {
        latitude = null;
        longitude = null;
    }

    private bool gpsError(bool _error, string _latitude, string _longitude) {
        if (!_error)
        {
            return false;
        }
        if (_latitude == null || _longitude == null)
        {
            return false;
        }
        return true;
    }

    public void showTask() {
        StartCoroutine(showTaskIEnumerator());
    }

    private IEnumerator showTaskIEnumerator() {
         if (!Input.location.isEnabledByUser)
         {
             error = false;
             errormessage = "位置服务不可用";
             TaskText1.text = errormessage;
             yield break;
         }

         try
         {
             Input.location.Start();
         }
         catch (Exception e)
         {
             TaskText1.text = e.ToString();
             yield break;
         }

         int maxWait = 20;
         while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
         {
             yield return new WaitForSeconds(1);
             maxWait--;
         }

         // 服务初始化超时  
         if (maxWait < 1)
         {
             error = false;
             errormessage = "服务初始化超时";
             TaskText1.text = errormessage;
             yield break;
         }

         // 连接失败  
         if (Input.location.status == LocationServiceStatus.Failed)
         {
             error = false;
             errormessage = "无法确定设备位置";
             TaskText1.text = errormessage;
             yield break;
         }
        latitude = Input.location.lastData.latitude.ToString();
        longitude = Input.location.lastData.longitude.ToString();
        error = true;

        Input.location.Stop();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("COOKIE"));
        string data = "latitude=" + latitude + "&longitude=" + longitude + "&_tokenpasswd=" + PlayerPrefs.GetString("_tokenpasswd");
        byte[] bs = System.Text.UTF8Encoding.UTF8.GetBytes(data);
        WWW www = new WWW(ip + showMethod, bs, headers);
        yield return www;

        if (wwwError(www))
        {
            Debug.Log(www.text);
            TaskAllInformation taskallinformation = LitJson.JsonMapper.ToObject<TaskAllInformation>(www.text);
            if (taskallinformation.status != "200")
            {
                Debug.Log(taskallinformation.status);
                yield return taskallinformation.status;
            }
            else
            {
                int count = taskallinformation.place_informations.Count;
                if (count >= 1)
                {
                    TaskText1.text = "任务：请到达 " + taskallinformation.place_informations[0].place.name + " 详细地址：" + taskallinformation.place_informations[0].place.address;
                    PlayerPrefs.SetString("task1_lat", taskallinformation.place_informations[0].place.location.lat.ToString());
                    PlayerPrefs.SetString("task1_lng", taskallinformation.place_informations[0].place.location.lng.ToString());
                    PlayerPrefs.SetString("task1_name", taskallinformation.place_informations[0].place.name);
                    PlayerPrefs.SetString("task1_address", taskallinformation.place_informations[0].place.address);
                    PlayerPrefs.SetString("task1_uid", taskallinformation.place_informations[0].place.uid);
                    HiddenText1.text = "task1";
                }
                if (count >= 2)
                {
                    TaskText2.text = "任务：请到达 " + taskallinformation.place_informations[1].place.name + " 详细地址：" + taskallinformation.place_informations[1].place.address;
                    PlayerPrefs.SetString("task2_lat", taskallinformation.place_informations[1].place.location.lat.ToString());
                    PlayerPrefs.SetString("task2_lng", taskallinformation.place_informations[1].place.location.lng.ToString());
                    PlayerPrefs.SetString("task2_name", taskallinformation.place_informations[1].place.name);
                    PlayerPrefs.SetString("task2_address", taskallinformation.place_informations[1].place.address);
                    PlayerPrefs.SetString("task2_uid", taskallinformation.place_informations[1].place.uid);
                    HiddenText2.text = "task2";
                }
                if (count >= 3)
                {
                    TaskText3.text = "任务：请到达 " + taskallinformation.place_informations[2].place.name + " 详细地址：" + taskallinformation.place_informations[2].place.address;
                    PlayerPrefs.SetString("task3_lat", taskallinformation.place_informations[2].place.location.lat.ToString());
                    PlayerPrefs.SetString("task3_lng", taskallinformation.place_informations[2].place.location.lng.ToString());
                    PlayerPrefs.SetString("task3_name", taskallinformation.place_informations[2].place.name);
                    PlayerPrefs.SetString("task3_address", taskallinformation.place_informations[2].place.address);
                    PlayerPrefs.SetString("task3_uid", taskallinformation.place_informations[2].place.uid);
                    HiddenText3.text = "task3";
                }
            }
        }
    }

    public void startTask(string sign) {
        if (sign == "1")
        {
            StartCoroutine(startTaskIEnumerator(ButtonText1, "task1"));
        }
        else if (sign == "2")
        {
            StartCoroutine(startTaskIEnumerator(ButtonText2, "task1"));
        }
        else if (sign == "3")
        {
            StartCoroutine(startTaskIEnumerator(ButtonText3, "task1"));
        }
    }

    private IEnumerator startTaskIEnumerator(Text buttontext, string taskValue)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Cookie", PlayerPrefs.GetString("COOKIE"));
        string data = "lat=" + PlayerPrefs.GetString(taskValue + "_lat") 
            + "&lng=" + PlayerPrefs.GetString(taskValue + "_lng") 
            + "&name=" + PlayerPrefs.GetString(taskValue + "_name") 
            + "&address=" + PlayerPrefs.GetString(taskValue + "_address") 
            + "&uid=" + PlayerPrefs.GetString(taskValue + "_uid") 
            + "&_tokenpasswd=" + PlayerPrefs.GetString("_tokenpasswd");

        byte[] bs = System.Text.UTF8Encoding.UTF8.GetBytes(data);

        WWW www = new WWW(ip + startMethod, bs, headers);
        yield return www;
        if (www.error != null)
        {
            Debug.Log(www.error);
            yield return www.error;
        }
        else
        {
            Status status = LitJson.JsonMapper.ToObject<Status>(www.text);
            if (status.status != "200")
            {
                Debug.Log(status.status);
                yield return status.status;
            }
            else
            {
                PlayerPrefs.SetString("task_lat", PlayerPrefs.GetString(taskValue + "_lat"));
                PlayerPrefs.SetString("task_lng", PlayerPrefs.GetString(taskValue + "_lng"));
                buttontext.text = "任务进行中";
            }
        }
        yield break;
    }

    public void promptTask()
    {
        StartCoroutine(promptTaskIEnumerator());
    }

    private IEnumerator promptTaskIEnumerator()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("位置服务不可用");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // 服务初始化超时  
        if (maxWait < 1)
        {
            Debug.Log("服务初始化超时");
            yield break;
        }

        // 连接失败  
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("无法确定设备位置");
            yield break;
        }
        else
        {
            string url = ip + promptMethod + "/lat/" + Input.location.lastData.latitude + "/lng/" + Input.location.lastData.longitude + "/tokenpasswd/" + PlayerPrefs.GetString("_tokenpasswd");
            GameObject BrowserGo;
            BrowserGo = new GameObject("uniWebViewObject");
            uwv = BrowserGo.GetComponent<UniWebView>();
            if (uwv == null)
            {
                uwv = BrowserGo.AddComponent<UniWebView>();
            }
            uwv.OnLoadComplete += OnLoadComplete;
            uwv.OnWebViewShouldClose += OnWebViewShouldClose;
            uwv.url = url;
            uwv.Load();
        }
        Input.location.Stop();
    }

    private bool OnWebViewShouldClose(UniWebView webView)
    {
        webView.Hide();
        UnityEngine.Object.Destroy(webView);
        uwv = null;
        return true;
    }

    private void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
    {
        if (success)
        {
            webView.Show();
        }
        else
        {
            Debug.LogError("Something wrong in web view loading: " + errorMessage);
        }
    }
}

[System.Serializable]
class TaskAllInformation
{
    public string status;
    public string latitude;
    public string longitude;
    public List<PlaceInformation> place_informations;
}

[System.Serializable]
class PlaceInformation
{
    public int num;
    public Place place;
}

[System.Serializable]
class Place
{
    public string name;
    public PlaceLocation location;
    public string address;
    public string street_id;
    public string telephone;
    public int detail;
    public string uid;
    public DetailInfo detail_info;
}

[System.Serializable]
class PlaceLocation
{
    public double lat;
    public double lng;
}

[System.Serializable]
class DetailInfo
{
    public int distance;
    public string tag;
    public string type;
    public string detail_url;
    public string taste_rating;
    public string price;
    public string overall_rating;
    public string service_rating;
    public string environment_rating;
    public string technology_rating;
    public string image_num;
    public string groupon_num;
    public string comment_num;
    public string atmosphere;
    public string featured_service;
    public string recommendation;
    public string alias;
    public string shop_hours;
    public List<DiReviewKeyword> di_review_keyword;
    public string description;
}

[System.Serializable]
class DiReviewKeyword
{
    public string keyword;
    public string keyword_category_name;
    public int keyword_category_seq;
    public List<string> keyword_comment_ids;
    public string keyword_desc;
    public int keyword_num;
    public string keyword_tag;
    public int keyword_type;
}

[System.Serializable]
class Status
{
    public string status;
}
