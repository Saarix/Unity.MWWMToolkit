using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [CreateAssetMenu(fileName = "Rarity Color Map", menuName = "Geewa/KeyMaps/Rarity Color")]
    public class RarityColorMap : ScriptableObject
    {
        [field: SerializeField] public List<RarityColorMapEntity> Entities { get; set; }
    }

    [Serializable]
    public struct RarityColorMapEntity
    {
        [field: SerializeField] public int Rarity { get; set; }
        [field: SerializeField] public Color FrameInnerColor { get; set; }
        [field: SerializeField] public Color BackgroundColor { get; set; }
        [field: SerializeField] public Color BackgroundShineColor { get; set; }
        [field: SerializeField] public Gradient GradientColor { get; set; }
    }
}
