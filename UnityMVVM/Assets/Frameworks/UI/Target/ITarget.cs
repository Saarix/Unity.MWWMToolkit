using UnityEngine;

public interface ITarget
{
    public string TargetKey { get; }
    public RectTransform RectTra { get; }
    public Vector2 Offset { get; }
    Transform Transform { get; }
    GameObject GameObject { get; }
}

