using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public enum ESubjectClipType
{
    Intro,
    Interview,
    Idle,
    Sentenced
};

[System.Serializable]
public struct ClipKeyframeData
{
    public float time;
    public string subtitles;
    public string displayOnMonitorMessage;
}

[CreateAssetMenu(fileName = "SubjectClipData", menuName = "GameData/SubjectClipData")]
public class SubjectClipData : ScriptableObject
{
    public ESubjectClipType clipType;
    public VideoClip videoClip;
    public bool shouldLoop;
    public List<ClipKeyframeData> keyFrameData;
}