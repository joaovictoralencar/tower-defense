using UnityEngine;

namespace Singletons
{
    public class GameManager : Singleton<GameManager>
    {
        public bool Debug = true;
    }
}