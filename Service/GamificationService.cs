using System;
using TaskQuest.API.Models;

namespace TaskQuest.API.Services;

public class GamificationResult
{
    public int XpGanho { get; set; }
    public int XpTotalAtual { get; set; }
    public int NivelAtual { get; set; }
    public bool SubiuDeNivel { get; set; }
}

public class GamificationService
{
    // Calcula o XP de uma tarefa concluída conforme a documentação técnica
    public int CalcularXpTarefa(string prioridade, DateTime dataVencimento, DateTime concluidoEm)
    {
        const double xpBase = 100.0;

        // 1. Multiplicador de Prioridade
        double mPrioridade = prioridade.ToUpper() switch
        {
            "BAIXA" => 1.0,
            "MEDIA" => 1.2,
            "ALTA" => 1.5,
            _ => 1.0
        };

        // 2. Multiplicador de Prazo (50% de penalidade se entregue com atraso)
        double mPrazo = (concluidoEm <= dataVencimento) ? 1.0 : 0.5;

        // Cálculo Final
        double xpFinal = xpBase * mPrioridade * mPrazo;

        return (int)Math.Round(xpFinal);
    }

    // Calcula o XP necessário para o próximo nível: 100 * (Nivel ^ 1.5)
    public int CalcularXpParaProximoNivel(int nivelAtual)
    {
        return (int)Math.Round(100.0 * Math.Pow(nivelAtual, 1.5));
    }

    // Processa o ganho de XP do usuário e verifica se subiu de nível
    public GamificationResult ProcessarGanhoXp(Usuario usuario, int xpGanho)
    {
        int nivelAnterior = usuario.Nivel;
        
        usuario.XpTotal += xpGanho;

        // Verifica se o XP total atingiu a meta para subir de nível
        while (usuario.XpTotal >= CalcularXpParaProximoNivel(usuario.Nivel))
        {
            usuario.Nivel++;
        }

        bool subiuDeNivel = usuario.Nivel > nivelAnterior;

        return new GamificationResult
        {
            XpGanho = xpGanho,
            XpTotalAtual = usuario.XpTotal,
            NivelAtual = usuario.Nivel,
            SubiuDeNivel = subiuDeNivel
        };
    }
}
