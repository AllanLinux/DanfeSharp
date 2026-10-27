# DanfeSharp

DanfeSharp é uma biblioteca em C# que permite a geração do DANFE em formato PDF.

A biblioteca [PDFsharp](https://www.pdfsharp.net/) é utilizada para a escrita dos arquivos em PDF.

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
Também é possível informar os arquivos da fonte antes de gerar o primeiro DANFE:

```csharp
DanfeSharp.Fontes.Registrar("serif-regular.ttf", "serif-negrito.ttf", "serif-italico.ttf");
```

## Uso

Exemplo de uso:
```csharp

using DanfeSharp;
using DanfeSharp.Modelo;

//Cria o modelo a partir do arquivo Xml da NF-e.
var modelo = DanfeViewModelCreator.CriarDeArquivoXml("nfe.xml");


//O modelo também pode ser criado e preenchido de outra forma.
var modelo = new DanfeViewModel()
{
    NfNumero = 123456,
    NfSerie = 123,
    ChaveAcesso = "123456987...",
    Emitente = new EmpresaViewModel()
    {
        CnpjCpf = "123456...",
        Nome = "DanfeSharp Ltda",    
	...


//Inicia o Danfe com o modelo criado
using (var danfe = new Danfe(modelo))
{
	danfe.Gerar();
	danfe.Salvar("danfe.pdf");
}
```


