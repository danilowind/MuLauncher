using System.Windows;
using ProjetoCheck.Models;
using System.IO;
using System.Text.Json;
using System.Windows.Threading;

namespace ProjetoCheck;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void btnGerarCheckSum_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string serverDirectory = textDirectory.Text;//  @"C:\xampp\htdocs\update";
            serverDirectory = @"C:\\xampp\\htdocs\\updates";
            string baseUrl = textUrl.Text;//"https://localhost/updates/";
            baseUrl = "https://localhost/updates/";

            if (string.IsNullOrWhiteSpace(baseUrl))
                MessageBox.Show("Por favor, insira uma URL válida");
            else
            {
                if (File.Exists(serverDirectory + "\\list.json"))
                    File.Delete(serverDirectory + "\\list.json");

                string outputFilePath = System.IO.Path.Combine(serverDirectory, "list.json");
                var files = Directory.GetFiles(serverDirectory, "*", SearchOption.AllDirectories);
                var fileList = new List<ServerFile>();
                int totalFiles = files.Length;
                int processedFiles = 0;
                foreach (var filePath in files)
                {
                    string relativePath = System.IO.Path.GetRelativePath(serverDirectory, filePath).Replace("\\", "/");
                    string fileUrl = $"{baseUrl}{relativePath}";
                    string hash = ProjetoCheck.Methods.FileHash.HashFile(filePath);

                    fileList.Add(new ServerFile{Name = relativePath, Url = fileUrl, Hash = hash});

                    processedFiles++;
                    double progress = (double)processedFiles / totalFiles * 100;
                    Dispatcher.Invoke(() =>
                    {
                        labelStatus.Content = $"Processando: {relativePath} ({processedFiles}/{totalFiles})";
                        progressBar.Value = progress;
                    }, DispatcherPriority.Background);
                }
                string json = JsonSerializer.Serialize(fileList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(outputFilePath, json);
                Dispatcher.Invoke(() =>
                {
                    labelStatus.Content = "100%";
                    progressBar.Value = 100;
                });
                MessageBox.Show("Foi gerado o list.json com o hash dos arquivos!");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ocorreu um erro: {ex.Message} \n Verifique se o caminho do diretório está correto");
        }
    }

    private void btnFechar_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnMinimizar_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }
}
