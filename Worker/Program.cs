using System;
using System.Collections.Generic;
using Bogus;
using Bogus.Extensions.Brazil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Worker
{
    class Program
    {
        private const string Path = "C:/Users/leonardo.costa/source/repos/PB/PB.MarketPlace.Template/Worker/jsons";
        private const int NumRelationshipsWorker = 2;
        private const int NumInputWorker = 10;
        private const int NumInputCarga = 50;

        static void Main(string[] args)
        {
            GerarJsonCrud();
            GerarJsonWroker();
        }

        public static string GerarJson(object obj)
        {
            return JValue.Parse(JsonConvert.SerializeObject(obj)).ToString(Formatting.Indented);
        }

        public static void GerarJsonCrud()
        {
            //DEV 1 - INPUT CARGA - CRUD VENDEDOR
            var vendedores = new Faker<VendedorBaseJson>("pt_BR").CustomInstantiator(_faker => new VendedorBaseJson(_faker)).Generate(NumInputCarga);

            //DEV 2 - INPUT CARGA - CRUD GERENTE
            var gerentes = new Faker<VendedorBaseJson>("pt_BR").CustomInstantiator(_faker => new VendedorBaseJson(_faker)).Generate(NumInputCarga);

            //DEV 3 - INPUT CARGA - CRUD FORNECEDOR
            var fornecedores = new Faker<VendedorBaseJson>("pt_BR").CustomInstantiator(_faker => new VendedorBaseJson(_faker)).Generate(NumInputCarga);

            //DEV 4 - INPUT CARGA - CRUD CLIENTE
            var clientes = new Faker<VendedorBaseJson>("pt_BR").CustomInstantiator(_faker => new VendedorBaseJson(_faker)).Generate(NumInputCarga);

            System.IO.File.WriteAllText(Path + "/vendedoresInputCarga.json", GerarJson(vendedores));
            System.IO.File.WriteAllText(Path + "/gerentesInputCarga.json", GerarJson(gerentes));
            System.IO.File.WriteAllText(Path + "/fornecedoresInputCarga.json", GerarJson(fornecedores));
            System.IO.File.WriteAllText(Path + "/clientesInputCarga.json", GerarJson(clientes));
        }

        public static void GerarJsonWroker()
        {
            //DEV 1 - WORKER VENDEDOR 
            var vendWorker = new Faker<VendedorWorker>("pt_BR").CustomInstantiator(
                        _faker => new VendedorWorker(
                            fornecedores: new Faker<FornecedorBaseJson>("pt_BR").CustomInstantiator(_faker => new FornecedorBaseJson(_faker)).Generate(NumRelationshipsWorker),
                            _faker)).Generate(NumInputWorker);

            //DEV 2 - WORKER GERENTE 
            var gerWorker = new Faker<GerenteWorker>("pt_BR").CustomInstantiator(
                        _faker => new GerenteWorker(
                            vendedores: new Faker<VendedorBaseJson>("pt_BR").CustomInstantiator(_faker => new VendedorBaseJson(_faker)).Generate(NumRelationshipsWorker),
                            _faker)).Generate(NumInputWorker);

            //DEV 3 - WORKER FORNECEDOR 
            var fornWorker = new Faker<FornecedorWorker>("pt_BR").CustomInstantiator(
                        _faker => new FornecedorWorker(
                            gerentes: new Faker<GerenteBaseJson>("pt_BR").CustomInstantiator(_faker => new GerenteBaseJson(_faker)).Generate(NumRelationshipsWorker),
                            _faker)).Generate(NumInputWorker);

            //DEV 4 - WORKER CLIENTE 
            var cliWorker = new Faker<ClienteWorker>("pt_BR").CustomInstantiator(
                        _faker => new ClienteWorker(
                            vendedores: new Faker<VendedorBaseJson>("pt_BR").CustomInstantiator(_faker => new VendedorBaseJson(_faker)).Generate(NumRelationshipsWorker),
                            _faker)).Generate(NumInputWorker);


            System.IO.File.WriteAllText(Path + "/vendedoresWorkerInput.json", GerarJson(vendWorker));
            System.IO.File.WriteAllText(Path + "/gerentesWorkerInput.json", GerarJson(gerWorker));
            System.IO.File.WriteAllText(Path + "/fornecedoresWorkerInput.json", GerarJson(fornWorker));
            System.IO.File.WriteAllText(Path + "/clientesWorkerInput.json", GerarJson(cliWorker));
        }
    }

    #region Classes Base 
    class Base
    {
        public Base(string nome)
        {
            Id = Guid.NewGuid();
            DataInclusao = DateTime.Now;
            DataAlteracao = null;
            Nome = nome;
        }

        [JsonProperty(PropertyName = "id", Order = 0)]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "nome", Order = 1)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "dataInclusao", Order = 9997)]
        public DateTime DataInclusao { get; set; }

        [JsonProperty(PropertyName = "dataAlteracao", Order = 9998)]
        public DateTime? DataAlteracao { get; set; }
    }

    class FornecedorBaseJson : Base
    {
        public FornecedorBaseJson(Faker _faker) : base(_faker.Company.CompanyName())
        {
            DataAberturaEmpresa = _faker.Date.Between(DateTime.Now.AddMonths(25), DateTime.Now);
            ValorProduto = _faker.Finance.Amount(900, 3000, 2);
            NomeProduto = _faker.Commerce.Product();
        }

        [JsonProperty(PropertyName = "nomeProduto", Order = 2)]
        public string NomeProduto { get; set; }

        [JsonProperty(PropertyName = "dataAberturaEmpresa", Order = 3)]
        public DateTime DataAberturaEmpresa { get; set; }

        [JsonProperty(PropertyName = "valorProduto", Order = 4)]
        public decimal ValorProduto { get; set; }
    }

    class VendedorBaseJson : Base
    {
        public VendedorBaseJson(Faker _faker) : base(_faker.Person.FullName)
        {
            DataAdmissao = _faker.Date.Between(DateTime.Now.AddMonths(25), DateTime.Now);
            Salario = _faker.Finance.Amount(900, 3000, 2);
            Comissao = _faker.Finance.Amount(900, 3000, 2);
        }

        [JsonProperty(PropertyName = "dataAdmissao", Order = 2)]
        public DateTime DataAdmissao { get; set; }

        [JsonProperty(PropertyName = "salario", Order = 3)]
        public decimal Salario { get; set; }

        [JsonProperty(PropertyName = "comissao", Order = 4)]
        public decimal Comissao { get; set; }
    }

    class GerenteBaseJson : Base
    {
        public GerenteBaseJson(Faker _faker) : base(_faker.Person.FullName)
        {
            DataAdmissao = _faker.Date.Between(DateTime.Now.AddMonths(25), DateTime.Now);
            Salario = _faker.Finance.Amount(900, 3000, 2);
            Bonus = _faker.Finance.Amount(900, 3000, 2);
        }

        [JsonProperty(PropertyName = "dataAdmissao", Order = 2)]
        public DateTime DataAdmissao { get; set; }

        [JsonProperty(PropertyName = "salario", Order = 3)]
        public decimal Salario { get; set; }

        [JsonProperty(PropertyName = "bonus", Order = 4)]
        public decimal Bonus { get; set; }
    }

    class ClienteBaseJson : Base
    {
        public ClienteBaseJson(Faker _faker) : base(_faker.Person.FullName)
        {
            Cpf = _faker.Person.Cpf();
            DataVenda = DateTime.Now;
            ValorTotalVenda = _faker.Finance.Amount(900, 3000, 2);
        }

        [JsonProperty(PropertyName = "cpf", Order = 2)]
        public string Cpf { get; set; }

        [JsonProperty(PropertyName = "valorTotalVenda", Order = 3)]
        public decimal ValorTotalVenda { get; set; }

        [JsonProperty(PropertyName = "dataVenda", Order = 4)]
        public DateTime DataVenda { get; set; }
    }

    #endregion

    #region Classes de Relacionamento
    //CLASSES PARA GERAR O JSON APENAS COM O RELACIONAMENTO DE UMA CLASSE 
    class FornecedorWorker : FornecedorBaseJson
    {
        public FornecedorWorker(List<GerenteBaseJson> gerentes, Faker _faker) : base(_faker)
        {
            Gerentes = gerentes;
        }
        [JsonProperty(PropertyName = "gerentes", Order = 9999)]
        public List<GerenteBaseJson> Gerentes { get; set; }
    }

    class VendedorWorker : VendedorBaseJson
    {
        public VendedorWorker(List<FornecedorBaseJson> fornecedores, Faker _faker) : base(_faker)
        {
            Fornecedores = fornecedores;
        }
        [JsonProperty(PropertyName = "fornecedores", Order = 9999)]
        public List<FornecedorBaseJson> Fornecedores { get; set; }
    }

    class ClienteWorker : ClienteBaseJson
    {
        public ClienteWorker(List<VendedorBaseJson> vendedores, Faker _faker) : base(_faker)
        {
            Vendedores = vendedores;
        }
        [JsonProperty(PropertyName = "vendedores", Order = 9999)]
        public List<VendedorBaseJson> Vendedores { get; set; }
    }

    class GerenteWorker : GerenteBaseJson
    {
        public GerenteWorker(List<VendedorBaseJson> vendedores, Faker _faker) : base(_faker)
        {
            Vendedores = vendedores;
        }
        [JsonProperty(PropertyName = "vendedores", Order = 9999)]
        public List<VendedorBaseJson> Vendedores { get; set; }
    }
    #endregion
}