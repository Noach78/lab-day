using UnityEngine;

public class QuitManager : MonoBehaviour
{
    // Cette fonction doit être 'public' pour que le bouton puisse la voir
    public void QuitGame()
    {
        // Affiche un message dans la console (utile pour vérifier que le bouton marche)
        Debug.Log("Fermeture du jeu !");

        // Quitte le jeu (ne fonctionne que sur l'application compilée/buildée)
        Application.Quit();

        // Optionnel : ce code arrête le mode "Play" quand vous testez dans l'éditeur Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}