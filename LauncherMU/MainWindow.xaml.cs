using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using LauncherMU.Methods;
using LauncherMU.Models;
using System.Security.Cryptography;
using System.Text.Json;
using System.Net.Http;
using System.Data;
using Microsoft.Web.WebView2.Core;

namespace LauncherMU;

public partial class MainWindow : Window
{
    private static readonly HttpClient httpClientclient = new HttpClient();

    private string caminhoMain = System.AppDomain.CurrentDomain.BaseDirectory + "main.exe"; //Informação da localização do main
    private string serverUrl = "https://localhost/updates/list.json";//Informação do URL onde estará a pasta de update
    private string localDirectory = System.AppDomain.CurrentDomain.BaseDirectory;//Informação do diretório onde está o client
    private string urlLauncher = "https://localhost";//Informação da página que será carregada no navegador do launcher
    private string serverIp = "127.0.0.1";//Informação do IP do servidor para ping
    private int port = 55901;
    private string launcherExe = Process.GetCurrentProcess().MainModule.FileName;
    public MainWindow()
    {
        InitializeComponent();
        Browser();
        btnJogar.IsEnabled = false;
        btnConfiguracoes.IsEnabled = false;
        StatusServer();
        StartSync();
    }

    private async void Browser()
    {
        string runtimePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs");
        var env = await CoreWebView2Environment.CreateAsync(null, runtimePath); 
        await myWebBrowser.EnsureCoreWebView2Async(env);
        myWebBrowser.DefaultBackgroundColor = System.Drawing.Color.White;
        var url = new Uri("about:blank");
        myWebBrowser.Source = url;

        url = new Uri(urlLauncher);
        myWebBrowser.Source = url;  
    }

    private void StatusServer()
    {
        
        if (PingStatusServer.CheckServerPing(serverIp, port))
        {
            labelStatusServidor.Content = "Servidor online";
            labelStatusServidor.Foreground = new SolidColorBrush(Colors.Green);
        }
        else
        {
            labelStatusServidor.Content = "Servidor Offline";
            labelStatusServidor.Foreground = new SolidColorBrush(Colors.Red);
        }
    }

    private async void StartSync()
    {
        string fullPath = string.Empty;
        string directoryPath = string.Empty;
        string fileName = string.Empty;
        string localPath = string.Empty;
        int currentFile = 0;
        float progress = 0;

        labelAtualizacao.Content = "Obtendo lista de arquivos do servidor...";
        await Task.Delay(500);
        btnConfiguracoes.IsEnabled = true;
        try
        {
            // string dllPath = @"D:\Mu\Client\LauncherMU.dll";
            // byte[] data = File.ReadAllBytes(dllPath);
            // MessageBox.Show("Sucesso! O launcher conseguiu ler a DLL.");

            var serverFiles = await GetServerFileList();

            if (!Directory.Exists(localDirectory))
                Directory.CreateDirectory(localDirectory);


            foreach (var file in serverFiles)
            {
                fullPath = Path.Combine(localDirectory, file.Name);
                directoryPath = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }

            var serverFileNames = new HashSet<string>(serverFiles.Select(f => f.Name));
            var localFiles = Directory.GetFiles(localDirectory);

            int totalFiles = serverFiles.Length;

            // Verifica arquivos locais e remove os que não estão no servidor
            foreach (var localFilePath in localFiles)
            {
                currentFile++;
                progress = (currentFile / totalFiles) * 100;
                fileName = Path.GetFileName(localFilePath);
                if (localFilePath.Equals(launcherExe, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!serverFileNames.Contains(fileName))
                {
                    labelAtualizacao.Content = $"Removendo arquivo obsoleto: {fileName}";
                    labelAtualizacaoPorcentagem.Content = $"{progress.ToString("0.00")}%";
                    File.Delete(localFilePath);
                }
                progressBar.Value = progress;
            }
            
            // Baixa ou atualiza arquivos do servidor
            foreach (var file in serverFiles)
            {
                currentFile++;
                progress = ((float)currentFile / (float)totalFiles) * 100;
                localPath = Path.Combine(localDirectory, file.Name);

                if (!File.Exists(localPath) || !IsFileUpToDate(localPath, file.Hash))
                {
                    labelAtualizacao.Content = $"Baixando {file.Name}... ({currentFile}/{totalFiles})";
                    labelAtualizacaoPorcentagem.Content = $"{progress.ToString("0.00")}%";
                    await DownloadFile(file.Url, localPath);
                }
                progressBar.Value = progress;
            }
            
            labelAtualizacao.Content = "Sincronização concluída!";
            labelAtualizacaoPorcentagem.Content = "100%";
            progressBar.Value = 100;
            btnJogar.IsEnabled = true;
        }
        catch (Exception ex)
        {
            labelAtualizacao.Content = $"Erro: {ex.Message}";
        }
    }
    private async Task<ServerFile[]> GetServerFileList()
    {
        var response = await httpClientclient.GetStringAsync(serverUrl);
        return JsonSerializer.Deserialize<ServerFile[]>(response);
    }

    private bool IsFileUpToDate(string filePath, string serverHash)
    {
        var sha256 = SHA256.Create();
        var stream = File.OpenRead(filePath);
        byte[] hashBytes = sha256.ComputeHash(stream);
        string localHash = BitConverter.ToString(hashBytes).Replace("-", "");
        stream.Close();
        return localHash.Equals(serverHash, StringComparison.OrdinalIgnoreCase);
    }
    
    private async Task DownloadFile(string fileUrl, string destinationPath)
    {
        var response = await httpClientclient.GetAsync(fileUrl);
        response.EnsureSuccessStatusCode();

        var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await response.Content.CopyToAsync(fileStream);
    }


    private void btnJogar_Click(object sender, RoutedEventArgs e)
    {
        if (System.IO.File.Exists(caminhoMain))
        {
            try
            {
                Process process = Process.Start(caminhoMain);
                //string arguments = "/argumento1 /argumento2";
                //startProcess.StartInfo.Arguments = @" connect /u198.27.111.106 /p15987";
                //Process.Start(filePath, arguments);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao executar o programa: " + ex.Message);
            }
        }
        else
            MessageBox.Show("O programa main não foi encontrado.");
    }

    private void btnConfiguracoes_Click(object sender, RoutedEventArgs e)
    {
        Opcoes Dialog = new Opcoes();
        Dialog.ShowDialog();
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