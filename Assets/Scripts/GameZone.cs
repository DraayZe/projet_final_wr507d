using UnityEngine;

public class GameZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Quelque chose est entré dans la zone : {other.name} avec le tag {other.tag}");

        if (other.CompareTag(playerTag))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
                Debug.Log("Joueur entré dans la zone - Jeu démarré !");
            }
            else
            {
                Debug.LogWarning("GameManager.Instance est null !");
            }
        }
        else
        {
            Debug.LogWarning($"L'objet {other.name} n'a pas le bon tag. Tag actuel: {other.tag}, Tag attendu: {playerTag}");
        }
    }
}
