using UnityEngine;
using UnityEngine.Events;

namespace Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        public bool Debug = true;
        
        public UnityEvent<GameObject> OnPlayerDie = new UnityEvent<GameObject>();
    }
}