using UnityEngine;

public class ParallaxUnico : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private Renderer fundoRenderer;
    [SerializeField] private Vector2 velocidadeOffset = new Vector2(0.02f, 0f);

    private Vector2 offsetAtual;

    void Start()
    {
        if (fundoRenderer == null)
            fundoRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Atualiza o offset com base na velocidade e no tempo
        offsetAtual += velocidadeOffset * Time.deltaTime;

        // Aplica o deslocamento na textura do Material
        fundoRenderer.material.mainTextureOffset = offsetAtual;
    }
}