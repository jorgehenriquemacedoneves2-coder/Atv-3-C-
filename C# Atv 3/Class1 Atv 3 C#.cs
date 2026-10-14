using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// ============================================
// CELLTECH - SISTEMA DE ASSISTÊNCIA TÉCNICA
// Projeto POO - C#
// ============================================

// INTERFACE
public interface IOrcamento
{
    decimal CalcularValor();
}

// CLASSE PARA GERAR IDs
public static class GeradorId
{
    private static int contador = 0;

    public static int ProximoId()
    {
        contador++;
        return contador;
    }

    public static void DefinirAtual(int maiorId)
    {
        if (maiorId > contador)
            contador = maiorId;
    }
}

// CLASSE BASE: PESSOA
public abstract class Pessoa
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Telefone { get; set; } = "";

    protected Pessoa()
    {
    }

    protected Pessoa(int id, string nome, string telefone)
    {
        this.Id = id;
        this.Nome = nome;
        this.Telefone = telefone;
    }

    public virtual string ExibirDados()
    {
        return $"ID: {Id} | Nome: {Nome} | Telefone: {Telefone}";
    }
}

// CLASSE CLIENTE: HERDA DE PESSOA
public class Cliente : Pessoa
{
    public string Email { get; set; } = "";

    public Cliente()
    {
    }

    public Cliente(int id, string nome, string telefone, string email)
        : base(id, nome, telefone)
    {
        this.Email = email;
    }

    public override string ExibirDados()
    {
        return $"CLIENTE | {base.ExibirDados()} | E-mail: {Email}";
    }
}

// CLASSE TÉCNICO: HERDA DE PESSOA
public class Tecnico : Pessoa
{
    public string Especialidade { get; set; } = "";

    public Tecnico()
    {
    }

    public Tecnico(int id, string nome, string telefone, string especialidade)
        : base(id, nome, telefone)
    {
        this.Especialidade = especialidade;
    }

    public override string ExibirDados()
    {
        return $"TÉCNICO | {base.ExibirDados()} | Especialidade: {Especialidade}";
    }
}

// CLASSE EQUIPAMENTO
public class Equipamento
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Marca { get; set; } = "";
    public string Modelo { get; set; } = "";
    public string Problema { get; set; } = "";

    public Equipamento()
    {
    }

    public Equipamento(int id, int clienteId, string marca,
        string modelo, string problema)
    {
        this.Id = id;
        this.ClienteId = clienteId;
        this.Marca = marca;
        this.Modelo = modelo;
        this.Problema = problema;
    }

    public string ExibirDados()
    {
        return $"ID: {Id} | Cliente: {ClienteId} | " +
               $"Celular: {Marca} {Modelo} | Problema: {Problema}";
    }
}

// CLASSE ORDEM DE SERVIÇO
public class OrdemServico : IOrcamento
{
    private decimal valorPecas;
    private decimal valorMaoDeObra;

    public int Id { get; set; }
    public int EquipamentoId { get; set; }
    public int TecnicoId { get; set; }
    public string Descricao { get; set; } = "";
    public string Status { get; set; } = "Aberta";
    public DateTime DataAbertura { get; set; } = DateTime.Today;
    public DateTime? DataGarantiaFim { get; set; }

    // Encapsulamento: impede valores negativos.
    public decimal ValorPecas
    {
        get { return valorPecas; }
        set
        {
            if (value < 0)
                throw new ArgumentException("O valor das peças não pode ser negativo.");

            valorPecas = value;
        }
    }

    public decimal ValorMaoDeObra
    {
        get { return valorMaoDeObra; }
        set
        {
            if (value < 0)
                throw new ArgumentException("O valor da mão de obra não pode ser negativo.");

            valorMaoDeObra = value;
        }
    }

    public OrdemServico()
    {
    }

    public OrdemServico(int id, int equipamentoId, int tecnicoId,
        string descricao, decimal pecas, decimal maoDeObra)
    {
        this.Id = id;
        this.EquipamentoId = equipamentoId;
        this.TecnicoId = tecnicoId;
        this.Descricao = descricao;
        this.ValorPecas = pecas;
        this.ValorMaoDeObra = maoDeObra;
        this.DataAbertura = DateTime.Today;
        this.Status = "Aberta";
    }

    // Implementação da interface.
    public decimal CalcularValor()
    {
        return ValorPecas + ValorMaoDeObra;
    }

