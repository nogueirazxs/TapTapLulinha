using UnityEngine;

public class PontoObstaculo : MonoBehaviour
{
    private bool jaContou;


    void OnTriggerEnter2D(Collider2D outro)
    {
        if (jaContou)
        {
            return;
        }

        if (!outro.CompareTag("Player") && !outro.transform.root.CompareTag("Player"))
        {
            return;
        }

        jaContou = true;

        if (gerenciador_tudo.instancia != null)
        {
            gerenciador_tudo.instancia.AdicionarPonto();
        }
    }
}