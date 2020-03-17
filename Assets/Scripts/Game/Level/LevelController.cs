using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Level {
    public class LevelController : MonoBehaviour {

        [SerializeField] private bool spawnObstacles = true;
        [SerializeField] private float baseSpeed = 3;
        [SerializeField] private int piecesBetweenObstacles;
        [SerializeField] private float pieceSize;
        [SerializeField] private int drawDistance;
        
        [Header("Pieces")]
        [SerializeField] private List<GameObject> groundPieces;
        [SerializeField] private List<Obstacle> obstacles;
        [SerializeField] private List<ParallaxBackgroundLayer> parallaxBackgroundLayer;

        private readonly List<Transform> _instantiatedPieces = new List<Transform>();
        private int _remainingPiecesBeforeObstacle;

        void Start() {
            _remainingPiecesBeforeObstacle = piecesBetweenObstacles;
            
            for (var i = -1; i < drawDistance; i++)
                _instantiatedPieces?.Add(SpawnGround(i * pieceSize));
        }

        void Update() {
            MoveBackground(baseSpeed * Time.deltaTime * Vector3.back);
        }

        private void FixedUpdate() {
            MovePieces(baseSpeed * Time.deltaTime * Vector3.back);
        }

        private void MovePieces(Vector3 movement) {
            foreach (var p in _instantiatedPieces) {

                if (p.position.z <= -pieceSize * 2) {

                    if (p.childCount > 0) {
                        // TODO: Move back obstacles to pool
                        foreach (Transform t in p) {
                            Destroy(t.gameObject);
                        }
                    }

                    _remainingPiecesBeforeObstacle--;
                    if (_remainingPiecesBeforeObstacle <= 0) {
                        _remainingPiecesBeforeObstacle = piecesBetweenObstacles;
                        if (spawnObstacles)
                            SpawnObstacle(p);
                    }
                    
                    p.position += new Vector3(0, 0, drawDistance * pieceSize);
                }

                p.position += movement;
            }
        }

        private void MoveBackground(Vector3 movement) {
            foreach (var l in parallaxBackgroundLayer)
                l.ApplyMovement(movement);
        }

        // Increase speed exponentially while _gameDuration belongs to [speedIncreaseFrame.x, speedIncreaseFrame.y]

        Transform SpawnGround(float position) {
            if (groundPieces == null || groundPieces.Count == 0)
                return null;

            var piece = groundPieces[Random.Range(0, groundPieces.Count)];
            return Instantiate(piece, piece.transform.position + new Vector3(0, 0, position), Quaternion.identity, transform)?.transform;
        }

        private void SpawnObstacle(Transform groundAnchor) {
            var prefab = obstacles[Random.Range(0, obstacles.Count)];
            var obstacle = Instantiate(prefab, groundAnchor);
            var position = groundAnchor.transform.position;
            obstacle.transform.localPosition = prefab.transform.position - new Vector3(position.x, position.y, Random.Range(-pieceSize / 2, pieceSize / 2));
        }
    }
}
