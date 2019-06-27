using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "ninja";
            app.Description = ".NET Core console app with argument parsing.";
            app.HelpOption("-?|-h|--help");

            app.OnExecute(() =>
            {
                Console.WriteLine("Hello World!");
                return 0;
            });

            app.Command("hide", (command) =>
            {
                command.Description = "Instruct the ninja to hide in a specific location.";
                command.HelpOption("-?|-h|--help");

                var locationArgument = command.Argument("[location]", "Where the ninja should hide.");

                command.OnExecute(() =>
                {
                    var location = !string.IsNullOrWhiteSpace(locationArgument.Value)
                        ? locationArgument.Value
                        : "under a turtle";
                    Console.WriteLine("hide has finished. ninja hide " + location);
                    return 0;
                });
            });

            app.Command("attack", (command) =>
            {
                command.Description = "Instruct the ninja to go and attack!";
                command.HelpOption("-?|-h|--help");

                // multiple commnad: -e | --exclude 
                var excludeOption = command.Option("-e|--exclude <exclusions>",
                                        "Things to exclude while attacking.",
                                        CommandOptionType.MultipleValue);

                // single commnad: -s | --scream 
                var screamOption = command.Option("-s|--scream",
                                       "Scream while attacking",
                                       CommandOptionType.NoValue);

                command.OnExecute(() =>
                {
                    var exclusions = excludeOption.Values;
                    var attacking = (new List<string>
                    {
                        "dragons",
                        "badguys",
                        "civilians",
                        "animals"
                    })
                    .Where(x => !exclusions.Contains(x));

                    Console.Write("Ninja is attacking " + string.Join(", ", attacking));
                    if (screamOption.HasValue())
                    {
                        Console.Write(" while screaming");
                    }
                    Console.WriteLine();
                    return 0;
                });
            });

            app.Execute(args);

            /*
            var basicOption = app.Option("-o|--option&amp;lt;optionvalue&amp;gt;",
                "Some option value",
                CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (basicOption.HasValue())
                {
                    Console.WriteLine("Option was selected, value: {0}", basicOption.Value());
                }
                else
                {
                    app.ShowHint();
                }

                return 0;
            });

            */

            // var provider = ConfigureServices();

            // var app = new CommandLineApplication<Application>();
            // app.Conventions
            //     .UseDefaultConventions()
            //     .UseConstructorInjection(provider);

            // app.Execute(args);
        }
    }

    // public static ServiceProvider ConfigureServices()
    // {
    //     var services = new ServiceCollection();

    //     services.AddLogging(c => c.AddConsole());
    //     services.AddSingleton<IFileSystem, FileSystem>();
    //     services.AddSingleton<IMarkdownToHtml, MarkdownToHtml>();

    //     return services.BuildServiceProvider();
    // }
}
