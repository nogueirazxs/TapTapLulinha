using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gerenciador_tudo : MonoBehaviour
{
    [Header("Interface do Menu")]
    public GameObject fundo, bt_jogar, bt_sair, bt_personagens;
    public GameObject painel_personagens, textoVoto;

    [Header("Jogador")]
    public GameObject player;
    public SpriteRenderer spritePlayer;

    [Header("Personagens")]
    public Sprite[] personagens;
    public Button[] botoesPersonagens;

    [Header("Pontuação")]
    public TMP_Text votoEscrito;
    public static int voto13;

    [Header("Desbloqueio de Personagens")]
    public GameObject painelDesbloqueio;
    public Image imagemDesbloqueio;

    [Header("DIFICULDADE DO JOGO")]
    [Tooltip("Quantidade de pontos necessária para liberar cada personagem.")]
    public int pontosParaDesbloquear = 13;

    [Tooltip("Tempo que o aviso de personagem desbloqueado ficará na tela.")]
    public float tempoMostrarDesbloqueio = 3f;

    [Tooltip("Quanto a velocidade dos obstáculos aumenta a cada desbloqueio.")]
    public float aumentoVelocidade = 1f;

    [Header("Referências")]
    public SpawnObstaculo spawnar;

    private int personagemEscolhido = 0;
    private bool[] personagensDesbloqueados;

    private int proximoPersonagem = 1;
    private int proximoDesbloqueio;

    private bool mostrandoDesbloqueio;
    private bool fimDeJogo;

    private Rigidbody2D rbPlayer;
    private Vector3 posicaoInicialPlayer;

    public static gerenciador_tudo instancia;


    void Awake()
    {
        instancia = this;
        Time.timeScale = 1f;
    }


    void Start()
    {
        voto13 = 0;

        if (player != null)
        {
            rbPlayer = player.GetComponent<Rigidbody2D>();
            posicaoInicialPlayer = player.transform.position;
            player.SetActive(false);
        }

        if (textoVoto != null)
        {
            textoVoto.SetActive(false);
        }

        if (painel_personagens != null)
        {
            painel_personagens.SetActive(false);
        }

        if (painelDesbloqueio != null)
        {
            painelDesbloqueio.SetActive(false);
        }

        personagensDesbloqueados = new bool[personagens.Length];

        // Recupera os personagens salvos
        for (int i = 0; i < personagens.Length; i++)
        {
            int desbloqueado = PlayerPrefs.GetInt("personagemDesbloqueado_" + i, 0);

            if (i == 0)
            {
                desbloqueado = 1;
                PlayerPrefs.SetInt("personagemDesbloqueado_0", 1);
            }

            personagensDesbloqueados[i] = desbloqueado == 1;
        }

        PlayerPrefs.Save();

        // Procura o próximo personagem que ainda está bloqueado
        while (proximoPersonagem < personagens.Length && personagensDesbloqueados[proximoPersonagem])
        {
            proximoPersonagem++;
        }

        if (pontosParaDesbloquear < 1)
        {
            pontosParaDesbloquear = 1;
        }

        proximoDesbloqueio = proximoPersonagem * pontosParaDesbloquear;

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
        {
            votoEscrito.text = voto13.ToString();
        }
    }


    void AtualizarBotoesPersonagens()
    {
        if (botoesPersonagens == null || personagensDesbloqueados == null)
        {
            return;
        }

        for (int i = 0; i < botoesPersonagens.Length; i++)
        {
            if (botoesPersonagens[i] == null)
            {
                continue;
            }

            bool desbloqueado = false;

            if (i < personagensDesbloqueados.Length)
            {
                desbloqueado = personagensDesbloqueados[i];
            }

            botoesPersonagens[i].interactable = desbloqueado;

            ColorBlock cores = botoesPersonagens[i].colors;

            if (desbloqueado)
            {
                cores.normalColor = Color.white;
            }
            else
            {
                cores.normalColor = Color.gray;
            }

            botoesPersonagens[i].colors = cores;
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
        {
            return;
        }

        if (!personagensDesbloqueados[numero])
        {
            return;
        }

        personagemEscolhido = numero;
        spritePlayer.sprite = personagens[personagemEscolhido];

        VoltarMenu();
    }


    public void VoltarMenu()
    {
        if (painel_personagens != null)
        {
            painel_personagens.SetActive(false);
        }

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
        {
            painelDesbloqueio.SetActive(false);
        }

        if (painel_personagens != null)
        {
            painel_personagens.SetActive(false);
        }

        fundo.SetActive(false);
        bt_jogar.SetActive(false);
        bt_sair.SetActive(false);
        bt_personagens.SetActive(false);

        spritePlayer.sprite = personagens[personagemEscolhido];

        // Coloca o jogador na posição inicial.
        player.transform.position = posicaoInicialPlayer;

        if (rbPlayer != null)
        {
            rbPlayer.linearVelocity = Vector2.zero;
            rbPlayer.angularVelocity = 0f;
        }

        player.SetActive(true);
        textoVoto.SetActive(true);

        if (spawnar != null)
        {
            spawnar.LimparObstaculos();
            spawnar.RestaurarVelocidade();
            spawnar.IniciarSpawn();
        }

        AtualizarVotos();
    }


    public void AdicionarPonto()
    {
        if (fimDeJogo || mostrandoDesbloqueio)
        {
            return;
        }

        voto13++;

        AtualizarVotos();

        // Verifica se chegou a hora de liberar outro personagem
        if (proximoPersonagem < personagens.Length && voto13 >= proximoDesbloqueio)
        {
            int numero = proximoPersonagem;

            proximoPersonagem++;

            proximoDesbloqueio = proximoPersonagem * pontosParaDesbloquear;

            StartCoroutine(MostrarDesbloqueio(numero));
        }
    }


    IEnumerator MostrarDesbloqueio(int numero)
    {
        mostrandoDesbloqueio = true;

        personagensDesbloqueados[numero] = true;

        PlayerPrefs.SetInt("personagemDesbloqueado_" + numero, 1);
        PlayerPrefs.Save();

        AtualizarBotoesPersonagens();

        if (imagemDesbloqueio != null)
        {
            imagemDesbloqueio.sprite = personagens[numero];
        }

        if (painelDesbloqueio != null)
        {
            painelDesbloqueio.SetActive(true);
        }

        player.SetActive(false);

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(tempoMostrarDesbloqueio);

        if (painelDesbloqueio != null)
        {
            painelDesbloqueio.SetActive(false);
        }

        player.SetActive(true);

        if (spawnar != null)
        {
            spawnar.AumentarVelocidade(aumentoVelocidade);
        }

        Time.timeScale = 1f;
        mostrandoDesbloqueio = false;
    }


    public void PerderJogo()
    {
        if (fimDeJogo)
        {
            return;
        }

        fimDeJogo = true;

        Time.timeScale = 1f;

        if (spawnar != null)
        {
            spawnar.PararSpawn();
            spawnar.LimparObstaculos();
        }

        if (painelDesbloqueio != null)
        {
            painelDesbloqueio.SetActive(false);
        }

        if (player != null)
        {
            player.SetActive(false);
        }

        if (textoVoto != null)
        {
            textoVoto.SetActive(false);
        }

        VoltarMenu();
    }


    public void Sair()
    {
        Application.Quit();
    }
}