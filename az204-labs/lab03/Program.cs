using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;

class Program
{
    // Substitua pela sua connection string do Azure Storage Account
    private static readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=YOUR_ACCOUNT_NAME;AccountKey=YOUR_ACCOUNT_KEY;EndpointSuffix=core.windows.net";
    
    static async Task Main(string[] args)
    {
        Console.WriteLine("Azure Blob Storage - Laboratório 03");
        Console.WriteLine("===================================");
        
        while (true)
        {
            ShowMenu();
            var option = Console.ReadLine();
            
            try
            {
                switch (option)
                {
                    case "1":
                        await CreateContainerAsync();
                        break;
                    case "2":
                        await UploadBlobAsync();
                        break;
                    case "3":
                        await ListBlobsAsync();
                        break;
                    case "4":
                        await DownloadBlobAsync();
                        break;
                    case "5":
                        await DeleteContainerAsync();
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
    
    static void ShowMenu()
    {
        Console.WriteLine("\nSelecione uma opção:");
        Console.WriteLine("1 - Criar um contêiner");
        Console.WriteLine("2 - Carregar blob em um contêiner");
        Console.WriteLine("3 - Listar blobs em um contêiner");
        Console.WriteLine("4 - Baixar blob");
        Console.WriteLine("5 - Excluir um contêiner");
        Console.WriteLine("0 - Sair");
        Console.Write("\nOpção: ");
    }
    
    // Função para criar um contêiner
    static async Task CreateContainerAsync()
    {
        Console.Write("Digite o nome do contêiner: ");
        string containerName = Console.ReadLine()?.ToLower() ?? "";
        
        if (string.IsNullOrEmpty(containerName))
        {
            Console.WriteLine("Nome do contêiner não pode ser vazio!");
            return;
        }
        
        try
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            Console.WriteLine($"Contêiner '{containerName}' criado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar contêiner: {ex.Message}");
        }
    }
    
    // Função para carregar blobs em um contêiner
    static async Task UploadBlobAsync()
    {
        Console.Write("Digite o nome do contêiner: ");
        string containerName = Console.ReadLine()?.ToLower() ?? "";
        
        Console.Write("Digite o nome do blob: ");
        string blobName = Console.ReadLine() ?? "";
        
        Console.Write("Digite o conteúdo do blob: ");
        string content = Console.ReadLine() ?? "";
        
        if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
        {
            Console.WriteLine("Nome do contêiner e blob não podem ser vazios!");
            return;
        }
        
        try
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            // Verifica se o contêiner existe
            if (!await containerClient.ExistsAsync())
            {
                Console.WriteLine($"Contêiner '{containerName}' não existe!");
                return;
            }
            
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            
            // Converte o conteúdo para bytes
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }
            
            Console.WriteLine($"Blob '{blobName}' carregado com sucesso no contêiner '{containerName}'!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar blob: {ex.Message}");
        }
    }
    
    // Função para listar os blobs em um contêiner
    static async Task ListBlobsAsync()
    {
        Console.Write("Digite o nome do contêiner: ");
        string containerName = Console.ReadLine()?.ToLower() ?? "";
        
        if (string.IsNullOrEmpty(containerName))
        {
            Console.WriteLine("Nome do contêiner não pode ser vazio!");
            return;
        }
        
        try
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            // Verifica se o contêiner existe
            if (!await containerClient.ExistsAsync())
            {
                Console.WriteLine($"Contêiner '{containerName}' não existe!");
                return;
            }
            
            Console.WriteLine($"\nBlobs no contêiner '{containerName}':");
            Console.WriteLine("=====================================");
            
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine($"- Nome: {blobItem.Name}");
                Console.WriteLine($"  Tamanho: {blobItem.Properties.ContentLength} bytes");
                Console.WriteLine($"  Última modificação: {blobItem.Properties.LastModified}");
                Console.WriteLine($"  Tipo de conteúdo: {blobItem.Properties.ContentType}");
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao listar blobs: {ex.Message}");
        }
    }
    
    // Função para baixar blobs
    static async Task DownloadBlobAsync()
    {
        Console.Write("Digite o nome do contêiner: ");
        string containerName = Console.ReadLine()?.ToLower() ?? "";
        
        Console.Write("Digite o nome do blob: ");
        string blobName = Console.ReadLine() ?? "";
        
        if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
        {
            Console.WriteLine("Nome do contêiner e blob não podem ser vazios!");
            return;
        }
        
        try
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            
            // Verifica se o blob existe
            if (!await blobClient.ExistsAsync())
            {
                Console.WriteLine($"Blob '{blobName}' não existe no contêiner '{containerName}'!");
                return;
            }
            
            // Baixa o conteúdo do blob
            BlobDownloadInfo downloadInfo = await blobClient.DownloadAsync();
            
            using (StreamReader reader = new StreamReader(downloadInfo.Content))
            {
                string content = await reader.ReadToEndAsync();
                Console.WriteLine($"\nConteúdo do blob '{blobName}':");
                Console.WriteLine("==============================");
                Console.WriteLine(content);
            }
            
            // Opção para salvar em arquivo local
            Console.Write("\nDeseja salvar o blob em um arquivo local? (s/n): ");
            string saveFile = Console.ReadLine()?.ToLower() ?? "";
            
            if (saveFile == "s" || saveFile == "sim")
            {
                Console.Write("Digite o nome do arquivo local: ");
                string fileName = Console.ReadLine() ?? blobName;
                
                await blobClient.DownloadToAsync(fileName);
                Console.WriteLine($"Blob salvo como '{fileName}'!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao baixar blob: {ex.Message}");
        }
    }
    
    // Função para excluir um contêiner
    static async Task DeleteContainerAsync()
    {
        Console.Write("Digite o nome do contêiner a ser excluído: ");
        string containerName = Console.ReadLine()?.ToLower() ?? "";
        
        if (string.IsNullOrEmpty(containerName))
        {
            Console.WriteLine("Nome do contêiner não pode ser vazio!");
            return;
        }
        
        Console.Write($"Tem certeza que deseja excluir o contêiner '{containerName}' e todos os seus blobs? (s/n): ");
        string confirmation = Console.ReadLine()?.ToLower() ?? "";
        
        if (confirmation != "s" && confirmation != "sim")
        {
            Console.WriteLine("Operação cancelada.");
            return;
        }
        
        try
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            // Verifica se o contêiner existe
            if (!await containerClient.ExistsAsync())
            {
                Console.WriteLine($"Contêiner '{containerName}' não existe!");
                return;
            }
            
            await containerClient.DeleteAsync();
            Console.WriteLine($"Contêiner '{containerName}' excluído com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao excluir contêiner: {ex.Message}");
        }
    }
}
