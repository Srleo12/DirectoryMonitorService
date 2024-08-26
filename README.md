# Serviço de Monitoramento de Diretório

<div>
<h2>Descrição<h3>
Este projeto é um serviço Windows desenvolvido em C# que monitora mudanças em um diretório específico. Ele registra eventos como criação, modificação, exclusão e renomeação de arquivos em um log de texto. Além disso, o serviço envia notificações de sistema e pode ser configurado para enviar alertas por e-mail quando ocorrem mudanças.
</div>

## Funcionalidades
- Monitoramento de um diretório específico para eventos de criação, modificação, exclusão e renomeação de arquivos.
- Registro de eventos em arquivos de log.
- Envio de notificações de sistema para alertar sobre mudanças.
- Suporte a envio de alertas por e-mail (configuração opcional).
- Integração com scripts Python para notificações customizadas.

## Requisitos
- .NET Framework: O projeto foi desenvolvido utilizando o .NET Framework, portanto, é necessário ter a versão correspondente instalada.
- Python: Para executar notificações de sistema via script Python, é necessário ter o Python instalado. O projeto foi testado com Python 3.11.
- Pacotes Python: O pacote win10toast deve estar instalado para enviar notificações.

## Instalação
1. Clonando o Repositório
- Primeiro, clone este repositório para o seu ambiente local:
```
git clone https://github.com/seu-usuario/seu-repositorio.git
cd seu-repositorio
```
2. Configurando o Projeto no Visual Studio
- Abra o projeto no Visual Studio.
- Certifique-se de que todas as dependências necessárias estão instaladas.
- Compile o projeto para verificar se não há erros.
3. Configurando o Serviço
- Edite o arquivo SystemMonit.cs para configurar o caminho do diretório a ser monitorado:
```csharp
string pathToWatch = @"C:\temp\NAS";
```
- Se desejar utilizar o recurso de envio de e-mails, configure as variáveis smtpServer, smtpUsername, smtpPassword e toEmail dentro do método SendEmailNotification.
4. Instalando o Serviço
- Para instalar o serviço, utilize o utilitário sc do Windows:

```bash
sc create NomeDoServico binPath= "caminho\para\o\arquivo.exe"
```
Substitua NomeDoServico pelo nome que você deseja dar ao serviço e caminho\para\o\arquivo.exe pelo caminho do executável compilado.

5. Instalando o Python e Dependências
Certifique-se de que o Python está instalado no sistema. Você pode baixar o Python <a href='https://www.python.org/downloads/'><button>aqui</button></a>.
- Instale o pacote win10toast utilizando o pip:

```bash
pip install win10toast
```
6. Configurando e Testando o Script Python
- Crie o script notificação.py no caminho desejado, com o seguinte conteúdo:

```py
from win10toast import ToastNotifier

toaster = ToastNotifier()
toaster.show_toast(
    "Notificação",
    "Serviço Sistema de Monitoramento iniciado!",
    threaded=True,
    icon_path=None,
    duration=2
)
```
- Teste o script executando-o diretamente no terminal:
```bash
python caminho\para\o\script\notificação.py
```
7. Executando o Serviço
- Inicie o serviço utilizando o utilitário de serviços do Windows ou o comando sc:
```bash
sc start NomeDoServico
```
## Logs e Notificações
Os logs dos eventos monitorados serão armazenados em arquivos de texto na pasta Logs dentro do diretório onde o serviço está instalado. As notificações de sistema aparecerão como pop-ups no sistema operacional.

## Problemas Comuns
- Erro "ModuleNotFoundError: No module named 'win10toast'": Certifique-se de que o Python utilizado para rodar o script possui o pacote win10toast instalado.
- Erro "Acesso negado" ao iniciar o serviço: Verifique as permissões do serviço e do diretório monitorado.

## Contribuição
Sinta-se à vontade para abrir issues e enviar pull requests para melhorias neste projeto!
