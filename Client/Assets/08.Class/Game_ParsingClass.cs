using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//----------------------------------------
// WebRequest 사용하기 위하여
using System;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
//----------------------------------------

namespace TheLastOne.ParsingClass
{
    public class Game_ParsingClass : MonoBehaviour
    {
        //----------------------------------------
        // WebRequest 속도를 위하여 미리 선언
        private HttpWebRequest request;
        private HttpWebResponse response;
        private StreamReader readerPost;
        private Stream dataStrem;
        WebClient wc = new WebClient();
        private string resResult = string.Empty;
        private byte[] sendData;
        private string Getcookie;
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        //----------------------------------------
        public string splitParsing(string Data, string one, string two)
        {
            try
            {
                string searchParsing = one + "(?<splitData>.*?)" + two;
                Regex re = new Regex(searchParsing, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection mc = re.Matches(Data); // str은 처리할 스트링
                Match m = mc[0];
                return m.Groups["splitData"].Value;
            }
            catch
            {
                return "";
            }
        }

        public string httpWebPost(string url, string Referer, string postData, bool Cookie)
        {
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Referer = Referer;
            request.UserAgent = "TheLastOne/Unity/Game";
            request.ContentType = "application/x-www-form-urlencoded";
            //request.Host = "app.genie.co.kr";//splitParsing(url, "https://", "/");
            request.KeepAlive = true;
            request.AllowAutoRedirect = false;
            if (Cookie == true)
            {
                request.CookieContainer = new CookieContainer();
                Uri uri = new Uri(url); // string 을 Uri 로 형변환
                request.CookieContainer.SetCookies(uri, Getcookie);
            }

            sendData = UTF8Encoding.UTF8.GetBytes(postData);

            dataStrem = request.GetRequestStream();
            dataStrem.Write(sendData, 0, sendData.Length);
            dataStrem.Close();

            response = (HttpWebResponse)request.GetResponse();
            readerPost = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8, true);   // Encoding.GetEncoding("EUC-KR")
            resResult = readerPost.ReadToEnd();
            if (Cookie == false)
            {
                Getcookie = response.GetResponseHeader("Set-Cookie"); // 쿠키정보 값을 확인하기 위해서
                Getcookie = Getcookie.Replace("Path=/;", "").Trim();
                Getcookie = Getcookie.Replace("HttpOnly", "").Trim();
            }
            //MessageBox.Show(cookie);
            //MessageBox.Show(resResult);
            readerPost.Close();
            response.Close();
            return resResult;
        }


    }
}