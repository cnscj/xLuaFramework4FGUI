using UnityEngine;

namespace XGame
{

    public class GameController : MonoBehaviour
    {
        private static GameController s_instance;

        public static GameController GetInstance()
        {
            return s_instance;
        }

        void Awake()
        {
            s_instance = this;
        }

        void Start()
        {

        }


        void Update()
        {

        }
    }
}
