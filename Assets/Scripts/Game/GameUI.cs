using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace Game
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Text _scoreTxt;
        private const string SCORE = "Score: {0}";

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Score
        private void AddScore(int points)
        {


            //todo: do something cool
        }

        private void SubstractScore(int points)
        {

        }

        public void UpdateScoreTxt(int score, int points)
        {
            _scoreTxt.text = string.Format(SCORE, score);

            if (points > 0) //If we're adding
            {
                AddScore(points);
            }
            else //If we're substracting
            {
                SubstractScore(points);
            }
        }
        #endregion
    }
}
