
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gerenciador_tudo : MonoBehaviour
{
    public GameObject fundo, bt_jogar, bt_sair, bt_personagens, player, textoVoto, painel_personagens;

    public Sprite[] personagens;
    public SpriteRenderer spritePlayer;

    private int personagemEscolhido = 0;

    public static int voto13;
    public TMP_Text votoEscrito;

    public SpawnObstaculo spawnar;

    [Header("Desbloqueio de personagens")]
    public GameObject painelDesbloqueio;
    public Image imagemDesbloqueio;
    public Button[] botoesPersonagens;

    public int pontosParaDesbloquear = 5;
    public float tempoMostrarDesbloqueio = 3f;
    public float aumentoVelocidade = 1f;

    private bool[] personagensDesbloqueados;
    private int proximoPersonagem = 1;
    private int proximoDesbloqueio;
    private bool mostrandoDesbloqueio;
    private bool fimDeJogo;

    public static gerenciador_tudo instancia;

    void Awake()
    {
        // Permite que outros scripts encontrem o gerenciador.
        instancia = this;

        // Garante que o jogo não comece pausado.
        Time.timeScale = 1f;
    }

    void Start()
    {
        voto13 = 0;

        player.SetActive(false);
        textoVoto.SetActive(false);
        painel_personagens.SetActive(false);

        if (painelDesbloqueio != null)
            painelDesbloqueio.SetActive(false);

        // Carrega quais personagens já foram desbloqueados.
        personagensDesbloqueados = new bool[personagens.Length];

        for (int i = 0; i < personagens.Length; i++)
        {
            personagensDesbloqueados[i] = PlayerPrefs.GetInt("personagemDesbloqueado_" + i, i == 0 ? 1 : 0) == 1;
        }

        // O primeiro personagem sempre fica disponível.
        if (personagens.Length > 0)
        {
            personagensDesbloqueados[0] = true;
            PlayerPrefs.SetInt("personagemDesbloqueado_0", 1);
        }

        // Procura o próximo personagem que ainda está bloqueado.
        while (proximoPersonagem < personagens.Length && personagensDesbloqueados[proximoPersonagem])
        {
            proximoPersonagem++;
        }

        proximoDesbloqueio = Mathf.Max(1, pontosParaDesbloquear);

        AtualizarBotoesPersonagens();
        EscolherPersonagem(0);
        AtualizarVotos();
    }

    void Update()
    {
        AtualizarVotos();
    }

    void AtualizarVotos()
    {
        if (votoEscrito != null)
            votoEscrito.text = voto13.ToString();
    }

    void AtualizarBotoesPersonagens()
    {
        // Desativa os botões dos personagens que ainda estão bloqueados.
        if (botoesPersonagens == null)
            return;

        for (int i = 0; i < botoesPersonagens.Length; i++)
        {
            if (botoesPersonagens[i] != null)
            {
                botoesPersonagens[i].interactable = i < personagensDesbloqueados.Length && personagensDesbloqueados[i];
            }
        }
    }

    public void AbrirPersonagens()
    {
        fundo.SetActive(false);
        bt_jogar.SetActive(false);
        bt_sair.SetActive(false);
        bt_personagens.SetActive(false);

        painel_personagens.SetActive(true);
    }

    public void EscolherPersonagem(int numero)
    {
        if (numero < 0 || numero >= personagens.Length)
            return;

        // Impede a escolha de um personagem bloqueado.
        if (personagensDesbloqueados != null && !personagensDesbloqueados[numero])
            return;

        personagemEscolhido = numero;
        spritePlayer.sprite = personagens[personagemEscolhido];

        VoltarMenu();
    }

    public void VoltarMenu()
    {
        painel_personagens.SetActive(false);

        fundo.SetActive(true);
        bt_jogar.SetActive(true);
        bt_sair.SetActive(true);
        bt_personagens.SetActive(true);
    }

    public void Jogar()
    {
        voto13 = 0;
        fimDeJogo = false;
        mostrandoDesbloqueio = false;

        Time.timeScale = 1f;

        if (painelDesbloqueio != null)
            painelDesbloqueio.SetActive(false);

        fundo.SetActive(false);
        bt_jogar.SetActive(false);
        bt_sair.SetActive(false);
        bt_personagens.SetActive(false);

        spritePlayer.sprite = personagens[personagemEscolhido];

        player.SetActive(true);
        textoVoto.SetActive(true);

        // Inicia o spawn dos obstáculos.
        if (spawnar != null)
            spawnar.IniciarSpawn();

        AtualizarVotos();
    }

    public void AdicionarPonto()
    {
        if (fimDeJogo || mostrandoDesbloqueio)
            return;

        voto13++;
        AtualizarVotos();

        // Verifica se chegou a hora de desbloquear outro personagem.
        if (proximoPersonagem < personagens.Length && voto13 >= proximoDesbloqueio)
        {
            int numero = proximoPersonagem;

            proximoPersonagem++;
            proximoDesbloqueio += Mathf.Max(1, pontosParaDesbloquear);

            StartCoroutine(MostrarDesbloqueio(numero));
        }
    }

    IEnumerator MostrarDesbloqueio(int numero)
    {
        mostrandoDesbloqueio = true;

        // Libera o novo personagem e salva o desbloqueio.
        personagensDesbloqueados[numero] = true;

        PlayerPrefs.SetInt("personagemDesbloqueado_" + numero, 1);
        PlayerPrefs.Save();

        AtualizarBotoesPersonagens();

        // Mostra a imagem do personagem desbloqueado.
        if (imagemDesbloqueio != null)
            imagemDesbloqueio.sprite = personagens[numero];

        if (painelDesbloqueio != null)
            painelDesbloqueio.SetActive(true);

        // Pausa o jogo enquanto a imagem aparece.
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(tempoMostrarDesbloqueio);

        // Esconde a imagem e aumenta a dificuldade.
        if (painelDesbloqueio != null)
            painelDesbloqueio.SetActive(false);

        if (spawnar != null)
            spawnar.AumentarVelocidade(aumentoVelocidade);

        Time.timeScale = 1f;
        mostrandoDesbloqueio = false;
    }

    public void ReiniciarCena()
    {
        if (fimDeJogo)
            return;

        fimDeJogo = true;

        // Restaura o tempo antes de recarregar a cena.
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Sair()
    {
        Application.Quit();
    }
}
