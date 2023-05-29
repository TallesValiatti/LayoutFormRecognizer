
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using ConsoleTables;

string endpoint = "<YOUR-ENDPOINT>";
string key = "<YOUR-KEY>";

AzureKeyCredential credential = new AzureKeyCredential(key);
DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

var fileUrl = new Uri("<YOUR-FILE-URL>");

AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-layout", fileUrl);

AnalyzeResult result = operation.Value;

Console.WriteLine($"{result.Tables.Count} table found");
Console.WriteLine(Environment.NewLine);

foreach (var tableItem in result.Tables.Select((table, index) => new {table, index}))
{
    Console.WriteLine($"****** Table {tableItem.index} ******");
    
    Console.WriteLine(Environment.NewLine);

    var headersCells = tableItem.table.Cells.Where(x => x.RowIndex == 0);
    var contentCells = tableItem.table.Cells.Where(x => x.RowIndex != 0);

    var headers = headersCells.Select(x => x.Content).ToArray();

    var table = new ConsoleTable(headers);
    
    for (int i = 1; i < tableItem.table.RowCount; i++)
    {
        var line = contentCells
            .Where(x => x.RowIndex == i)
            .OrderBy(x => x.ColumnIndex)
            .Select(x => x.Content)
            .ToArray();

        table.AddRow(line);
    }

    table.Write();
}