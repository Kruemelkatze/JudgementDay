using Extensions.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "SubjectInformation", menuName ="GameData/SubjectInformation")]
public class SubjectInformation : ScriptableObject
{
    public Sprite sprite = null;
    public string subjectName = "<unknown>";
    public string country = "<unknown";
    [TextArea(7, 10)]
    public string informationText = "<no information>";
    public SubjectClipData entry;
    public SubjectClipData confession;
    public SubjectClipData awaitingSentence;
    public SubjectClipData sentenced;
    public SubjectClipData GetClipData(ESubjectClipType type)
    {
        switch(type)
        {
            case ESubjectClipType.AwaitSentencing:
                return awaitingSentence;
            case ESubjectClipType.Confession:
                return confession;
            case ESubjectClipType.Sentenced:
                return sentenced;
            case ESubjectClipType.Entry:
                return entry;
        }
        return null;
    }
}
