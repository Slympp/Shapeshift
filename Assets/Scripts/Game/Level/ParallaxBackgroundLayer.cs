using System.Collections.Generic;
using UnityEngine;

namespace Game.Level {
    public class ParallaxBackgroundLayer : MonoBehaviour {

        [Range(0, 1f)]
        [SerializeField] private float speed;
        [SerializeField] private float size;
        [SerializeField] private int forwardDrawDistance;
        [SerializeField] private int backwardDrawDistance = -1;

        [SerializeField] private GameObject backgroundObject;

        private readonly List<Transform> _childs = new List<Transform>();

        private void Awake() {
            var t = transform;
            for (var i = backwardDrawDistance; i < forwardDrawDistance; i++)
                _childs?.Add(Instantiate(backgroundObject, backgroundObject.transform.position + t.position + new Vector3(0, 0, i * size), Quaternion.identity, t)?.transform);
        }

        public void ApplyMovement(Vector3 movement) {
            foreach (var c in _childs) {
                if (c.position.z <= size * (backwardDrawDistance - 1))
                    c.position += new Vector3(0, 0, size * (forwardDrawDistance - backwardDrawDistance));

                c.position += movement * speed;
            }
        }
    }
}