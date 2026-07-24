# Agente de Suporte e-Commerce com AI Tool Calling (.NET 9)

Este projeto demonstra a implementacao de um Agente de IA Autonomo focado em suporte de e-commerce, utilizando C# (.NET 9), Microsoft Semantic Kernel e a API da Groq (Llama 3.3).

O objetivo principal deste repositorio e demonstrar o conceito de Tool Calling (Function Calling): a capacidade de uma LLM (Large Language Model) decidir, em tempo real e de forma autonoma, invocar metodos C# nativos para consultar ou manipular o estado da aplicacao.

---

## Como Funciona a Arquitetura

Em vez de depender apenas da geracao de texto, a aplicacao integra o modelo de linguagem ao codigo C# atraves de Schemas JSON gerados automaticamente pelo Semantic Kernel.

```text
[ Usuario ] ----> [ Terminal C# ] ----> [ Groq / Llama 3.3 ]
                       |                         |
                       | <--- Requisita Funcao --|
                       v
            [ PedidoPlugin.cs ]
                       |
                       +---> Executa C# e retorna dados para a IA
```

1. Intencao: O usuario faz uma pergunta via terminal (ex: "Qual o status do pedido 102?").
2. Decisao: A LLM identifica que precisa de dados do sistema e solicita a execucao da funcao ConsultarPedido.
3. Execucao: O C# intercepta a requisicao, executa o metodo local e devolve a resposta estruturada para o modelo.
4. Resposta Final: A IA processa o retorno do C# e gera uma resposta natural e precisa para o usuario.

---

## Funcionalidades do Agente

- Consulta de Pedidos: A IA recupera status e valores de pedidos no sistema.
- Cancelamento Seguro: Permite cancelamento no banco de dados ficticio apenas com solicitacao explicita do cliente.
- Calculo de Frete e Prazos: Higieniza o CEP informado, valida os digitos e calcula frete com regras de negocio em C#.
- Respostas Institucionais: Responde horarios de atendimento e politicas de troca via instrucoes de sistema (System Prompt).
- Observabilidade em Tempo Real: Logs coloridos no console para identificar exatamente quando o codigo C# entra em acao.

---

## Engenharia Defensiva e Trava de Seguranca

Para evitar problemas comuns em agentes de IA (como alucinacao de parametros e execucoes indevidas), o projeto utiliza uma abordagem em tres camadas:

| Camada | Implementacao | Funcao |
| :--- | :--- | :--- |
| System Prompt | history.AddSystemMessage | Restringe comportamentos e proibe alteracoes de endereco por chat. |
| Atributos de Ferramenta | [Description(...)] | Orienta a LLM a nao inventar dados caso o usuario nao os tenha fornecido. |
| Validacao em C# | Where(char.IsDigit) | Trata e valida parametros recebidos antes do processamento logico. |

---

## Tecnologias Utilizadas

- Plataforma: .NET 9.0 (C# 13)
- Orquestrador de IA: Microsoft Semantic Kernel
- Provedor de LLM: Groq Cloud API (Modelo llama-3.3-70b-versatile)
- Conector: OpenAI Chat Completion SDK

---

## Estrutura do Projeto

```text
AgenteDeIA/
├── Program.cs          # Configuracao do Kernel, gerenciador do ChatHistory e loop do terminal
├── PedidoPlugin.cs     # Plugin C# contendo as ferramentas (KernelFunctions) expostas a IA
├── AgenteDeIA.csproj   # Dependencias do SDK e pacotes NuGet
└── README.md           # Documentacao do projeto
```

---

## Como Executar o Projeto

### Pre-requisitos

- .NET 9 SDK instalado.
- Uma chave de API gratuita cadastrada no Groq Console (console.groq.com).

### Passo a Passo

1. Clone este repositorio:
   ```bash
   git clone [https://github.com/arthurpereiraa/AgenteDeIA.git](https://github.com/arthurpereiraa/AgenteDeIA.git)
   cd AgenteDeIA
   ```

2. Configure a Chave de API:
   
   No Windows (CMD / PowerShell):
   ```cmd
   setx GROQ_API_KEY "sua_chave_aqui"
   ```
   No Linux / macOS:
   ```bash
   export GROQ_API_KEY="sua_chave_aqui"
   ```
   (Nota: Se preferir nao usar variavel de ambiente para testes locais, insira a chave diretamente na variavel apiKey no arquivo Program.cs).

3. Restaurar e Executar:
   ```bash
   dotnet run
   ```

---

## Exemplo de Uso no Console

```text
==================================================
Agente de IA Inicializado com Sucesso!
Digite sua mensagem abaixo (ou 'sair' para fechar)
==================================================

Voce: Qual o status do pedido 102?

[LOG DO SISTEMA] Metodo C# 'ConsultarPedido' executado para ID: 102

Agente: O seu pedido #102 esta em transporte com previsao de chegada para amanha. O valor total e R$ 89,90.

Voce: Qual o valor do frete para o CEP 05778-200?

[LOG DO SISTEMA] Metodo C# 'CalcularFrete' executado para CEP: 05778-200

Agente: O frete para o CEP 05778200 (Sao Paulo e Regiao Metropolitana) fica em R$ 12,50, com prazo de entrega de 2 dias uteis.
```

---

## Licenca

Este projeto e de uso livre para fins de estudo, portfolio e demonstracao tecnica.
