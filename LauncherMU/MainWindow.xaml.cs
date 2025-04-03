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
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;

namespace LauncherMU;

public partial class MainWindow : Window
{
    private static readonly HttpClient httpClientclient = new HttpClient();
    private string caminhoMain = @"D:\Mu\Client\main.exe"; //Informação da localização do main
    private string serverUrl = "http://localhost/update/list.json";//Informação do URL onde estará a pasta de update
    private string localDirectory = @"D:\Mu\Client";//Informação do diretório onde está o client
    private string urlLauncher = "http://localhost/";//Informação da página que será carregada no navegador do launcher
    string serverStatus = "127.0.0.1";//Informação do IP do servidor para ping

    public MainWindow()
    {
        InitializeComponent();
        Browser();
        btnJogar.IsEnabled = false;
        btnConfiguracoes.IsEnabled = false;
        StatusServer();
        StartSync();
    }

    private void Browser()
    {
        var url = new Uri("about:blank");
        myWebBrowser.Source = url;
        
        url = new Uri(urlLauncher);
        myWebBrowser.Source = url;
        myWebBrowser.DefaultBackgroundColor = System.Drawing.Color.White;
    }

    private void StatusServer()
    {
        if (PingStatusServer.CheckServerPing(serverStatus))
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

            var serverFileNames = new HashSet<string>(serverFiles.Select(f => f.Name));
            var localFiles = Directory.GetFiles(localDirectory);

            int totalFiles = serverFiles.Length + localFiles.Length;
            int currentFile = 0;

            // Verifica arquivos locais e remove os que não estão no servidor
            foreach (var localFilePath in localFiles)
            {
                currentFile++;
                var fileName = Path.GetFileName(localFilePath);
                if (!serverFileNames.Contains(fileName))
                {
                    labelAtualizacao.Content = $"Removendo arquivo obsoleto: {fileName}";
                    File.Delete(localFilePath);
                }
                progressBar.Value = (int)(currentFile / totalFiles * 100);
            }

            // Baixa ou atualiza arquivos do servidor
            string localPath = "";
            foreach (var file in serverFiles)
            {
                currentFile++;
                localPath = Path.Combine(localDirectory, file.Name);

                if (!File.Exists(localPath) || !IsFileUpToDate(localPath, file.Hash))
                {
                    labelAtualizacao.Content = $"Baixando {file.Name}... ({currentFile}/{totalFiles})";
                    await DownloadFile(file.Url, localPath);
                }
                progressBar.Value = (int)(currentFile / totalFiles * 100);
            }
            labelAtualizacao.Content = "Sincronização concluída!";
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