using UnityEngine;

public class PropsCollider : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;


    private void Awake()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Props"))
        {

            LayerOrder order = collision.gameObject.GetComponent<LayerOrder>();
            order.render.gameObject.SetActive(true);
        }
    }
}
