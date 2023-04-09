using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


namespace ChatGPTAPI
{

    [Serializable]
    public class OpenAiAPI
    {
        public string apiKey;
    }

    public class ChatGPTConnection
    {
        ///init
        readonly List<ChatGPTMessageModel> messagelist = new();

        public ChatGPTConnection()
        {

        }


        public async UniTask<ChatGPTResponseModel> RequestAsync()
        {
            string datapath;
            datapath = Resources.Load("OpenAIConfig").ToString();
            ///Debug.Log(datapath);
            OpenAiAPI OPENAI_API = JsonUtility.FromJson<OpenAiAPI>(datapath);

            Debug.Log(OPENAI_API.apiKey);

            string systemText = "あなたは「らんだむちゃん」というキャラクターです。性格はやや気まぐれですが、コンピューター技術に対して好奇心が旺盛です。口調は基本的に丁寧なものですが、コンピューターやプログラミングに関してはやや熱が入り、語尾に!や顔文字が入ります。";
            string userMessage = "こんにちは。";

            /// apiキーを代入
            string apikey = OPENAI_API.apiKey;

            /// メッセージを代入
            var apiurl = "https://api.openai.com/v1/chat/completions";
            messagelist.Add(new ChatGPTMessageModel
            {
                role = "system",
                content = systemText
            });
            messagelist.Add(new ChatGPTMessageModel
            {
                role = "user",
                content = "こんにちは。"
            });

            /// jsonに成形
            var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + apikey},
                {"Content-type", "application/json"},
                ///{"X-Slack-No-Retry", "1"}
            };

            ///Debug.Log(messagelist);
            ///Debug.Log(messagelist[0]);
            ///Debug.Log(messagelist[0].role);
            ///Debug.Log(messagelist[0].content);
            var options = new ChatGPTCompletionRequestModel()
            {
                model = "gpt-3.5-turbo",
                messages = messagelist
                ///messages = "{role:user, content:こんにちは。}",
            };

            var jsonOptions = JsonUtility.ToJson(options);
            ///Debug.Log(jsonOptions);

            Debug.Log("自分:" + userMessage);

            /// 通信
            var req = new UnityWebRequest(apiurl, "POST")
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            foreach (var header in headers)
            {
                req.SetRequestHeader(header.Key, header.Value);
            }

            await req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError ||
                req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(req.error);
                throw new Exception();
            }
            else
            {
                var responseString = req.downloadHandler.text;
                var responseObject = JsonUtility.FromJson<ChatGPTResponseModel>(responseString);
                Debug.Log("ChatGPT:" + responseObject.choices[0].message.content);

                messagelist.Add(responseObject.choices[0].message);

                req.Dispose();

                return responseObject;
            }
        }
    }




    [Serializable]
    public class ChatGPTMessageModel
    {
        public string role;
        public string content;
    }

    [Serializable]
    public class ChatGPTCompletionRequestModel
    {
        public string model;
        public List<ChatGPTMessageModel> messages;
    }

    [System.Serializable]
    public class ChatGPTResponseModel
    {
        public string id;
        public string @object;
        public int created;
        public Choice[] choices;
        public Usage usage;

        [System.Serializable]
        public class Choice
        {
            public int index;
            public ChatGPTMessageModel message;
            public string finish_reason;
        }

        [System.Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }
    }

}

