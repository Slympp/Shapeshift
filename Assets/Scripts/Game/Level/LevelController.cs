using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Level {
    public class LevelController : MonoBehaviour {

        [Header("Settings")]
        [SerializeField] private bool spawnObstacles = true;
        [SerializeField] private float baseSpeed;
        [SerializeField] private int piecesBetweenObstacles;
        [SerializeField] private Vector2Int dynamicObstacleRange;
        [SerializeField] private float pieceSize;
        [SerializeField] private int forwardDrawDistance;
        [SerializeField] private int backwardDrawDistance;
        
        [Header("Pieces")]
        [SerializeField] private List<GameObject> grounds;
        [SerializeField] private List<Obstacle> obstacles;
        [SerializeField] private List<ParallaxBackgroundLayer> parallaxBackgroundLayer;

        private readonly List<Transform> _instantiatedPieces = new List<Transform>();
        private int _remainingPiecesBeforeObstacle;

        void Start() {
            _remainingPiecesBeforeObstacle = GetPiecesBeforeNextObstacle();
            
            for (var i = backwardDrawDistance; i < forwardDrawDistance; i++)
                _instantiatedPieces?.Add(SpawnGround(i * pieceSize));
        }

        void Update() {

            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused)
                return;
            
            MoveBackground(baseSpeed * Time.deltaTime * Vector3.back);
        }

        private void FixedUpdate() {
            
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused)
                return;
            
            MoveGround(baseSpeed * Time.deltaTime * Vector3.back);
        }

        private void MoveGround(Vector3 movement) {
            foreach (var p in _instantiatedPieces) {
                if (p.position.z <= pieceSize * (backwardDrawDistance - 1)) {

                    if (p.childCount > 0) {
                        // TODO: Move back obstacles to pool
                        foreach (Transform t in p)
                            if (t.CompareTag("Obstacle")) 
                                Destroy(t.gameObject);
                    }

                    _remainingPiecesBeforeObstacle--;
                    if (_remainingPiecesBeforeObstacle <= 0) {
                        _remainingPiecesBeforeObstacle = GetPiecesBeforeNextObstacle();
                        if (spawnObstacles)
                            SpawnObstacle(p);
                    }
                    
                    p.position += new Vector3(0, 0, (forwardDrawDistance - backwardDrawDistance) * pieceSize);
                }
                p.position += movement;
            }
        }

        private void MoveBackground(Vector3 movement) {
            foreach (var l in parallaxBackgroundLayer)
                l.ApplyMovement(movement);
        }

        Transform SpawnGround(float position) {
            if (grounds == null || grounds.Count == 0)
                return null;

            var piece = grounds[Random.Range(0, grounds.Count)];
            return Instantiate(piece, piece.transform.position + new Vector3(0, 0, position), Quaternion.identity, transform)?.transform;
        }

        private void SpawnObstacle(Transform groundAnchor) {
            var prefab = obstacles[Random.Range(0, obstacles.Count)];
            var obstacle = Instantiate(prefab, groundAnchor);
            var position = groundAnchor.transform.position;
            obstacle.transform.localPosition = prefab.transform.position - new Vector3(position.x, position.y, Random.Range(-pieceSize / 2, pieceSize / 2));
        }

        int GetPiecesBeforeNextObstacle() => piecesBetweenObstacles + Random.Range(dynamicObstacleRange.x, dynamicObstacleRange.y);
    }

    [Serializable]
    class Vector2Int {
        public int x;
        public int y;
    }
}
