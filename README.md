# Sistema de Gestão de Consultas UVV

Aplicação ASP.NET Core MVC em C# para cadastro e login de usuários e gerenciamento de consultas. Cada usuário só acessa os próprios registros. O banco é SQL Server, criado pelo Entity Framework Core com a migration incluída em `Migrations/`.

## Requisitos

- .NET SDK 10.0
- SQL Server instalado e em execução (a configuração padrão usa a instância local `.` com autenticação Windows)
- Ferramenta `dotnet-ef` versão 10, para usar a CLI; ou o Package Manager Console do Visual Studio

## Configuração e execução

1. Ajuste `ConnectionStrings:DefaultConnection` em `appsettings.json` para a instância SQL Server disponível. A string padrão é `Server=.;Database=ConsultasUVV;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;`. Se usar LocalDB, substitua `Server=.` por `Server=(localdb)\MSSQLLocalDB`. Para não incluir credenciais no repositório, use User Secrets ou a variável de ambiente `ConnectionStrings__DefaultConnection` quando necessário.
2. Na pasta do projeto, execute:

   ```powershell
   dotnet restore
   dotnet tool install --global dotnet-ef --version 10.0.12
   dotnet ef database update
   dotnet run
   ```

   No **Package Manager Console** do Visual Studio, selecione este projeto como projeto padrão e execute `Update-Database` no lugar de `dotnet ef database update`.

3. Abra a URL exibida pelo `dotnet run`. Faça o cadastro, entre na conta e crie, edite e exclua uma consulta.

O comando de atualização aplica a migration inicial ao banco configurado. Para criar novas migrations após alterar os modelos, use `dotnet ef migrations add NomeDaAlteracao` e depois `dotnet ef database update`.

## Organização

- `Models/`: entidades e Data Annotations.
- `Data/`: `AppDbContext`, índices e relacionamento.
- `ViewModels/`: campos aceitos pelos formulários; o `UsuarioId` nunca vem do navegador.
- `Controllers/`: autenticação e CRUD com ações GET/POST.
- `Views/` e `wwwroot/`: interface MVC e estilos.
- `Migrations/`: esquema Code First do SQL Server.

O login usa autenticação por cookie; a senha é armazenada por hash com `PasswordHasher<Usuario>`. Todas as ações de consultas exigem autenticação e verificam o dono do registro. Os formulários POST usam token antiforgery, inclusive sair e excluir.

## Demonstração e entrega

**Vídeo demonstrativo:** [Adicionar link do vídeo após a gravação](COLOCAR_LINK_DO_VIDEO_AQUI)

Antes de entregar, substitua o link acima pelo vídeo real gravado via Loom, YouTube ou similar, mostrando cadastro, login e CRUD de consultas. Publique o projeto em um repositório GitHub acessível ao professor. O representante do grupo deve enviar no portal um PDF com os participantes em ordem alfabética e os links do repositório, deste README e do vídeo. Esses links e os nomes precisam ser preenchidos com os dados reais do grupo.
