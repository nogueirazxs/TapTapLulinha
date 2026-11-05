using UnityEngine;

public class ParallaxUnico : MonoBehaviour
{
    [Header("Configurações do Fundo")]
    [SerializeField] private Renderer fundoRenderer;

    [Header("Velocidade do Parallax")]
    [SerializeField] private Vector2 velocidadeOffset = new Vector2(0.02f, 0f);

    private Vector2 offsetAtual;


    void Start()
    {
        if (fundoRenderer == null)
        {
            fundoRenderer = GetComponent<Renderer>();
        }
    }


    void Update()
    {
        if (fundoRenderer == null)
        {
            return;
        }

        offsetAtual += velocidadeOffset * Time.deltaTime;

        fundoRenderer.material.mainTextureOffset = offsetAtual;
    }
}