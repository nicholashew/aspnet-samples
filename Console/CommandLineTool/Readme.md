# CommandLineExample

A sample of .Net Core Command Line Application with arguments

## Commands

```bash
$ ninja -?
Usage: ninja [options] [command]

Options:
  -?|-h|--help  Show help information

Commands:
  attack  Instruct the ninja to go and attack!
  hide    Instruct the ninja to hide in a specific location.

Use "ninja [command] --help" for more information about a command.
```

```bash
$ ninja hide
Ninja is hidden under a turtle

$ ninja hide "on top of a street lamp"
Ninja is hidden on top of a street lamp

$ ninja hide --help
Usage: ninja hide [arguments] [options]

Arguments:
  [location]  Where the ninja should hide.

Options:
  -?|-h|--help  Show help information
```

```bash
$ ninja attack
Ninja is attacking dragons, badguys, civilians, animals

$ ninja attack --scream
Ninja is attacking dragons, badguys, civilians, animals while screaming

$ ninja attack -e dragons -s --exclude=animals
Ninja is attacking badguys, civilians while screaming

$ ninja attack -?
Usage: ninja attack [options]

Options:
  -?|-h|--help               Show help information
  -e|--exclude <exclusions>  Things to exclude while attacking.
  -s|--scream                Scream while attacking
```

## Generate the exe

If you really want to generate the exe then just run below command:

Debug Build
```bash
dotnet publish -c Debug -r win10-x64
```

Release Build
```bash
dotnet publish -c Release -r win10-x64 --self-contained false
```

## Reference

- https://gist.github.com/iamarcel/8047384bfbe9941e52817cf14a79dc34#orgheadline8
