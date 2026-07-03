using UnityEngine;

namespace Eloi.TBIO
{
    public class TBIOMono_ExitAndRestartLevel : MonoBehaviour
    {
 
        public void QuitTheApplication()
        {
            //if (Application.isEditor) {
            //    #if UNITYE_EITOR
            //                    UnityEditor.EditorApplication.isPlaying = false;
            //    #endif
            //}
            Application.Quit();
        }
        public void RestartTheLevel()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

}
