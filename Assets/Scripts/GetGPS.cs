using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class GetGPS:MonoBehaviour
{
    public GetGPS GetGps;
    public UniWebView uwv;
    public Image image;
    public string url;
    public Text text1;
    public Text text2;
    public Text text3;
    public Text htext1;
    public Text htext2;
    public Text htext3;
    public Text btext1;
    public Text btext2;
    public Text btext3;








    public void showTask ()
    {  
        StartCoroutine (IEGpsShowTask());   
    }
    
    IEnumerator IEGpsShowTask ()
    {
        if (!Input . location . isEnabledByUser)
        {
            Debug . Log ("位置服务不可用");
            yield break;
        }

        Input . location . Start ();

        int maxWait = 20;
        while (Input . location . status == LocationServiceStatus . Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds (1);
            maxWait--;
        }

        // 服务初始化超时  
        if (maxWait < 1)
        {
            Debug . Log ("服务初始化超时");
            yield break;
        }

        // 连接失败  
        if (Input . location . status == LocationServiceStatus . Failed)
        {
            Debug . Log ("无法确定设备位置");
            yield break;
        }
        else
        {
            url = "http://115.159.147.201/goplace/index.php/Home/Map/show/latitude/" + PlayerPrefs . GetString ("task_lat") + "/longitude/" + PlayerPrefs . GetString ("task_lng") + "/mylat/" + Input . location . lastData . latitude + "/mylng/" + Input . location . lastData . longitude;
            GameObject BrowserGo;
            BrowserGo = new GameObject ("uniWebViewObject");
            uwv = BrowserGo . GetComponent<UniWebView> ();
            if (uwv == null)
            {
                uwv = BrowserGo . AddComponent<UniWebView> ();
            }
            uwv . OnLoadComplete += OnLoadComplete;
            uwv . OnWebViewShouldClose += OnWebViewShouldClose;
            uwv . url = url;
            uwv . Load ();
        }
        Input . location . Stop ();
    }

    private bool OnWebViewShouldClose (UniWebView webView)
    {
        webView . Hide ();
        UnityEngine . Object . Destroy (webView);
        uwv = null;
        return true;
    }

    private void OnLoadComplete (UniWebView webView ,bool success ,string errorMessage)
    {
        if (success)
        {
            webView . Show ();
        }
        else
        {
            Debug . LogError ("Something wrong in web view loading: " + errorMessage);
        }
    }

    public void uploadTask (string text)
    {
        string hvalue = "";
        if (text == "1")
        {
            hvalue = htext1 . text;
        }
        else if (text == "2")
        {
            hvalue = htext2 . text;
        }
        else if (text == "3")
        {
            hvalue = htext3 . text;
        }

        if (hvalue == "none")
        {
            return;
        }

        StartCoroutine (IEUploadTask (hvalue,text));
    }

    IEnumerator IEUploadTask (string hvalue,string text)
    {
        Dictionary<string ,string> headers = new Dictionary<string ,string> ();
        headers . Add ("Cookie" ,PlayerPrefs . GetString ("COOKIE") . Replace (";" ,""));

        string data = "userid="+ PlayerPrefs . GetString ("userid") + "&lat="+ PlayerPrefs . GetString (hvalue + "_lat") + "&lng="+ PlayerPrefs . GetString (hvalue + "_lng")+ "&name=" + PlayerPrefs . GetString (hvalue + "_name")+ "&address="+ PlayerPrefs . GetString (hvalue + "_address");
        byte [ ] bs = System . Text . UTF8Encoding . UTF8 . GetBytes (data);

        WWW www = new WWW ("http://115.159.147.201/goplace/index.php/Home/user/task" ,bs ,headers);
        yield return www ;
        if (www . error != null)
        {
            Debug . Log (www . error);
            yield return www . error;
        }
        else
        {
            if (www . text != "ok")
            {
                Debug . Log (www . text);
                yield return www . text;
            }
            else
            {
                PlayerPrefs . SetString ("task_lat" ,PlayerPrefs . GetString (hvalue + "_lat"));
                PlayerPrefs . SetString ("task_lng" ,PlayerPrefs . GetString (hvalue + "_lng"));
                if (text == "1")
                {
                    btext1.text = "任务进行中";
                }
                else if (text == "2")
                {
                    btext2 . text = "任务进行中";
                }
                else if (text == "3")
                {
                    btext3 . text = "任务进行中";
                }
            }
        }
    }

    public void loginTask ()
    {
        StartCoroutine (IEloginTask ());
    }

    IEnumerator IEloginTask ()
    {
       /* if (!Input . location . isEnabledByUser)
        {
            Debug . Log ("位置服务不可用");
            yield break;
        }

        Input . location . Start ();

        int maxWait = 20;
        while (Input . location . status == LocationServiceStatus . Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds (1);
            maxWait--;
        }

        // 服务初始化超时  
        if (maxWait < 1)
        {
            Debug . Log ("服务初始化超时");
            yield break;
        }

        // 连接失败  
        if (Input . location . status == LocationServiceStatus . Failed)
        {
            Debug . Log ("无法确定设备位置");
            yield break;
        }
        else
        {*/

            Dictionary<string ,string> headers = new Dictionary<string ,string> ();
        string cookie = Regex.Replace(PlayerPrefs.GetString("COOKIE"), "expires=Wed, \\d+-[a-zA-Z]+-\\d+ \\d+:\\d+:\\d+ GMT; Max-Age=7200;", "");
        cookie = cookie.Replace("path=/,", "");
        cookie = cookie.Replace("path=/; HttpOnly", "");
        headers . Add ("Cookie" , cookie);
        string data = "userid=1&_tokenpasswd=" + PlayerPrefs.GetString("_tokenpasswd");   
        // string data = "userid=" + PlayerPrefs . GetString ("userid") + "&lat=" + Input . location . lastData . latitude + "&lng=" + Input . location . lastData . longitude;
            byte [ ] bs = System . Text . UTF8Encoding . UTF8 . GetBytes (data);

            WWW www = new WWW ("http://127.0.0.1/goplace/public/task/show" ,bs ,headers);
            yield return www;
            if (www . error != null)
            {
                Debug . Log (www . error);
                yield return www . error;
            }
            else
            {
            Debug.Log(www.text);
                /*if (www . text != "恭喜任务完成")
                {
                    text1 . text = www . text;
                    yield return www . text;
                }
                else
                {
                    text1 . text = www . text;
                    yield return www . text;
                }*/
            }
        //}
        //Input . location . Stop ();
    }


    public void startTask ()
    {
        StartCoroutine (uploadGpsToLbs ());
    }

    IEnumerator uploadGpsToLbs ()
    {
         if (!Input . location . isEnabledByUser)
         {
             Debug . Log ("位置服务不可用");
             yield break;
         }

         Input . location . Start ();

         int maxWait = 20;
         while (Input . location . status == LocationServiceStatus . Initializing && maxWait > 0)
         {
             yield return new WaitForSeconds (1);
             maxWait--;
         }

         // 服务初始化超时  
         if (maxWait < 1)
         {
             Debug . Log ("服务初始化超时");
             yield break;
         }

         // 连接失败  
         if (Input . location . status == LocationServiceStatus . Failed)
         {
             Debug . Log ("无法确定设备位置");
             yield break;
         }
         else
         {
            url = "http://115.159.147.201/goplace/index.php/Home/Map/lbs/latitude/" + Input . location . lastData . latitude+"/longitude/" + Input . location . lastData . longitude;
            WWW www = new WWW (url);
            yield return www;
            if (www . error != null)
            {
                Debug . Log (www . error);
                yield return www . error;
            }
            else
            {
                Loc loc = LitJson . JsonMapper . ToObject<Loc> (www . text);
                PlayerPrefs . SetString ("mylat" ,loc . lat.ToString());
                PlayerPrefs . SetString ("mylng" ,loc . lng.ToString());
                int count = loc . result . Count;
                if (count == 1)
                {
                    text1 . text = "任务：请到达 " + loc . result [ 0 ] . name + " 详细地址：" + loc . result [ 0 ] . address;
                    PlayerPrefs . SetString ("task1_lat" ,loc . result [ 0 ] . location . lat . ToString ());
                    PlayerPrefs . SetString ("task1_lng" ,loc . result [ 0 ] . location . lng . ToString ());
                    PlayerPrefs . SetString ("task1_name" ,loc . result [ 0 ] . name);
                    PlayerPrefs . SetString ("task1_address" ,loc . result [ 0 ] . address);
                    htext1 . text = "task1";
                }
                else if (count == 2)
                {
                    text1 . text = "任务：请到达 " + loc . result [ 0 ] . name + " 详细地址：" + loc . result [ 0 ] . address;
                    text2 . text = "任务：请到达 " + loc . result [ 1 ] . name + " 详细地址：" + loc . result [ 1 ] . address;
                    PlayerPrefs . SetString ("task1_lat" ,loc . result [ 0 ] . location . lat . ToString ());
                    PlayerPrefs . SetString ("task1_lng" ,loc . result [ 0 ] . location . lng . ToString ());
                    PlayerPrefs . SetString ("task1_name" ,loc . result [ 0 ] . name);
                    PlayerPrefs . SetString ("task1_address" ,loc . result [ 0 ] . address);
                    PlayerPrefs . SetString ("task2_lat" ,loc . result [ 1 ] . location . lat . ToString ());
                    PlayerPrefs . SetString ("task2_lng" ,loc . result [ 1 ] . location . lng . ToString ());
                    PlayerPrefs . SetString ("task2_name" ,loc . result [ 1 ] . name);
                    PlayerPrefs . SetString ("task2_address" ,loc . result [ 1 ] . address);
                    htext1 . text = "task1";
                    htext2 . text = "task2";
                }
                else
                {
                    text1 . text = "任务：请到达 " + loc . result [ 0 ] . name + " 详细地址：" + loc . result [ 0 ] . address;
                    text2 . text = "任务：请到达 " + loc . result [ 1 ] . name + " 详细地址：" + loc . result [ 1 ] . address;
                    text3 . text = "任务：请到达 " + loc . result [ 2 ] . name + " 详细地址：" + loc . result [ 2 ] . address;
                    PlayerPrefs . SetString ("task1_lat" ,loc . result [ 0 ] . location . lat . ToString ());
                    PlayerPrefs . SetString ("task1_lng" ,loc . result [ 0 ] . location . lng . ToString ());
                    PlayerPrefs . SetString ("task1_name" ,loc . result [ 0 ] . name);
                    PlayerPrefs . SetString ("task1_address" ,loc . result [ 0 ] . address);
                    PlayerPrefs . SetString ("task2_lat" ,loc . result [ 1 ] . location . lat . ToString ());
                    PlayerPrefs . SetString ("task2_lng" ,loc . result [ 1 ] . location . lng . ToString ());
                    PlayerPrefs . SetString ("task2_name" ,loc . result [ 1 ] . name);
                    PlayerPrefs . SetString ("task2_address" ,loc . result [ 1 ] . address);
                    PlayerPrefs . SetString ("task3_lat" ,loc . result [ 2 ] . location . lat . ToString ());
                    PlayerPrefs . SetString ("task3_lng" ,loc . result [ 2 ] . location . lng . ToString ());
                    PlayerPrefs . SetString ("task3_name" ,loc . result [ 2 ] . name);
                    PlayerPrefs . SetString ("task3_address" ,loc . result [ 2 ] . address);
                    htext1 . text = "task1";
                    htext2 . text = "task2";
                    htext3 . text = "task3";
                }
            }
        }
        Input . location . Stop ();
    }
}

[System . Serializable]
class Loc
{
    public string status;
    public double lat;
    public double lng;
    public List<Result> result;
}

[System . Serializable]
class Result
{
    public string name;
    public Location location;
    public string telephone;
    public string address;
    public string street_id;
    public int detail;
    public string uid;
}

[System . Serializable]
class Location
{
    public double lat;
    public double lng;
}