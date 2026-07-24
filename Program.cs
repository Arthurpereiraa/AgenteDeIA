using System;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using AgenteDeIA;

string apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY")
                ?? "SUA_CHAVE_GROQ_AQUI";

if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains("SUA_CHAVE"))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("ERRO: Chave de API nao configurada.");
    Console.WriteLine("Configure a variavel de ambiente GROQ_API_KEY ou insira a chave no codigo.");
    Console.ResetColor();
    return;
}

try
{
    var builder = Kernel.CreateBuilder();

    builder.AddOpenAIChatCompletion(
        modelId: "llama-3.3-70b-versatile",
        apiKey: apiKey,
        endpoint: new Uri("https://api.groq.com/openai/v1")
    );

    builder.Plugins.AddFromType<PedidoPlugin>("PedidoPlugin");

    var kernel = builder.Build();
    var chatService = kernel.GetRequiredService<IChatCompletionService>();
    var history = new ChatHistory();

    history.AddSystemMessage("""
        Voce e o assistente virtual oficial da loja virtual.
        
        Diretrizes de Comportamento:
        1. Seja sempre cordial, profissional e conciso.
        2. Horario de atendimento: Segunda a Sexta, das 08:00 as 18:00.
        3. Politica de trocas: Ate 30 dias apos o recebimento.
        
        Regras para Chamadas de Ferramentas (MUITO IMPORTANTE):
        - NUNCA invente parametros para executar as funcoes.
        - NUNCA execute 'CancelarPedido' a menos que o usuario utilize a palavra 'cancelar' ou peca expressamente o cancelamento.
        - Alteracao de endereco ou CEP de pedidos JA REALIZADOS nao e suportada pelo sistema. Se o usuario solicitar isso, apenas informe educadamente que nao e possivel alterar o endereco de um pedido em andamento pelo chat.
        - So chame 'CalcularFrete' se o usuario fornecer um CEP de 8 digitos.
        """);

    var executionSettings = new OpenAIPromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };

    Console.WriteLine("==================================================");
    Console.WriteLine("Agente de IA Inicializado com Sucesso!");
    Console.WriteLine("Digite sua mensagem abaixo (ou 'sair' para fechar)");
    Console.WriteLine("==================================================\n");

    while (true)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Voce: ");
        string? inputUsuario = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(inputUsuario) || inputUsuario.Trim().ToLower() == "sair")
        {
            Console.WriteLine("\nAtendimento encerrado. Ate logo!");
            break;
        }

        history.AddUserMessage(inputUsuario);

        var resposta = await chatService.GetChatMessageContentAsync(
            history,
            executionSettings,
            kernel
        );

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nAgente: {resposta.Content}\n");

        history.AddMessage(resposta.Role, resposta.Content ?? string.Empty);
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nOcorreu um erro na execucao: {ex.Message}");
    Console.ResetColor();
}