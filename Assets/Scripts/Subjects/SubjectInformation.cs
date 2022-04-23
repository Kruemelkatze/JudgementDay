using Extensions.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "SubjectInformation", menuName ="GameData/SubjectInformation")]
public class SubjectInformation : ScriptableObject
{
    public SubjectClipData intro;
    public SubjectClipData interview;
    public SubjectClipData idle;
    public SubjectClipData sentenced;
    public SubjectClipData GetClipData(ESubjectClipType type)
    {
        switch(type)
        {
            case ESubjectClipType.Idle:
                return idle;
            case ESubjectClipType.Interview:
                return interview;
            case ESubjectClipType.Sentenced:
                return sentenced;
            case ESubjectClipType.Intro:
                return intro;
        }
        return null;
    }
}