    public string ExibirDados()
    {
        string garantia = DataGarantiaFim.HasValue
            ? DataGarantiaFim.Value.ToString("dd/MM/yyyy")
            : "Não definida";

        return $"OS: {Id} | Celular ID: {EquipamentoId} | " +
               $"Técnico ID: {TecnicoId}\n" +
               $"Problema: {Descricao}\n" +
               $"Peças: R$ {ValorPecas:F2} | " +
               $"Mão de obra: R$ {ValorMaoDeObra:F2} | " +
               $"Total: R$ {CalcularValor():F2}\n" +
               $"Status: {Status} | Abertura: {DataAbertura:dd/MM/yyyy} | " +
               $"Garantia até: {garantia}";
    }
}

// CLASSE PARA ARMAZENAR OS DADOS DO SISTEMA
public class DadosSistema
{
    public List<Cliente> Clientes { get; set; } = new List<Cliente>();
    public List<Tecnico> Tecnicos { get; set; } = new List<Tecnico>();
    public List<Equipamento> Equipamentos { get; set; } = new List<Equipamento>();
    public List<OrdemServico> Ordens { get; set; } = new List<OrdemServico>();
}

// ============================================
// PROGRAMA PRINCIPAL
// ============================================
class Program
{
    static DadosSistema dados = new DadosSistema();

    static string arquivo = "celltech_dados.json";

    static void Main(string[] args)
    {
        int opcao = -1;

        while (opcao != 0)
        {
            Console.Clear();

            Console.WriteLine("==================================");
            Console.WriteLine("       CELLTECH - SISTEMA");
            Console.WriteLine("     ASSISTÊNCIA DE CELULARES");
            Console.WriteLine("==================================");
            Console.WriteLine("1 - Cadastrar cliente");
            Console.WriteLine("2 - Cadastrar técnico");
            Console.WriteLine("3 - Cadastrar celular");
            Console.WriteLine("4 - Abrir ordem de serviço");
            Console.WriteLine("5 - Listar clientes e técnicos");
            Console.WriteLine("6 - Listar celulares");
            Console.WriteLine("7 - Listar ordens de serviço");
            Console.WriteLine("8 - Pesquisar cliente");
            Console.WriteLine("9 - Excluir cliente");
            Console.WriteLine("10 - Concluir serviço e registrar garantia");
            Console.WriteLine("11 - Relatório financeiro");
            Console.WriteLine("12 - Salvar dados");
            Console.WriteLine("13 - Carregar dados");
            Console.WriteLine("0 - Sair");
            Console.WriteLine("==================================");

            opcao = LerInteiro("Escolha uma opção: ");

            try
            {
                switch (opcao)
                {
                    case 1:
                        CadastrarCliente();
                        break;

                    case 2:
                        CadastrarTecnico();
                        break;

                    case 3:
                        CadastrarEquipamento();
                        break;

                    case 4:
                        AbrirOrdemServico();
                        break;

                    case 5:
                        ListarPessoas();
                        break;

                    case 6:
                        ListarEquipamentos();
                        break;

                    case 7:
                        ListarOrdens();
                        break;

                    case 8:
                        PesquisarCliente();
                        break;

                    case 9:
                        ExcluirCliente();
                        break;

                    case 10:
                        ConcluirServico();
                        break;

                    case 11:
                        RelatorioFinanceiro();
                        break;

                    case 12:
                        SalvarDados();
                        break;

                    case 13:
                        CarregarDados();
                        break;

                    case 0:
                        SalvarDados();
                        Console.WriteLine("Sistema encerrado!");
                        break;

                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nOcorreu um erro: " + ex.Message);
            }

            if (opcao != 0)
            {
                Console.WriteLine("\nPressione ENTER para continuar...");
                Console.ReadLine();
            }
        }
    }

    // MÉTODOS DE ENTRADA DE DADOS

    static string LerTexto(string mensagem)
    {
        string texto;

        do
        {
            Console.Write(mensagem);
            texto = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(texto))
                Console.WriteLine("O campo não pode ficar vazio.");

        } while (string.IsNullOrWhiteSpace(texto));

        return texto.Trim();
    }

    static int LerInteiro(string mensagem)
    {
        int valor;

        while (true)
        {
            Console.Write(mensagem);

            if (int.TryParse(Console.ReadLine(), out valor))
                return valor;

            Console.WriteLine("Digite um número inteiro válido.");
        }
    }

