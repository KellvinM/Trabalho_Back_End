param([string]$BaseUrl = 'http://127.0.0.1:5087')
$ErrorActionPreference = 'Stop'
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

function Get-Page([string]$path) {
    Invoke-WebRequest "$BaseUrl$path" -WebSession $session -UseBasicParsing
}

function Post-Form([string]$path, [string]$page, [hashtable]$fields) {
    $response = Get-Page $page
    $match = [regex]::Match($response.Content, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"')
    if (-not $match.Success) { throw "Token antiforgery ausente em $page" }
    $fields['__RequestVerificationToken'] = $match.Groups[1].Value
    Invoke-WebRequest "$BaseUrl$path" -Method Post -Body $fields -WebSession $session -UseBasicParsing
}

$email = "smoke-$([guid]::NewGuid().ToString('N'))@example.test"
$password = 'SenhaForte123!'
$response = Post-Form '/Conta/Cadastro' '/Conta/Cadastro' @{
    Nome = 'Usuario de teste'; Email = $email; Senha = $password; ConfirmarSenha = $password
}
if ($response.Content -notmatch 'Minhas consultas') { throw 'Cadastro não abriu a agenda' }

$response = Post-Form '/Conta/Sair' '/Consultas' @{}
if ($response.Content -notmatch 'Criar conta') { throw 'Logout falhou' }

$response = Post-Form '/Conta/Login' '/Conta/Login' @{ Email = $email; Senha = $password }
if ($response.Content -notmatch 'Minhas consultas') { throw 'Login falhou' }

$future = (Get-Date).AddDays(3).ToString('yyyy-MM-ddTHH:mm')
$response = Post-Form '/Consultas/Criar' '/Consultas/Criar' @{
    Id = 0; Especialidade = 'Cardiologia'; DataHora = $future; Descricao = 'Consulta de teste'
}
if ($response.Content -notmatch 'Consulta de teste') { throw 'Criação falhou' }
$match = [regex]::Match($response.Content, '/Consultas/Editar/([0-9]+)')
if (-not $match.Success) { throw 'ID da consulta não encontrado' }
$id = $match.Groups[1].Value

$response = Post-Form "/Consultas/Editar/$id" "/Consultas/Editar/$id" @{
    Id = $id; Especialidade = 'Dermatologia'; DataHora = $future; Descricao = 'Consulta editada'
}
if ($response.Content -notmatch 'Consulta editada') { throw 'Edição falhou' }

$response = Post-Form "/Consultas/Excluir/$id" "/Consultas/Excluir/$id" @{}
if ($response.Content -match 'Consulta editada') { throw 'Exclusão falhou' }
Write-Output 'Cadastro, logout, login e CRUD de consultas: OK'
