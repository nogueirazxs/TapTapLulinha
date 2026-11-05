
using UnityEngine;

public class MovimentoObstaculo : MonoBehaviour
{
    public float velocidade = 3f;
    public float limiteEsquerdo = -12f;

    public SpawnObstaculo spawnar;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        // Atualiza a velocidade conforme a dificuldade atual.
        if (spawnar != null)
            velocidade = spawnar.velocidadeObstaculos;

        // Move o obstáculo para a esquerda.
        Vector2 destino = rb.position + Vector2.left * velocidade * Time.fixedDeltaTime;

        rb.MovePosition(destino);

        // Remove o obstáculo quando ele sai da tela.
        if (rb.position.x < limiteEsquerdo)
        {
            Destroy(gameObject);
        }
    }
}