    static decimal LerDecimal(string mensagem)
    {
        decimal valor;

        while (true)
        {
            Console.Write(mensagem);

            if (decimal.TryParse(Console.ReadLine(), out valor) && valor >= 0)
                return valor;

            Console.WriteLine("Digite um valor válido, maior ou igual a zero.");
        }
    }

    // 1 - CADASTRAR CLIENTE

    static void CadastrarCliente()
    {
        Console.WriteLine("\n--- CADASTRO DE CLIENTE ---");

        string nome = LerTexto("Nome: ");
        string telefone = LerTexto("Telefone: ");
        string email = LerTexto("E-mail: ");

        Cliente cliente = new Cliente(
            GeradorId.ProximoId(),
            nome,
            telefone,
            email
        );

        dados.Clientes.Add(cliente);

        Console.WriteLine("Cliente cadastrado com sucesso!");
        Console.WriteLine(cliente.ExibirDados());
    }

    // 2 - CADASTRAR TÉCNICO

    static void CadastrarTecnico()
    {
        Console.WriteLine("\n--- CADASTRO DE TÉCNICO ---");

        string nome = LerTexto("Nome: ");
        string telefone = LerTexto("Telefone: ");
        string especialidade = LerTexto("Especialidade: ");

        Tecnico tecnico = new Tecnico(
            GeradorId.ProximoId(),
            nome,
            telefone,
            especialidade
        );

        dados.Tecnicos.Add(tecnico);

        Console.WriteLine("Técnico cadastrado com sucesso!");
        Console.WriteLine(tecnico.ExibirDados());
    }

    // 3 - CADASTRAR CELULAR

    static void CadastrarEquipamento()
    {
        Console.WriteLine("\n--- CADASTRO DE CELULAR ---");

        if (dados.Clientes.Count == 0)
        {
            Console.WriteLine("Cadastre um cliente antes de cadastrar um celular.");
            return;
        }

        Console.WriteLine("\nClientes cadastrados:");

        foreach (Cliente cliente in dados.Clientes)
        {
            Console.WriteLine($"ID: {cliente.Id} | Nome: {cliente.Nome}");
        }

        int clienteId = LerInteiro("ID do cliente: ");

        Cliente? clienteEncontrado =
            dados.Clientes.FirstOrDefault(c => c.Id == clienteId);

        if (clienteEncontrado == null)
        {
            Console.WriteLine("Cliente não encontrado.");
            return;
        }

        string marca = LerTexto("Marca do celular: ");
        string modelo = LerTexto("Modelo: ");
        string problema = LerTexto("Problema apresentado: ");

        Equipamento equipamento = new Equipamento(
            GeradorId.ProximoId(),
            clienteId,
            marca,
            modelo,
            problema
        );

        dados.Equipamentos.Add(equipamento);

        Console.WriteLine("Celular cadastrado com sucesso!");
    }

    // 4 - ABRIR ORDEM DE SERVIÇO

    static void AbrirOrdemServico()
    {
        Console.WriteLine("\n--- ABRIR ORDEM DE SERVIÇO ---");

        if (dados.Equipamentos.Count == 0)
        {
            Console.WriteLine("Cadastre um celular primeiro.");
            return;
        }

        if (dados.Tecnicos.Count == 0)
        {
            Console.WriteLine("Cadastre um técnico primeiro.");
            return;
        }

        Console.WriteLine("\nCelulares cadastrados:");

        foreach (Equipamento equipamento in dados.Equipamentos)
        {
            Console.WriteLine(equipamento.ExibirDados());
        }

        int equipamentoId = LerInteiro("\nID do celular: ");

        Equipamento? equipamentoEncontrado =
            dados.Equipamentos.FirstOrDefault(e => e.Id == equipamentoId);

        if (equipamentoEncontrado == null)
        {
            Console.WriteLine("Celular não encontrado.");
            return;
        }

        Console.WriteLine("\nTécnicos cadastrados:");

        foreach (Tecnico tecnico in dados.Tecnicos)
        {
            Console.WriteLine($"ID: {tecnico.Id} | Nome: {tecnico.Nome}");
        }

        int tecnicoId = LerInteiro("ID do técnico: ");

        Tecnico? tecnicoEncontrado =
            dados.Tecnicos.FirstOrDefault(t => t.Id == tecnicoId);

        if (tecnicoEncontrado == null)
        {
            Console.WriteLine("Técnico não encontrado.");
            return;
        }

        string descricao = LerTexto("Descrição do serviço: ");
        decimal pecas = LerDecimal("Valor das peças: R$ ");
        decimal maoDeObra = LerDecimal("Valor da mão de obra: R$ ");

        OrdemServico ordem = new OrdemServico(
            GeradorId.ProximoId(),
            equipamentoId,
            tecnicoId,
            descricao,
            pecas,
            maoDeObra
        );

        dados.Ordens.Add(ordem);

        Console.WriteLine("\nOrdem de serviço criada!");
        Console.WriteLine(ordem.ExibirDados());
    }

