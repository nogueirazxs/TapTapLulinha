
using UnityEngine;
using UnityEngine.InputSystem;

public class personagem : MonoBehaviour
{
    private Rigidbody2D rb;
    public float forcaDoImpulso = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Impede o jogador de se movimentar durante a pausa.
        if (Time.timeScale == 0f)
            return;

        bool clicouOuTocou = false;

        // Detecta toque no celular.
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            clicouOuTocou = true;
        }
        // Detecta clique no PC.
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            clicouOuTocou = true;
        }

        if (clicouOuTocou)
        {
            // Mantém o impulso vertical consistente.
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            rb.AddForce(Vector2.up * forcaDoImpulso, ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        // Verifica se bateu no obstáculo.
        bool bateuNoObstaculo = colisao.gameObject.CompareTag("Obstaculo") || colisao.transform.root.CompareTag("Obstaculo");

        // Verifica se bateu no chão.
        bool bateuNoChao = colisao.gameObject.CompareTag("Chao") || colisao.transform.root.CompareTag("Chao");

        if (bateuNoObstaculo || bateuNoChao)
        {
            if (gerenciador_tudo.instancia != null)
                gerenciador_tudo.instancia.ReiniciarCena();
        }
    }
}
