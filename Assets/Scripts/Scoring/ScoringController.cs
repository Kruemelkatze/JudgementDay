using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using General;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;

namespace Scoring
{
    public class ScoringController : MonoBehaviour
    {
        public static readonly string UserName = Guid.NewGuid().ToString();

        [SerializeField] private string apiUrl = "https://judgementdayapi.vercel.app";

        private void Awake()
        {
            Hub.Register(this);
        }

        public void PostScore(string subjectName, int score)
        {
            score = Mathf.Clamp(score, 0, 10);
            var url = apiUrl + "/api/addStat";

            var scoreEntry = new ScoreEntry()
            {
                name = subjectName,
                score = score,
                user = UserName,
            };

            RestClient.Post(url, scoreEntry).Then(response => { Console.WriteLine(response.Text); })
                .Catch(error => { Console.WriteLine(error); });
        }

        public Task<Dictionary<string, float>> GetScores()
        {
            var url = apiUrl + "/api/stats";

            var t = new TaskCompletionSource<Dictionary<string, float>>();
            try
            {
                RestClient.Get(url)
                    .Then(response =>
                    {
                        var result = JsonConvert.DeserializeObject<StatResult>(response.Text);
                        t.TrySetResult(result.stats);
                    })
                    .Catch(error => { t.TrySetResult(new Dictionary<string, float>()); });

                //RestClient.Get(url).Then(response =>
                //{
                //    UnityEditor.EditorUtility.DisplayDialog("Response", response.Text, "Ok");
                //});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                t.TrySetResult(new Dictionary<string, float>());
            }

            return Task.Run(() => t.Task);
        }


#if UNITY_EDITOR

        [UnityEditor.CustomEditor(typeof(ScoringController))]
        public class CamControllerEditor : UnityEditor.Editor
        {
            private string subjectName = "testSubject";
            private int score = 4;

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                var scoringController = target as ScoringController;
                if (scoringController == null)
                {
                    return;
                }

                subjectName = UnityEditor.EditorGUILayout.TextField("SubjectName", subjectName);
                score = UnityEditor.EditorGUILayout.IntField("Score", score);

                if (GUILayout.Button("POST"))
                {
                    scoringController.PostScore(subjectName, score);
                }

                if (GUILayout.Button("GET STATS"))
                {
                    GetScores(scoringController);
                }
            }

            private async Task GetScores(ScoringController scoringController)
            {
                var scores = await scoringController.GetScores();
                foreach (var scoreEntry in scores)
                {
                    Console.WriteLine($"{scoreEntry.Key}: {scoreEntry.Value}");
                    Debug.Log($"{scoreEntry.Key}: {scoreEntry.Value}");
                }
            }
        }

#endif
    }
}