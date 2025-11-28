using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, ITarget
{
    private static IDictionary<string, ITarget> targets = new Dictionary<string, ITarget>();

    #region Inspector fields

    [SerializeField] private string targetKey;

    [Header("Used for offseting tooltip position")]
    [SerializeField] private Vector2 offset;

    #endregion Inspector fields

    #region Properties

    public string TargetKey
    {
        get => targetKey;
        set
        {
            targetKey = value;

            AddKey();
        }
    }
    public Vector2 Offset => offset;
    public RectTransform RectTra => rectTra ?? (rectTra = GetComponent<RectTransform>());
    public Transform Transform => transform;
    public GameObject GameObject
    {
        get
        {
            if (this == null)
                return null;
            else
                return gameObject;
        }
    }

    #endregion Properties

    private RectTransform rectTra;

    private void Start()
    {
        AddKey();
    }

    private void OnDestroy()
    {
        targets.Remove(targetKey);
    }

    public static ITarget Find(string targetKey)
    {
        if (targets.ContainsKey(targetKey))
        {
            return targets[targetKey];
        }
        else
        {
            Debug.LogError($"Target with key={targetKey}, was not found!");
            return null;
        }
    }

    public static bool TryFind(string targetKey, out ITarget target)
    {
        if (targets.ContainsKey(targetKey))
        {
            target = targets[targetKey];
            return true;
        }

        target = default;
        return false;
    }

    private void AddKey()
    {
        if (!string.IsNullOrWhiteSpace(targetKey) && !targets.ContainsKey(targetKey))
            targets.Add(targetKey, this);
    }
}

