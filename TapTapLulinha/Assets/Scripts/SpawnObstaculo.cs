using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnObstaculo : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject prefabObstaculo;

    [Header("DIFICULDADE - DISTÂNCIA E VELOCIDADE")]
    [Tooltip("Distância horizontal entre um obstáculo e outro.")]
    public float distanciaEntreObstaculos = 6f;

    [Tooltip("Velocidade inicial dos obstáculos.")]
    public float velocidadeObstaculos = 3f;

    [Header("DIFICULDADE - ABERTURA DOS OBSTÁCULOS")]
    [Tooltip("Espaço livre entre as duas barras.")]
    public float abertura = 2.5f;

    [Header("Limpeza")]
    private float velocidadeInicial;
    private Coroutine rotinaSpawn;

    private List<GameObject> obstaculosCriados = new List<GameObject>();


    void Awake()
    {
        velocidadeInicial = velocidadeObstaculos;
    }


    public void IniciarSpawn()
    {
        if (prefabObstaculo == null)
        {
            return;
        }

        if (rotinaSpawn == null)
        {
            rotinaSpawn = StartCoroutine(Spawnar());
        }
    }


    public void PararSpawn()
    {
        if (rotinaSpawn != null)
        {
            StopCoroutine(rotinaSpawn);
            rotinaSpawn = null;
        }
    }


    public void LimparObstaculos()
    {
        for (int i = 0; i < obstaculosCriados.Count; i++)
        {
            if (obstaculosCriados[i] != null)
            {
                Destroy(obstaculosCriados[i]);
            }
        }

        obstaculosCriados.Clear();
    }


    public void RestaurarVelocidade()
    {
        velocidadeObstaculos = velocidadeInicial;
    }


    IEnumerator Spawnar()
    {
        while (true)
        {
            Camera cam = Camera.main;

            if (cam != null)
            {
                float topo = cam.transform.position.y + cam.orthographicSize;
                float baixo = cam.transform.position.y - cam.orthographicSize;

                // Define a altura máxima e mínima para a abertura.
                float alturaMinima = baixo + abertura / 2f;
                float alturaMaxima = topo - abertura / 2f;

                float centroAbertura = Random.Range(alturaMinima, alturaMaxima);

                Vector3 posicao = new Vector3(transform.position.x, cam.transform.position.y, transform.position.z);

                GameObject novo = Instantiate(prefabObstaculo, posicao, Quaternion.identity);

                obstaculosCriados.Add(novo);

                Transform superior = novo.transform.Find("BarraSuperior");
                Transform inferior = novo.transform.Find("BarraInferior");

                Transform ponto = novo.transform.Find("Ponto");

                if (ponto != null)
                {
                    BoxCollider2D colliderPonto = ponto.GetComponent<BoxCollider2D>();

                    if (colliderPonto != null)
                    {
                        colliderPonto.size = new Vector2(0.2f, abertura);
                        colliderPonto.offset = Vector2.zero;
                    }

                    ponto.localPosition = new Vector3(0f, centroAbertura - posicao.y, 0f);
                }

                if (superior != null && inferior != null)
                {
                    AjustarBarra(superior, centroAbertura + abertura / 2f, topo, posicao.y);
                    AjustarBarra(inferior, baixo, centroAbertura - abertura / 2f, posicao.y);
                }

                MovimentoObstaculo movimento = novo.GetComponent<MovimentoObstaculo>();

                if (movimento != null)
                {
                    movimento.spawnar = this;
                }
            }

            float tempoEntreObstaculos = distanciaEntreObstaculos / velocidadeObstaculos;

            yield return new WaitForSeconds(tempoEntreObstaculos);
        }
    }


    void AjustarBarra(Transform barra, float limiteInferior, float limiteSuperior, float alturaRaiz)
    {
        SpriteRenderer sprite = barra.GetComponent<SpriteRenderer>();

        if (sprite == null || sprite.sprite == null)
        {
            return;
        }

        float altura = limiteSuperior - limiteInferior;

        Vector3 escala = barra.localScale;
        escala.y = altura / sprite.sprite.bounds.size.y;

        barra.localScale = escala;

        Vector3 posicao = barra.localPosition;
        posicao.y = (limiteInferior + limiteSuperior) / 2f - alturaRaiz;

        barra.localPosition = posicao;
    }


    public void AumentarVelocidade(float aumento)
    {
        velocidadeObstaculos += aumento;
    }
}