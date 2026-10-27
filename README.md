# DanfeSharp.Next

Biblioteca em C# que gera em PDF os documentos auxiliares dos documentos fiscais eletrônicos brasileiros a partir
do XML autorizado: DANFE da NF-e, DANFE NFC-e, documentos de eventos (carta de correção, cancelamento e outros) e
DACTE do CT-e. Já traz o leiaute da Reforma Tributária, com IBS, CBS e Imposto Seletivo (NT 2025.002 e NT 2026.010).

Este projeto é um fork do [DanfeSharp](https://github.com/SilverCard/DanfeSharp), criado por Ricardo Brito, e é
distribuído como o pacote `DanfeSharp.Next`. Os PDFs são escritos com a biblioteca [PDFsharp](https://www.pdfsharp.net/).

## Documentos suportados

| Documento | Classe | Observações |
|---|---|---|
| DANFE da NF-e (modelo 55) | `DanfeSharp.Danfe` | Retrato e paisagem. Leiaute da Reforma Tributária (NT 2026.010) com IBS, CBS e IS quando a nota os possui |
| DANFE NFC-e (modelo 65) | `DanfeSharp.NFCe.Danfce` | Bobina de 80 mm ou 58 mm, com QR Code. Aceita NFC-e em contingência offline, ainda sem protocolo |
| Eventos da NF-e e da NFC-e | `DanfeSharp.Eventos.DocumentoEvento` | Carta de correção (CC-e), cancelamento, cancelamento por substituição e demais eventos |
| DACTE do CT-e 4.00 (modelo 57) | `DanfeSharp.CTe.Dacte` | Com QR Code, IBS e CBS quando informados e documentos originários em várias folhas |

## Instalação

```
dotnet add package DanfeSharp.Next
```

Ou, no Visual Studio, procure por `DanfeSharp.Next` no Gerenciador de Pacotes NuGet.

## Plataformas

| Plataforma | Versão do PDFsharp | Fontes |
|---|---|---|
| .NET Framework 4.5.2 ou superior | 1.50 | Times New Roman instalada no Windows |
| .NET 8 ou superior (Windows, Linux e macOS) | 6.x | Times New Roman ou Liberation Serif instaladas |

No Linux, instale a Liberation Serif, que tem as mesmas métricas da Times New Roman (pacote `fonts-liberation` no
Debian/Ubuntu). Também é possível informar os arquivos da fonte antes de gerar o primeiro documento, o que é útil em
contêineres sem fontes instaladas:

```csharp
DanfeSharp.Fontes.Registrar("serif-regular.ttf", "serif-negrito.ttf", "serif-italico.ttf");
```

## Como usar

Todos os documentos seguem o mesmo padrão:

1. criar o modelo a partir do XML, com a classe `...ViewModelCreator` do documento;
2. ajustar as opções do modelo, se quiser;
3. criar o documento, adicionar o logotipo (opcional), chamar `Gerar()` e `Salvar(...)`.

O modelo pode ser criado de um arquivo (`CriarDeArquivoXml(caminho)`), de um `Stream` (`CriarDeArquivoXml(stream)`)
ou de uma string (`CriarDeStringXml(xml)`). Para os eventos, só há as versões com caminho e com string. O documento
implementa `IDisposable`, então use-o dentro de um `using`.

### DANFE da NF-e

```csharp
using DanfeSharp;
using DanfeSharp.Modelo;

var modelo = DanfeViewModelCreator.CriarDeArquivoXml("nfe.xml");

using (var danfe = new Danfe(modelo))
{
    danfe.AdicionarLogoImagem("logo.jpg");
    danfe.Gerar();
    danfe.Salvar("danfe.pdf");
}
```

O XML deve ser a NF-e autorizada (`nfeProc`). A orientação (retrato ou paisagem) vem do campo `tpImp` do XML e pode
ser trocada em `modelo.Orientacao`.

#### Opções do DANFE

| Propriedade de `DanfeViewModel` | Padrão | Efeito |
|---|---|---|
| `Orientacao` | do XML | `Orientacao.Retrato` ou `Orientacao.Paisagem` |
| `ExibirLeiauteReformaTributaria` | automático | `null` usa o leiaute da NT 2026.010 só quando a nota tem IBS, CBS ou IS; `true` ou `false` força |
| `PreferirEmitenteNomeFantasia` | `true` | Mostra o nome fantasia do emitente no lugar da razão social |
| `ExibirPisConfins` | `true` | Exibe os valores de PIS e COFINS |
| `ExibirIcmsInterestadual` | `true` | Exibe os valores de ICMS interestadual (DIFAL) |
| `ExibirBlocoLocalEntrega` | `true` | Exibe o quadro do local de entrega, quando houver |
| `ExibirBlocoLocalRetirada` | `true` | Exibe o quadro do local de retirada, quando houver |

O modelo também pode ser montado sem XML, preenchendo `DanfeViewModel`, `EmpresaViewModel`, `ProdutoViewModel` e as
demais classes do namespace `DanfeSharp.Modelo`.

### DANFE NFC-e

```csharp
using DanfeSharp.NFCe;

var modelo = DanfceViewModelCreator.CriarDeArquivoXml("nfce.xml");
modelo.LarguraBobina = LarguraBobina.Mm58; // padrão: Mm80

using (var danfce = new Danfce(modelo))
{
    danfce.AdicionarLogoImagem("logo.png");
    danfce.Gerar();
    danfce.Salvar("danfce.pdf");
}
```

O documento sai em uma única página contínua, na largura da bobina, pronta para impressoras térmicas. O XML pode ser a
NFC-e autorizada (`nfeProc`) ou a NFC-e emitida em contingência offline (`NFe`, ainda sem protocolo).

### Carta de correção, cancelamento e outros eventos

```csharp
using DanfeSharp.Eventos;

// O XML da NF-e é opcional e serve para exibir emitente, destinatário e data de emissão.
var modelo = EventoViewModelCreator.CriarDeArquivoXml("procEventoNFe.xml", "nfe.xml");

using (var evento = new DocumentoEvento(modelo))
{
    evento.Gerar();
    evento.Salvar("cce.pdf");
}
```

O XML deve ser o evento processado (`procEventoNFe`). Eventos sem leiaute próprio mostram os campos do evento como
"campo: valor".

### DACTE

```csharp
using DanfeSharp.CTe;

var modelo = DacteViewModelCreator.CriarDeArquivoXml("cte.xml");

using (var dacte = new Dacte(modelo))
{
    dacte.AdicionarLogoImagem("logo.jpg");
    dacte.Gerar();
    dacte.Salvar("dacte.pdf");
}
```

O XML deve ser o CT-e autorizado (`cteProc`). Quando os documentos originários não cabem na primeira folha, eles
continuam nas folhas seguintes.

### Logotipo

Todos os documentos aceitam o logotipo em imagem (`AdicionarLogoImagem`, JPG ou PNG) ou em PDF (`AdicionarLogoPdf`,
que mantém o desenho vetorial), por caminho de arquivo ou por `Stream`.

### Gerar em memória (APIs web)

`Salvar(Stream)` grava o PDF sem fechar o stream, o que permite devolvê-lo direto em uma API:

```csharp
// ASP.NET Core
app.MapPost("/danfe", (HttpRequest request) =>
{
    var modelo = DanfeViewModelCreator.CriarDeArquivoXml(request.Body);
    var pdf = new MemoryStream();

    using (var danfe = new Danfe(modelo))
    {
        danfe.Gerar();
        danfe.Salvar(pdf);
    }

    pdf.Position = 0;
    return Results.File(pdf, "application/pdf", "danfe.pdf");
});
```

### Erros

- XML malformado ou de outro tipo de documento: `XmlException` ou `Exception` com a linha e a posição do erro.
- NFC-e passada ao `DanfeViewModelCreator`: `NotSupportedException`, indicando o uso do `Danfce`.
- Forma de emissão não suportada pelo DANFE: `NotSupportedException`.

## Compilar a partir do código

```
dotnet build DanfeSharp.sln
dotnet test DanfeSharp.Test/DanfeSharp.Test.csproj
dotnet pack DanfeSharp/DanfeSharp.csproj -c Release -o nupkgs
```

Os testes geram os PDFs de exemplo na pasta `Output` dentro de `DanfeSharp.Test/bin`, a partir dos XMLs fictícios em
`DanfeSharp.Test/Xml`.

## Limitações

- Os leiautes da NFC-e, dos eventos e do DACTE ainda não foram conferidos com notas reais. Valide com os seus
  documentos antes de usar em produção.
- Não há norma que defina o IBS e a CBS no DACTE nem na NFC-e. Os quadros atuais são provisórios.
- Ainda não são gerados: DANFSe, DAMDFE, NFC-e em A4 e DACTE OS (modelo 67).

## Licença

MIT. Veja [LICENSE.txt](LICENSE.txt), que mantém o aviso de copyright do projeto original.
