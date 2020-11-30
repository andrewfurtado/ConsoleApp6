using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp6
{
    class Program
    {
        private static Hotel hotel;
        public static Hotel instance
        {
            get
            {
                if (hotel == null)
                {
                    hotel = new Hotel();
                    hotel.Start();
                }
                return hotel;
            }
        }
        static void Main(string[] args)
        {
            int option = 0;
            do
            {
                Console.WriteLine("1-Cadastro");
                Console.WriteLine("2-Fechamento da Conta");
                Console.WriteLine("3-Relatorio");
                option = int.Parse(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        instance.CadastroReservaQuarto();
                        break;
                    case 2:
                        instance.FechamentaConta();
                        break;
                    case 3:
                        instance.Relatorio();
                        break;
                    default:
                        Console.WriteLine("Finalizado");
                        break;
                }
            } while (option != 0);
        }
    }
    abstract class CartaoCredito
    {
        public abstract float LimiteCartao { get; set; }
    }
    class MoneyBack : CartaoCredito
    {
        private float _limiteCredito;

        public MoneyBack(float creditLimit)
        {
            _limiteCredito = creditLimit;
        }

        public override float LimiteCartao
        {
            get { return _limiteCredito; }
            set { _limiteCredito = value; }
        }
    }
    abstract class CardFactory
    {
        public abstract CartaoCredito GetCreditCard();
    }
    class MoneyBackFactory : CardFactory
    {
        private float _limiteCredito;
        private MoneyBack moneyBack;
        public MoneyBackFactory(float creditLimit)
        {
            _limiteCredito = creditLimit;
        }
        public override CartaoCredito GetCreditCard()
        {
            return new MoneyBack(_limiteCredito);
        }
    }
    class Hotel
    {
        public Quarto[] quartos = new Quarto[20];
        public List<Cliente> clientesFechados = new List<Cliente>();
        bool flag = false;
        public void Start()
        {
            for (int i = 0; i < quartos.Length; i++)
            {
                quartos[i] = new Quarto();
            }
        }
        public void CadastroReservaQuarto()
        {
            string nome;
            DateTime dataNascimento;
            Console.WriteLine("Digite o nome do cliente:");
            nome = Console.ReadLine();
            Console.WriteLine("Digite a data de nascimento dd / MM / yyyy");
            string line = Console.ReadLine();
            DateTime.TryParse(line, out dataNascimento);
            Quarto quarto = null;
            if (flag)
            {
                quarto = Array.Find(quartos, item => item.cliente.nome == nome && item.cliente.dataNascimento == dataNascimento);
            }
            if (quarto == null)
            {
                Cliente cliente = new Cliente();
                cliente.CriarCliente(nome, dataNascimento);
                do
                {
                    Console.WriteLine("Digite o numero do quarto: 1 ao 20!");
                    int numQuarto = int.Parse(Console.ReadLine());
                    if (numQuarto < quartos.Length && numQuarto > 0)
                    {
                        if (quartos[numQuarto - 1].disponibilidade)
                        {
                            quarto = new Quarto();
                            quarto.cliente = cliente;
                            quarto.CriarQuarto();
                            quartos[numQuarto - 1] = quarto;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Quarto Indisponivel, por favor selecione outro quarto");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Numero de quarto invalido, digite outro novamente");
                    }
                } while (true);
                flag = true;
            }
            else
            {
                Console.WriteLine("Cliente ja cadastrado");
                quartos[Array.IndexOf(quartos, quarto)].CriarQuarto();
            }
        }
        public void FechamentaConta()
        {
            string nome;
            const float servicoHotel = 0.05f;
            DateTime dataNascimento;
            Console.WriteLine("Digite o nome do cliente:");
            nome = Console.ReadLine();
            Console.WriteLine("Digite a data de nascimento dd / MM / yyyy");
            string line = Console.ReadLine();
            DateTime.TryParse(line, out dataNascimento);
            Quarto quarto = null;
            if (flag)
            {
                quarto = Array.Find(quartos, item => item.cliente.nome == nome && item.cliente.dataNascimento == dataNascimento);
            }
            if (quarto != null)
            {
                if ((quartos[Array.IndexOf(quartos, quarto)].cliente.factory.GetCreditCard().LimiteCartao - quartos[Array.IndexOf(quartos, quarto)].getGastos() < 0))
                {
                    Console.WriteLine("Impossivel realizar tarefa, saldo insuficiente, entre com outros dados");
                }
                else
                {
                    quartos[Array.IndexOf(quartos, quarto)].cliente.factory.GetCreditCard().LimiteCartao -= quartos[Array.IndexOf(quartos, quarto)].getGastos();
                    clientesFechados.Add(quartos[Array.IndexOf(quartos, quarto)].cliente);
                    Console.WriteLine("Valor final:" + (quarto.alimentacao + quarto.gastoTelefone + quarto.diaria) + (quarto.alimentacao + quarto.gastoTelefone + quarto.diaria) * servicoHotel);
                    quartos[Array.IndexOf(quartos, quarto)] = new Quarto();
                }
            }
            else
            {
                Console.WriteLine("Usuario nao encontrado");
            }
        }
        public void Relatorio()
        {
            int cont = 0;
            foreach (var item in quartos)
            {
                if (item.disponibilidade != true)
                {
                    Console.WriteLine(item.ToString());
                    cont++;
                }
            }
            if (cont == 0)
            {
                Console.WriteLine("Nao existe quarto ocupado");
            }
        }
    }
    class Cliente
    {
        public Endereco endereco;
        public List<Telefone> telefones = new List<Telefone>();
        public string nome = "", rg;
        public DateTime dataNascimento = DateTime.Now;
        public CardFactory factory = null;
        public void CriarCliente(string nome, DateTime dataNascimento)
        {
            int qntTelefone;
            factory = new MoneyBackFactory(5000);
            this.dataNascimento = dataNascimento;
            this.nome = nome;
            Console.WriteLine("Digite o RG do cliente:");
            rg = Console.ReadLine();
            Console.WriteLine("Digite a quantidade de telefones que serao cadastrados:");
            qntTelefone = int.Parse(Console.ReadLine());
            for (int i = 0; i < qntTelefone; i++)
            {
                string ddd, telefone;
                Console.WriteLine("Digite o DDD:");
                ddd = Console.ReadLine();
                Console.WriteLine("Digite o numero de telefone");
                telefone = Console.ReadLine();
                Telefone tele = new Telefone(ddd, telefone);
                telefones.Add(tele);
            }
            string rua, bairro, cidade, estado, cep, numero;
            Console.WriteLine("Digite o nome da rua:");
            rua = Console.ReadLine();
            Console.WriteLine("Digite o nome do bairro:");
            bairro = Console.ReadLine();
            Console.WriteLine("Digite o nome da cidade:");
            cidade = Console.ReadLine();
            Console.WriteLine("Digite o nome do estado:");
            estado = Console.ReadLine();
            Console.WriteLine("Digite o CEP da rua:");
            cep = Console.ReadLine();
            Console.WriteLine("Digite o numero da casa:");
            numero = Console.ReadLine();
            endereco = new Endereco(rua, bairro, cidade, estado, cep, numero);
        }

        public override string ToString()
        {
            string tele = "";
            foreach (var item in telefones)
            {
                tele += "(" + item.ddd + ")" + item.telefone;
            }
            return "Nome:" + nome + "\n" +
                "Data Nascimento:" + dataNascimento.ToString("dd/MM/yyyy") + "\n" +
                "RG:" + rg +
                endereco.ToString() + "\n" +
                "Telefone" + tele + "\n" +
                endereco.ToString();
        }
    }
    class Endereco
    {
        public string rua, bairro, cidade, estado, cep, numero;

        public Endereco(string rua, string bairro, string cidade, string estado, string cep, string numero)
        {
            this.rua = rua;
            this.bairro = bairro;
            this.cidade = cidade;
            this.estado = estado;
            this.cep = cep;
            this.numero = numero;
        }

        public override string ToString()
        {
            return "Rua:" + rua + "Bairro" + bairro + "Cidade" + cidade + "Estado" + estado + "CEP" + cep + "Numero" + numero;
        }
    }
    class Telefone
    {
        public string ddd, telefone;

        public Telefone(string ddd, string telefone)
        {
            this.ddd = ddd;
            this.telefone = telefone;
        }
    }
    class Quarto
    {
        public enum TipoAcomodacao { Simples, Dupla, Tripla };
        public TipoAcomodacao acomodacao;
        public string numero, descricao;
        public DateTime diaEntrada = DateTime.Now, diaSaida = DateTime.Now;
        public float gastoTelefone, diaria, alimentacao;
        public bool disponibilidade = true;
        public Cliente cliente;
        public void CriarQuarto()
        {
            string line;
            Console.WriteLine("Selecione o tipo de acomodacao:");
            Console.WriteLine("1-Simples");
            Console.WriteLine("2-Dupla");
            Console.WriteLine("3-Tripla");
            line = Console.ReadLine();
            bool flag = false;
            while (!flag)
            {
                flag = true;
                switch (line)
                {
                    case "1":
                        acomodacao = TipoAcomodacao.Simples;
                        break;
                    case "2":
                        acomodacao = TipoAcomodacao.Dupla;
                        break;
                    case "3":
                        acomodacao = TipoAcomodacao.Tripla;
                        break;
                    default:
                        Console.WriteLine("Digite novamenteo o tipo de acomodacao:");
                        line = Console.ReadLine();
                        flag = false;
                        break;
                }
            }
            Console.WriteLine("Digite o numero do quarto:");
            numero = Console.ReadLine();
            Console.WriteLine("Digite a descricao do quarto:");
            descricao = Console.ReadLine();
            Console.WriteLine("Digite a data de Entrada DD/MM/YYYY:");
            string line1 = Console.ReadLine();
            DateTime.TryParse(line1, out diaEntrada);
            Console.WriteLine("Digite a data de Saida DD/MM/YYYY:");
            string line2 = Console.ReadLine();
            DateTime.TryParse(line2, out diaSaida);
            disponibilidade = false;
        }
        public float ControleGastosHospedes(float gastoTelefone, float diaria, float alimentacao)
        {
            this.gastoTelefone = gastoTelefone;
            this.diaria = diaria;
            this.alimentacao = alimentacao;
            return gastoTelefone + diaria + alimentacao;
        }
        public float getGastos()
        {
            return gastoTelefone + diaria + alimentacao;
        }
        public override string ToString()
        {

            return "Tipo de acomodacao:" + acomodacao.ToString("g") + "\n"
                + "Dia Entrada:" + diaEntrada.ToString("dd/MM/yyyy") + "\n"
                + "Dia Saida:" + diaSaida.ToString("dd/MM/yyyy") + "\n" +
                "Disponibilidade:" + (disponibilidade ? "Disponivel" : "Indisponivel") + "\n"
                + "Gasto diario:" + (gastoTelefone + diaria + alimentacao).ToString() + "\n"
                + "Cliente:" + cliente.ToString() + "\n";
        }
    }
}