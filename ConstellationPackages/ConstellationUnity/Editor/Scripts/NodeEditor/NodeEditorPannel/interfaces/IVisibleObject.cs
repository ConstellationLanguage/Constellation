using UnityEngine;
namespace ConstellationUnityEditor {
    public interface IVisibleObject {
        bool InView (Rect rect);
    }
}