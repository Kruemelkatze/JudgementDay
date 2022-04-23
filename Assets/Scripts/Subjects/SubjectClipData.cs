using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public enum ESubjectClipType
{
    Entry,
    Confession,
    AwaitSentencing,
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
    public string videoClipURL;
    public bool shouldLoop;
    public List<ClipKeyframeData> keyFrameData;
}