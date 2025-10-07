using Microsoft.Azure.Cosmos;
using System.Net;
using Newtonsoft.Json;

class Program
{
    // Configurações do Azure Cosmos DB
    // Substitua pelas suas credenciais reais
    private static readonly string EndpointUri = "https://YOUR_ACCOUNT_NAME.documents.azure.com:443/";
    private static readonly string PrimaryKey = "YOUR_PRIMARY_KEY";
    private static readonly string DatabaseName = "SampleDB";
    private static readonly string ContainerName = "products";
    
    private static CosmosClient? cosmosClient;
    private static Database? database;
    private static Container? container;
    
    static async Task Main(string[] args)
    {
        Console.WriteLine("Azure Cosmos DB - Laboratório");
        Console.WriteLine("=============================");
        
        try
        {
            // Inicializa o cliente Cosmos DB
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            
            // Criar database e container se não existirem
            await CreateDatabaseAndContainerAsync();
            
            // Menu principal
            await ShowMainMenuAsync();
        }
        catch (CosmosException de)
        {
            Exception baseException = de.GetBaseException();
            Console.WriteLine($"Erro do Cosmos DB: {de.StatusCode} - {de.Message}. {baseException.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro: {e.Message}");
        }
        finally
        {
            if (cosmosClient != null)
            {
                cosmosClient.Dispose();
            }
        }
    }
    
