using UnityEngine;

namespace Wartermelon
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton Pattern
        static private GameManager instance = null;

        static public GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                }

                return instance;
            }
        }
        #endregion

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

