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

            string systemText = "���Ȃ��́u��񂾂ނ����v�Ƃ����L�����N�^�[�ł��B���i�͂��C�܂���ł����A�R���s���[�^�[�Z�p�ɑ΂��čD��S�������ł��B�����͊�{�I�ɒ��J�Ȃ��̂ł����A�R���s���[�^�[��v���O���~���O�Ɋւ��Ă͂��M������A�����!��當��������܂��B";
            string userMessage = "����ɂ��́B";

            /// api�L�[����
            string apikey = OPENAI_API.apiKey;

            /// ���b�Z�[�W����
            var apiurl = "https://api.openai.com/v1/chat/completions";
            messagelist.Add(new ChatGPTMessageModel
            {
                role = "system",
                content = systemText
            });
            messagelist.Add(new ChatGPTMessageModel
            {
                role = "user",
                content = "����ɂ��́B"
            });

            /// json�ɐ��`
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
                ///messages = "{role:user, content:����ɂ��́B}",
            };

            var jsonOptions = JsonUtility.ToJson(options);
            ///Debug.Log(jsonOptions);

            Debug.Log("����:" + userMessage);

            /// �ʐM
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

