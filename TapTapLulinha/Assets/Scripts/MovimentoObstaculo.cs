using UnityEngine;

public class MovimentoObstaculo : MonoBehaviour
{
    [Header("Movimentação")]
    public float velocidade = 3f;

    [Header("Limite de Destruição")]
    public float limiteEsquerdo = -12f;

    [Header("Referência")]
    public SpawnObstaculo spawnar;

    private Rigidbody2D rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        if (rb == null || Time.timeScale == 0f)
        {
            return;
        }

        if (spawnar != null)
        {
            velocidade = spawnar.velocidadeObstaculos;
        }

        Vector2 novaPosicao = rb.position + Vector2.left * velocidade * Time.fixedDeltaTime;

        rb.MovePosition(novaPosicao);

        if (rb.position.x < limiteEsquerdo)
        {
            Destroy(gameObject);
        }
    }
}