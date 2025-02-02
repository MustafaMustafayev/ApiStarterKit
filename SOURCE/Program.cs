using SOURCE;
using SOURCE.Workers;

Console.WriteLine("Hello, World!");
Console.WriteLine("Please tell me, what you want to build?");
Console.WriteLine("1 - Source Codes For Entity");

int typeOfBuild;

if (!int.TryParse(Console.ReadLine(), out typeOfBuild))
{
    Console.WriteLine("Pick valid number!");
}

if (typeOfBuild == 1)
{
    await EntityBuilderDialogAsync();
}

/* refit removed temporary
else if (typeOfBuild == 2)
{
    await ClientBuilderDialogAsync();
}
*/

async Task EntityBuilderDialogAsync()
{
    Console.WriteLine("I am starting to build, please wait...");
    Console.WriteLine("I am not stuck, just working hard on millions of lines, please be patient...");

    var sourceBuilder = SourceBuilder.Instance;
    Console.WriteLine(
        await sourceBuilder.BuildSourceFiles()
            ? "I generated all of your code."
            : "Error has happened during process (:"
    );
}