    // 5 - LISTAR PESSOAS
    // Demonstra polimorfismo com a classe Pessoa.

    static void ListarPessoas()
    {
        Console.WriteLine("\n--- CLIENTES E TÉCNICOS ---");

        List<Pessoa> pessoas = new List<Pessoa>();

        pessoas.AddRange(dados.Clientes);
        pessoas.AddRange(dados.Tecnicos);

        if (pessoas.Count == 0)
        {
            Console.WriteLine("Nenhuma pessoa cadastrada.");
            return;
        }

        foreach (Pessoa pessoa in pessoas)
        {
            Console.WriteLine(pessoa.ExibirDados());
        }
    }

    // 6 - LISTAR CELULARES

    static void ListarEquipamentos()
    {
        Console.WriteLine("\n--- CELULARES CADASTRADOS ---");

        if (dados.Equipamentos.Count == 0)
        {
            Console.WriteLine("Nenhum celular cadastrado.");
            return;
        }

        foreach (Equipamento equipamento in dados.Equipamentos)
        {
            Cliente? cliente = dados.Clientes.FirstOrDefault(
                c => c.Id == equipamento.ClienteId
            );

            Console.WriteLine(equipamento.ExibirDados());
            Console.WriteLine("Nome do cliente: " +
                (cliente != null ? cliente.Nome : "Não encontrado"));

            Console.WriteLine("----------------------------------");
        }
    }

    // 7 - LISTAR ORDENS DE SERVIÇO

    static void ListarOrdens()
    {
        Console.WriteLine("\n--- ORDENS DE SERVIÇO ---");

        if (dados.Ordens.Count == 0)
        {
            Console.WriteLine("Nenhuma ordem de serviço cadastrada.");
            return;
        }

        foreach (OrdemServico ordem in dados.Ordens)
        {
            Console.WriteLine(ordem.ExibirDados());
            Console.WriteLine("----------------------------------");
        }
    }

    // 8 - PESQUISAR CLIENTE

    static void PesquisarCliente()
    {
        Console.WriteLine("\n--- PESQUISAR CLIENTE ---");

        int id = LerInteiro("Digite o ID do cliente: ");

        Cliente? cliente = dados.Clientes.FirstOrDefault(
            c => c.Id == id
        );

        if (cliente == null)
        {
            Console.WriteLine("Cliente não encontrado.");
            return;
        }

        Console.WriteLine("\nCliente encontrado:");
        Console.WriteLine(cliente.ExibirDados());

        Console.WriteLine("\nCelulares desse cliente:");

        List<Equipamento> equipamentosCliente =
            dados.Equipamentos.Where(e => e.ClienteId == id).ToList();

        if (equipamentosCliente.Count == 0)
        {
            Console.WriteLine("Nenhum celular cadastrado para esse cliente.");
        }
        else
        {
            foreach (Equipamento equipamento in equipamentosCliente)
            {
                Console.WriteLine(equipamento.ExibirDados());
            }
        }
    }

    // 9 - EXCLUIR CLIENTE

    static void ExcluirCliente()
    {
        Console.WriteLine("\n--- EXCLUIR CLIENTE ---");

        int id = LerInteiro("ID do cliente que deseja excluir: ");

        Cliente? cliente = dados.Clientes.FirstOrDefault(
            c => c.Id == id
        );

        if (cliente == null)
        {
            Console.WriteLine("Cliente não encontrado.");
            return;
        }

        bool possuiCelular = dados.Equipamentos.Any(
            e => e.ClienteId == id
        );

        if (possuiCelular)
        {
            Console.WriteLine(
                "Não é possível excluir: o cliente possui celulares cadastrados."
            );
            return;
        }

        dados.Clientes.Remove(cliente);

        Console.WriteLine("Cliente excluído com sucesso!");
    }

