# Tracer

[![Build status](https://dev.azure.com/aiursoft/Star/_apis/build/status/Tracer%20Build)](https://dev.azure.com/aiursoft/Star/_build/latest?definitionId=1)

Tracer is a simple network speed test app. Deploy this on your own server. Open your tracer on your browser on your own device. And you can test your speed between you and your server or diagnose network issue.

## Try

Try a running tracer [here](https://tracer.aiursoft.com).

## Deploy to Azure

With the following ARM template you can automate the creation of the resources for this website.

[![Deploy to Azure](https://azuredeploy.net/deploybutton.svg)](https://deploy.azure.com/?repository=https://github.com/AiursoftWeb/Tracer/tree/master)

## Requirements

Requirements about how to run

* [.NET Core runtime 3.1.0 or later](https://github.com/dotnet/core/tree/master/release-notes)

Requirements about how to develope

* [.NET Core SDK 3.1.0 or later](https://github.com/dotnet/core/tree/master/release-notes)
* [VS Code](https://code.visualstudio.com) (Strongly suggest)

## How to run locally

1. Excute `dotnet restore` to restore all dotnet requirements
2. Excute `dotnet run` to run the app
3. Use your browser to view [http://localhost:5000](http://localhost:5000)

## How to run in Microsoft Visual Studio

1. Open the `.sln` file in the project path. 
2. Press `F5`.

## How to run in docker

Pull the container using command:

```bash
$ docker pull anduin2019/tracer:1.0.0
$ docker run -d -p 8080:80 anduin2019/tracer:1.0.0
```

That will start a web server at `http://localhost:8080` and you can test the app.

## How to build locally in docker

Just install docker and docker-compose. Execute the following command.

```bash
$ docker build -t=tracer .
$ docker run -d -p 8080:80 tracer
```

That will start a web server at `http://localhost:8080` and you can test the app.


## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you have push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your personal workflow cruft out of sight.

We're also interested in your feedback for the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.
