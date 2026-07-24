using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.SemanticKernel;

namespace AgenteDeIA;

public class PedidoPlugin
{
    private static readonly Dictionary<string, (string Status, double Valor)> BancoDePedidos = new()
    {
        { "101", ("Entregue", 150.00) },
        { "102", ("Em transporte - Chega amanha", 89.90) },
        { "103", ("Aguardando Pagamento", 320.00) },
        { "104", ("Processando no estoque", 45.00) }
    };

    [KernelFunction, Description("Consulta o status e o valor de um pedido pelo seu ID.")]
    public string ConsultarPedido([Description("O codigo numerico do pedido, ex: 101, 102")] string idPedido)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n[LOG DO SISTEMA] Metodo C# 'ConsultarPedido' executado para ID: {idPedido}");
        Console.ResetColor();

        if (BancoDePedidos.TryGetValue(idPedido, out var pedido))
        {
            return $"Pedido #{idPedido}: Status = '{pedido.Status}', Valor = R$ {pedido.Valor:F2}";
        }

        return $"Pedido #{idPedido} nao foi encontrado no sistema.";
    }

    [KernelFunction, Description("Solicita o cancelamento de um pedido existente. ATENCAO: NUNCA chame esta funcao a menos que o usuario peça EXPLICITAMENTE para CANCELAR o pedido.")]
    public string CancelarPedido([Description("O ID do pedido a ser cancelado")] string idPedido)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[LOG DO SISTEMA] Metodo C# 'CancelarPedido' executado para ID: {idPedido}");
        Console.ResetColor();

        if (BancoDePedidos.ContainsKey(idPedido))
        {
            BancoDePedidos[idPedido] = ("Cancelado pelo cliente", BancoDePedidos[idPedido].Valor);
            return $"Sucesso: O pedido #{idPedido} foi cancelado no banco de dados.";
        }

        return $"Falha: Nao foi possivel cancelar o pedido #{idPedido} pois ele nao existe.";
    }

    [KernelFunction, Description("Calcula o valor do frete. ATENCAO: So execute esta funcao se o usuario ja tiver fornecido o numero do CEP.")]
    public string CalcularFrete(
        [Description("O numero do CEP com 8 digitos fornecido explicitamente pelo usuario. NUNCA invente um CEP.")] string cep)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n[LOG DO SISTEMA] Metodo C# 'CalcularFrete' executado para CEP: {cep}");
        Console.ResetColor();

        string cepApenasNumeros = new string(cep.Where(char.IsDigit).ToArray());

        if (cepApenasNumeros.Length != 8)
        {
            return "ERRO: O CEP fornecido e invalido. Peca educadamente ao usuario para informar um CEP valido com 8 numeros.";
        }

        if (cepApenasNumeros.StartsWith("0"))
        {
            return "Frete para Sao Paulo e Regiao Metropolitana: R$ 12,50 (Prazo: 2 dias uteis).";
        }

        return "Frete para demais regioes do Brasil: R$ 28,90 (Prazo: 5 a 8 dias uteis).";
    }
}