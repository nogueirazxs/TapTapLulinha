
using UnityEngine;
using System.Collections;

public class SpawnObstaculo : MonoBehaviour
{
    public GameObject prefabObstaculo;

    public float intervaloSpawn = 2f;
    public float abertura = 2.5f;
    public float margemVertical = 0.5f;

    public float velocidadeObstaculos = 3f;

    private Coroutine rotinaSpawn;

    public void IniciarSpawn()
    {
        if (prefabObstaculo == null)
            return;

        if (rotinaSpawn == null)
            rotinaSpawn = StartCoroutine(Spawnar());
    }

    public void PararSpawn()
    {
        if (rotinaSpawn != null)
        {
            StopCoroutine(rotinaSpawn);
            rotinaSpawn = null;
        }
    }

    IEnumerator Spawnar()
    {
        while (true)
        {
            Camera cam = Camera.main;

            if (cam != null)
            {
                // Descobre os limites verticais da câmera.
                float topo = cam.transform.position.y + cam.orthographicSize;
                float baixo = cam.transform.position.y - cam.orthographicSize;

                // Sorteia somente a altura da abertura.
                float alturaMinima = baixo + margemVertical + abertura / 2f;
                float alturaMaxima = topo - margemVertical - abertura / 2f;

                float centroAbertura = Random.Range(alturaMinima, alturaMaxima);

                // Cria o par na posição do spawner.
                Vector3 posicao = new Vector3(transform.position.x, cam.transform.position.y, transform.position.z);

                GameObject novo = Instantiate(prefabObstaculo, posicao, Quaternion.identity);

                // Procura as duas barras dentro do prefab.
                Transform superior = novo.transform.Find("BarraSuperior");
                Transform inferior = novo.transform.Find("BarraInferior");

                // Ajusta as barras para alcançar as extremidades.
                if (superior != null && inferior != null)
                {
                    AjustarBarra(superior, centroAbertura + abertura / 2f, topo, posicao.y);
                    AjustarBarra(inferior, baixo, centroAbertura - abertura / 2f, posicao.y);
                }

                // Conecta o obstáculo ao controle de velocidade.
                MovimentoObstaculo movimento = novo.GetComponent<MovimentoObstaculo>();

                if (movimento != null)
                    movimento.spawnar = this;
            }

            yield return new WaitForSeconds(intervaloSpawn);
        }
    }

    void AjustarBarra(Transform barra, float limiteInferior, float limiteSuperior, float alturaRaiz)
    {
        SpriteRenderer sprite = barra.GetComponent<SpriteRenderer>();

        if (sprite == null || sprite.sprite == null)
            return;

        // Calcula a altura necessária para alcançar o limite.
        float altura = limiteSuperior - limiteInferior;

        Vector3 escala = barra.localScale;
        escala.y = altura / sprite.sprite.bounds.size.y;
        barra.localScale = escala;

        // Posiciona o centro da barra no lugar correto.
        Vector3 posicao = barra.localPosition;
        posicao.y = (limiteInferior + limiteSuperior) / 2f - alturaRaiz;
        barra.localPosition = posicao;
    }

    public void AumentarVelocidade(float aumento)
    {
        // Aumenta a velocidade dos obstáculos.
        velocidadeObstaculos += aumento;
    }
}
