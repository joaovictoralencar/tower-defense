using UnityEngine;
using UnityEngine.Events;

namespace Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        public bool Debug = true;
        
        public UnityEvent<GameObject> OnPlayerDie = new UnityEvent<GameObject>();
        /// <summary>
        /// startPosition, endposition
        /// </summary>
        public UnityEvent<Vector3, Vector3> OnGenerateGrid = new UnityEvent<Vector3, Vector3>();
    }
}