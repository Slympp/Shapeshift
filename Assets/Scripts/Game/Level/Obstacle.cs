using System.Collections.Generic;
using UnityEngine;

namespace Game.Level {
    public class Obstacle : MonoBehaviour {

        [SerializeField] private List<Shape> bypassingShapes;
        [SerializeField] private List<ExplodableObstacle> _destroyableObstacles;

        public bool TryBypass(Shape shape) => bypassingShapes.Contains(shape);

        public void DestroyParts() {
            foreach (var p in _destroyableObstacles) {
                p.Explode();
            }
        }
    }
}
