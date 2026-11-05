using UnityEngine;
using UnityEngine.InputSystem;

public class personagem : MonoBehaviour
{
    [Header("Pulo")]
    public float forcaDoImpulso = 5f;

    private Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if (Time.timeScale == 0f || rb == null)
        {
            return;
        }

        bool clicouOuTocou = false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            clicouOuTocou = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            clicouOuTocou = true;
        }

        if (clicouOuTocou)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            rb.AddForce(Vector2.up * forcaDoImpulso, ForceMode2D.Impulse);
        }
    }


    void OnCollisionEnter2D(Collision2D colisao)
    {
        bool bateuNoObstaculo = false;
        bool bateuNoChao = false;

        if (colisao.gameObject.CompareTag("Obstaculo") || colisao.transform.root.CompareTag("Obstaculo"))
        {
            bateuNoObstaculo = true;
        }

        if (colisao.gameObject.CompareTag("Chao") || colisao.transform.root.CompareTag("Chao"))
        {
            bateuNoChao = true;
        }

        if (bateuNoObstaculo || bateuNoChao)
        {
            if (gerenciador_tudo.instancia != null)
            {
                gerenciador_tudo.instancia.PerderJogo();
            }
        }
    }
}