    static async Task ShowMainMenuAsync()
    {
        while (true)
        {
            Console.WriteLine("\nSelecione uma opção:");
            Console.WriteLine("1 - Criar item no container");
            Console.WriteLine("2 - Ler item por ID");
            Console.WriteLine("3 - Consultar itens (Query)");
            Console.WriteLine("4 - Atualizar item (Upsert)");
            Console.WriteLine("5 - Excluir item");
            Console.WriteLine("6 - Listar todos os itens");
            Console.WriteLine("0 - Sair");
            Console.Write("\nOpção: ");
            
            var option = Console.ReadLine();
            
            try
            {
                switch (option)
                {
                    case "1":
                        await CreateItemAsync();
                        break;
                    case "2":
                        await ReadItemAsync();
                        break;
                    case "3":
                        await QueryItemsAsync();
                        break;
                    case "4":
                        await UpsertItemAsync();
                        break;
                    case "5":
                        await DeleteItemAsync();
                        break;
                    case "6":
                        await ListAllItemsAsync();
                        break;
                    case "0":
                        Console.WriteLine("Saindo...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        }
    }
    
    static async Task CreateDatabaseAndContainerAsync()
    {
        // Criar database se não existir
        database = await cosmosClient!.CreateDatabaseIfNotExistsAsync(DatabaseName);
        Console.WriteLine($"Database '{DatabaseName}' criado ou já existe.");
        
        // Criar container se não existir
        container = await database.CreateContainerIfNotExistsAsync(ContainerName, "/group");
        Console.WriteLine($"Container '{ContainerName}' criado ou já existe.");
    }
    
    static async Task CreateItemAsync()
    {
        Console.WriteLine("\n=== Criar Item ===");
        
        Console.Write("ID do produto: ");
        string id = Console.ReadLine() ?? Guid.NewGuid().ToString();
        
        Console.Write("Nome do produto: ");
        string name = Console.ReadLine() ?? "";
        
        Console.Write("Grupo (categoria): ");
        string group = Console.ReadLine() ?? "";
        
        Console.Write("É diet? (true/false): ");
        bool.TryParse(Console.ReadLine(), out bool diet);
        
        Console.Write("Preço: ");
        decimal.TryParse(Console.ReadLine(), out decimal price);
        
        Console.Write("Quantidade: ");
        int.TryParse(Console.ReadLine(), out int quantity);
        
        var product = new Product
        {
            id = id,
            name = name,
            group = group,
            diet = diet,
            price = price,
            quantity = quantity
        };
        
        try
        {
            ItemResponse<Product> response = await container!.CreateItemAsync(product, new PartitionKey(product.group));
            Console.WriteLine($"Item criado com sucesso! ID: {response.Resource.id}");
            Console.WriteLine($"RU consumidas: {response.RequestCharge}");
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            Console.WriteLine($"Item com ID '{id}' já existe!");
        }
    }
    
    static async Task ReadItemAsync()
    {
        Console.WriteLine("\n=== Ler Item por ID ===");
        
        Console.Write("ID do produto: ");
        string id = Console.ReadLine() ?? "";
        
        Console.Write("Grupo (partition key): ");
        string group = Console.ReadLine() ?? "";
        
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(group))
        {
            Console.WriteLine("ID e grupo são obrigatórios!");
            return;
        }
        
        try
        {
            ItemResponse<Product> response = await container!.ReadItemAsync<Product>(id, new PartitionKey(group));
            Product product = response.Resource;
            
            Console.WriteLine($"\nProduto encontrado:");
            Console.WriteLine($"ID: {product.id}");
            Console.WriteLine($"Nome: {product.name}");
            Console.WriteLine($"Grupo: {product.group}");
            Console.WriteLine($"Diet: {product.diet}");
            Console.WriteLine($"Preço: {product.price:C}");
            Console.WriteLine($"Quantidade: {product.quantity}");
            Console.WriteLine($"RU consumidas: {response.RequestCharge}");
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine("Item não encontrado!");
        }
    }
    
    static async Task QueryItemsAsync()
    {
        Console.WriteLine("\n=== Consultar Itens ===");
        Console.WriteLine("1 - Produtos não-diet");
        Console.WriteLine("2 - Produtos por grupo");
        Console.WriteLine("3 - Query personalizada");
        Console.Write("Opção: ");
        
        string queryOption = Console.ReadLine() ?? "";
        string sqlQuery = "";
        
        switch (queryOption)
        {
            case "1":
                sqlQuery = "SELECT * FROM products p WHERE p.diet = false";
                break;
            case "2":
                Console.Write("Nome do grupo: ");
                string group = Console.ReadLine() ?? "";
                sqlQuery = $"SELECT * FROM products p WHERE p.group = '{group}'";
                break;
            case "3":
                Console.Write("Digite a query SQL: ");
                sqlQuery = Console.ReadLine() ?? "";
                break;
            default:
                Console.WriteLine("Opção inválida!");
                return;
        }
        
        try
        {
            FeedIterator<Product> iterator = container!.GetItemQueryIterator<Product>(sqlQuery);
            Console.WriteLine($"\nExecutando query: {sqlQuery}");
            Console.WriteLine("Resultados:");
            
            double totalRU = 0;
            int count = 0;
            
            while (iterator.HasMoreResults)
            {
                FeedResponse<Product> response = await iterator.ReadNextAsync();
                totalRU += response.RequestCharge;
                
                foreach (Product product in response)
                {
                    count++;
                    Console.WriteLine($"{count}. {product.name} - {product.group} - {product.price:C}");
                }
            }
            
            Console.WriteLine($"\nTotal de itens encontrados: {count}");
            Console.WriteLine($"Total RU consumidas: {totalRU}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na consulta: {ex.Message}");
        }
    }
    
    static async Task UpsertItemAsync()
    {
        Console.WriteLine("\n=== Atualizar/Inserir Item (Upsert) ===");
        
        Console.Write("ID do produto: ");
        string id = Console.ReadLine() ?? "";
        
        Console.Write("Nome do produto: ");
        string name = Console.ReadLine() ?? "";
        
        Console.Write("Grupo (categoria): ");
        string group = Console.ReadLine() ?? "";
        
        Console.Write("É diet? (true/false): ");
        bool.TryParse(Console.ReadLine(), out bool diet);
        
        Console.Write("Preço: ");
        decimal.TryParse(Console.ReadLine(), out decimal price);
        
        Console.Write("Quantidade: ");
        int.TryParse(Console.ReadLine(), out int quantity);
        
        var product = new Product
        {
            id = id,
            name = name,
            group = group,
            diet = diet,
            price = price,
            quantity = quantity
        };
        
        try
        {
            ItemResponse<Product> response = await container!.UpsertItemAsync(product, new PartitionKey(product.group));
            Console.WriteLine($"Item atualizado/inserido com sucesso! ID: {response.Resource.id}");
            Console.WriteLine($"RU consumidas: {response.RequestCharge}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no upsert: {ex.Message}");
        }
    }
    
    static async Task DeleteItemAsync()
    {
        Console.WriteLine("\n=== Excluir Item ===");
        
        Console.Write("ID do produto: ");
        string id = Console.ReadLine() ?? "";
        
        Console.Write("Grupo (partition key): ");
        string group = Console.ReadLine() ?? "";
        
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(group))
        {
            Console.WriteLine("ID e grupo são obrigatórios!");
            return;
        }
        
        Console.Write($"Tem certeza que deseja excluir o item '{id}'? (s/n): ");
        string confirmation = Console.ReadLine()?.ToLower() ?? "";
        
        if (confirmation != "s" && confirmation != "sim")
        {
            Console.WriteLine("Operação cancelada.");
            return;
        }
        
        try
        {
            ItemResponse<Product> response = await container!.DeleteItemAsync<Product>(id, new PartitionKey(group));
            Console.WriteLine("Item excluído com sucesso!");
            Console.WriteLine($"RU consumidas: {response.RequestCharge}");
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine("Item não encontrado!");
        }
    }
    
    static async Task ListAllItemsAsync()
    {
        Console.WriteLine("\n=== Listar Todos os Itens ===");
        
        try
        {
            FeedIterator<Product> iterator = container!.GetItemQueryIterator<Product>("SELECT * FROM products");
            
            double totalRU = 0;
            int count = 0;
            
            while (iterator.HasMoreResults)
            {
                FeedResponse<Product> response = await iterator.ReadNextAsync();
                totalRU += response.RequestCharge;
                
                foreach (Product product in response)
                {
                    count++;
                    Console.WriteLine($"{count}. ID: {product.id} | Nome: {product.name} | Grupo: {product.group} | Preço: {product.price:C} | Quantidade: {product.quantity}");
                }
            }
            
            if (count == 0)
            {
                Console.WriteLine("Nenhum item encontrado no container.");
            }
            else
            {
                Console.WriteLine($"\nTotal de itens: {count}");
                Console.WriteLine($"Total RU consumidas: {totalRU}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao listar itens: {ex.Message}");
        }
    }
}

// Classe Product para representar os itens no Cosmos DB
public class Product
{
    [JsonProperty(PropertyName = "id")]
    public string id { get; set; } = "";
    
    [JsonProperty(PropertyName = "name")]
    public string name { get; set; } = "";
    
    [JsonProperty(PropertyName = "group")]
    public string group { get; set; } = "";
    
    [JsonProperty(PropertyName = "diet")]
    public bool diet { get; set; }
    
    [JsonProperty(PropertyName = "price")]
    public decimal price { get; set; }
    
    [JsonProperty(PropertyName = "quantity")]
    public int quantity { get; set; }
    
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}

