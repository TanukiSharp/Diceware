# Overview

This project is a .NET command line tool that randomly generates pass-phrases based on the Diceware(TM) concept.<br/>
http://world.std.com/~reinhold/diceware.html

# Web application

You can use a web version [here](https://tanukisharp.github.io/Diceware/).

# Install

You must have .NET 6 or higher.

Open a command line prompt and type:
```
dotnet tool install -g diceware
```
Then you should be able to use the `diceware` command line from anywhere.

# Options

First, you can type `diceware --help` to print the help message, and `diceware --version` to display the version you have currently installed.

## Word count

Run with options `-c <COUNT>` or `--word-count <COUNT>` to define how many words you want in your pass-phrase. Defaults to 6.

## Extra security

Run with options `-s` or `--extra-security` to introduce a symbol at a random location in a word of you pass-phrase.

## Download URL

Run with options `-u <URL>` or `--download-url <URL>` to choose which word list to user. Defaults to `http://world.std.com/~reinhold/diceware.wordlist.asc`.

**Note:** If the URL is `sheme://domain/segment/the-words-file.ext`, a file named `the-words-file.ext` will be downloaded and cached for future reuse.

## Force download

Run with options `-f` or `--force-download` to force downloading the words list file, even if it is already cached. Defaults to false.
