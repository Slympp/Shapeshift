using System;
using System.Collections;
using System.Collections.Generic;
using Game.Level;
using UnityEngine;

namespace Game {
    public class PlayerController : MonoBehaviour {

        [SerializeField] private Shape defaultShape = Shape.Bear;
        [SerializeField] private List<ShapeProperty> shapeProperties;
        
        [SerializeField] private CameraController cameraController;

        private Shape _currentShape;
        private int _shapeCount;
        private Dictionary<Shape, SerializedShapeProps> _shapes;

        private int _transitionRefHash;
        private MaterialPropertyBlock _transitionPropertyBlock;
    
        void Awake() {
            _currentShape = defaultShape;
            _shapeCount = Enum.GetValues(typeof(Shape)).Length;
        
            _shapes = new Dictionary<Shape, SerializedShapeProps>();
            foreach (var p in shapeProperties)
                _shapes.Add(p.shape, new SerializedShapeProps(p.shapeObject));
            
            _transitionRefHash = Shader.PropertyToID("_Amount");
            _transitionPropertyBlock = new MaterialPropertyBlock();
        }
    
        void Start() {
            UpdateShape(_currentShape, true);
        }
        
        #region SHAPE_SHIFTING

        public IEnumerator ShapeShift(int value, float lockDuration) {

            var transitionDuration = lockDuration / 2f;

            yield return FadeOut(transitionDuration);
            UpdateShape(_currentShape, false);
        
            if (value == 1 && (int) _currentShape == _shapeCount - 1)
                _currentShape = 0;
            else if (value == -1 && (int) _currentShape == 0)
                _currentShape = (Shape) _shapeCount - 1;
            else
                _currentShape += value;

            UpdateShape(_currentShape, true);
            yield return FadeIn(transitionDuration);
        }

        void UpdateShape(Shape shape, bool active) {
            if (_shapes.TryGetValue(shape, out var v)) {
                v.shapeObject.SetActive(active);
                if (active)
                    cameraController.UpdateCameraTarget(v.shapeObject.transform);
            }
        }

        private IEnumerator FadeOut(float duration) {
            var time = 0f;
            if (_shapes.TryGetValue(_currentShape, out var fadeOut)) {
                var totalDuration = duration * Time.timeScale;
                while (time <= totalDuration) {
                    var v = time / totalDuration;
                    
                    _transitionPropertyBlock.SetFloat(_transitionRefHash, v);
                    fadeOut.renderer.SetPropertyBlock(_transitionPropertyBlock);

                    time += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                _transitionPropertyBlock.SetFloat(_transitionRefHash, 1);
                fadeOut.renderer.SetPropertyBlock(_transitionPropertyBlock);
            }
        }

        private IEnumerator FadeIn(float duration) {
            var totalDuration = duration * Time.timeScale;
            var time = totalDuration;
            if (_shapes.TryGetValue(_currentShape, out var fadeIn)) {
                while (time >= 0) {
                    var v = time / totalDuration;
                    
                    _transitionPropertyBlock.SetFloat(_transitionRefHash, v);
                    fadeIn.renderer.SetPropertyBlock(_transitionPropertyBlock);

                    time -= Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                _transitionPropertyBlock.SetFloat(_transitionRefHash, 0);
                fadeIn.renderer.SetPropertyBlock(_transitionPropertyBlock);
            }
        }
        
        #endregion

        private void OnTriggerEnter(Collider other) {
            
            if (!other.CompareTag("Obstacle"))
                return;

            var obstacle = other.transform.GetComponentInParent<Obstacle>();
            if (obstacle != null) {
                if (!obstacle.TryBypass(_currentShape))
                    GameManager.Instance.GameOver();
                else
                    obstacle.DestroyParts();
            } else {
                Debug.Log($"Failed to find Obstacle component in {other}");
                GameManager.Instance.GameOver();
            }
        }

        [Serializable]
        public class ShapeProperty {
            public Shape shape;
            public GameObject shapeObject;
        }

        [Serializable]
        public class SerializedShapeProps {
            public GameObject shapeObject;
            public Renderer renderer;

            public SerializedShapeProps(GameObject p) {
                shapeObject = p;
                renderer = shapeObject.GetComponentInChildren<Renderer>();
            }
        }
    }
}
