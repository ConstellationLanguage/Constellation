using UnityEngine;
namespace ConstellationEditor {
    public interface IVisibleObject {
        bool InView (Rect rect);
    }
}