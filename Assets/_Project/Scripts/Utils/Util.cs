using UnityEngine;

namespace Utils
{
    public static class Util {
    
        public static bool IsInLayer(GameObject gameObject, LayerMask layerMask)
        {
            return (layerMask.value & (1 << gameObject.layer)) != 0;
        }
    }
}
