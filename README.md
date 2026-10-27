# DanfeSharp

DanfeSharp é uma biblioteca em C# que gera em PDF os documentos auxiliares dos documentos fiscais eletrônicos brasileiros.

Este projeto é um fork do [DanfeSharp](https://github.com/SilverCard/DanfeSharp), criado por Ricardo Brito, e é distribuído como o pacote `DanfeSharp.Next`.

A biblioteca [PDFsharp](https://www.pdfsharp.net/) é utilizada para a escrita dos arquivos em PDF.

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

## Plataformas

| Plataforma | Versão do PDFsharp | Fontes |
|---|---|---|
| .NET Framework 4.5.2 ou superior | 1.50 | Times New Roman instalada no Windows |
| .NET 8 ou superior (Windows, Linux e macOS) | 6.x | Times New Roman ou Liberation Serif instaladas |

No Linux, instale a Liberation Serif, que tem as mesmas métricas da Times New Roman (pacote `fonts-liberation` no Debian/Ubuntu).
Também é possível informar os arquivos da fonte antes de gerar o primeiro documento:

```csharp
DanfeSharp.Fontes.Registrar("serif-regular.ttf", "serif-negrito.ttf", "serif-italico.ttf");
```

## Uso

Todos os documentos seguem o mesmo padrão: criar o modelo a partir do XML, gerar e salvar. O logotipo é opcional
e aceita imagens (`AdicionarLogoImagem`) ou PDF (`AdicionarLogoPdf`).

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

O modelo também pode ser criado e preenchido de outra forma:

```csharp
var modelo = new DanfeViewModel()
{
    NfNumero = 123456,
    NfSerie = 123,
    ChaveAcesso = "123456987...",
    Emitente = new EmpresaViewModel()
    {
        CnpjCpf = "123456...",
        RazaoSocial = "DanfeSharp Ltda",
        ...
    }
};
```

### DANFE NFC-e

```csharp
using DanfeSharp.NFCe;

var modelo = DanfceViewModelCreator.CriarDeArquivoXml("nfce.xml");
modelo.LarguraBobina = LarguraBobina.Mm58; // padrão: Mm80

using (var danfce = new Danfce(modelo))
{
    danfce.Gerar();
    danfce.Salvar("danfce.pdf");
}
```

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

### DACTE

```csharp
using DanfeSharp.CTe;

var modelo = DacteViewModelCreator.CriarDeArquivoXml("cte.xml");

using (var dacte = new Dacte(modelo))
{
    dacte.Gerar();
    dacte.Salvar("dacte.pdf");
}
```

## Licença

MIT. Veja [LICENSE.txt](LICENSE.txt), que mantém o aviso de copyright do projeto original.
