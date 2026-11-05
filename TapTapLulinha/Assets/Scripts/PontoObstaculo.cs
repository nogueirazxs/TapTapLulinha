
using UnityEngine;

public class PontoObstaculo : MonoBehaviour
{
    private bool jaContou;

    void OnTriggerEnter2D(Collider2D outro)
    {
        // Só conta quando o jogador passar pelo sensor.
        if (jaContou || !outro.CompareTag("Player"))
            return;

        jaContou = true;

        if (gerenciador_tudo.instancia != null)
            gerenciador_tudo.instancia.AdicionarPonto();
    }
}