    // 10 - CONCLUIR SERVIÇO E REGISTRAR GARANTIA
    // Funcionalidade extra do projeto.

    static void ConcluirServico()
    {
        Console.WriteLine("\n--- CONCLUIR SERVIÇO ---");

        int id = LerInteiro("ID da ordem de serviço: ");

        OrdemServico? ordem = dados.Ordens.FirstOrDefault(
            o => o.Id == id
        );

        if (ordem == null)
        {
            Console.WriteLine("Ordem de serviço não encontrada.");
            return;
        }

        if (ordem.Status == "Concluída")
        {
            Console.WriteLine("Essa ordem de serviço já foi concluída.");
            return;
        }

        ordem.Status = "Concluída";

        // Regra de negócio: garantia de 90 dias após a conclusão.
        ordem.DataGarantiaFim = DateTime.Today.AddDays(90);

        Console.WriteLine("Serviço concluído com sucesso!");
        Console.WriteLine("Garantia registrada por 90 dias.");
        Console.WriteLine(ordem.ExibirDados());
    }

    // 11 - RELATÓRIO FINANCEIRO

    static void RelatorioFinanceiro()
    {
        Console.WriteLine("\n--- RELATÓRIO FINANCEIRO ---");

        int totalOrdens = dados.Ordens.Count;

        int ordensAbertas = dados.Ordens.Count(
            o => o.Status == "Aberta"
        );

        int ordensConcluidas = dados.Ordens.Count(
            o => o.Status == "Concluída"
        );

        decimal valorTotal = dados.Ordens.Sum(
            o => o.CalcularValor()
        );

        decimal valorConcluido = dados.Ordens
            .Where(o => o.Status == "Concluída")
            .Sum(o => o.CalcularValor());

        Console.WriteLine($"Total de clientes: {dados.Clientes.Count}");
        Console.WriteLine($"Total de técnicos: {dados.Tecnicos.Count}");
        Console.WriteLine($"Total de celulares: {dados.Equipamentos.Count}");
        Console.WriteLine($"Total de ordens: {totalOrdens}");
        Console.WriteLine($"Ordens abertas: {ordensAbertas}");
        Console.WriteLine($"Ordens concluídas: {ordensConcluidas}");
        Console.WriteLine($"Valor total dos serviços: R$ {valorTotal:F2}");
        Console.WriteLine($"Valor dos serviços concluídos: R$ {valorConcluido:F2}");
    }

    // 12 - SALVAR DADOS EM JSON

    static void SalvarDados()
    {
        try
        {
            JsonSerializerOptions opcoes = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(dados, opcoes);

            File.WriteAllText(arquivo, json);

            Console.WriteLine("Dados salvos com sucesso!");
            Console.WriteLine("Arquivo: " + arquivo);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao salvar os dados: " + ex.Message);
        }
    }

    // 13 - CARREGAR DADOS DO JSON

    static void CarregarDados()
    {
        try
        {
            if (!File.Exists(arquivo))
            {
                Console.WriteLine("Arquivo não encontrado. Salve os dados primeiro.");
                return;
            }

            string json = File.ReadAllText(arquivo);

            DadosSistema? dadosCarregados =
                JsonSerializer.Deserialize<DadosSistema>(json);

            if (dadosCarregados == null)
            {
                Console.WriteLine("Não foi possível carregar os dados.");
                return;
            }

            dados = dadosCarregados;

            // Evita repetir IDs depois de carregar o arquivo.
            int maiorId = 0;

            if (dados.Clientes.Count > 0)
                maiorId = Math.Max(maiorId, dados.Clientes.Max(c => c.Id));

            if (dados.Tecnicos.Count > 0)
                maiorId = Math.Max(maiorId, dados.Tecnicos.Max(t => t.Id));

            if (dados.Equipamentos.Count > 0)
                maiorId = Math.Max(maiorId, dados.Equipamentos.Max(e => e.Id));

            if (dados.Ordens.Count > 0)
                maiorId = Math.Max(maiorId, dados.Ordens.Max(o => o.Id));

            GeradorId.DefinirAtual(maiorId);

            Console.WriteLine("Dados carregados com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao carregar os dados: " + ex.Message);
        }
    }
}