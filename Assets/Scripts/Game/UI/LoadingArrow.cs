using DG.Tweening;
using UnityEngine;

namespace Game.UI {
    public class LoadingArrow : MonoBehaviour {

        [SerializeField] private float duration = 1f;

        void Start() {
            transform.DORotate(new Vector3(0, 0, -360), duration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetRelative()
                    .SetLoops(-1, LoopType.Incremental)
                    .SetUpdate(UpdateType.Normal, true);
        }
    }
}