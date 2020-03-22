using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    GameManager gm;
    public float secondsUntilDestructionAfterContact = 1f;

    void Start()
    {
        gm = (GameManager)FindObjectOfType(typeof(GameManager));
    }
    void OnCollisionStay(Collision col){
        if (col.gameObject.layer == LayerMask.NameToLayer("Boundary")){
            Destroy(gameObject, secondsUntilDestructionAfterContact); // destory if dropable hits ground or boundary
        }
    }
}
